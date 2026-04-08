using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Godot;
using STS2CompanionMod.Data;
using STS2CompanionMod.Overlay;

namespace STS2CompanionMod.Services;

/// <summary>
/// 두 API에서 카드/렐릭 티어 데이터를 비동기로 가져와 캐시하고
/// CardDatabase를 런타임 업데이트한다.
///
/// 데이터 소스:
///   1. QuestceSpire — 커뮤니티 승률 (기본 소스)
///   2. Spire Codex  — 카드/렐릭 메타데이터 (이름 매핑용)
/// </summary>
public sealed class OnlineDataService : IDisposable
{
    // API 엔드포인트
    private const string QuestceBase   = "https://questcespire-api.questcespire.workers.dev/api";
    private const string SpireCodexBase = "https://spire-codex.com/api";

    // 캐시 유효 시간 (시간 단위)
    private const int CacheValidHours = 6;

    // 최소 표본 수 (이하는 무시)
    private const int MinSamples = 10;

    private readonly HttpClient _http;
    private readonly string     _cachePath;

    // 현재 로드된 데이터
    public TierCache? CurrentCache { get; private set; }
    public bool       IsLoaded     { get; private set; }
    public string     StatusText   { get; private set; } = "데이터 로딩 중...";

    // 데이터 갱신 이벤트
    public event Action? OnDataUpdated;

    public OnlineDataService()
    {
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15),
        };
        _http.DefaultRequestHeaders.Add("User-Agent", "STS2CompanionMod/1.0");

        // Godot 사용자 데이터 디렉토리에 캐시 저장
        _cachePath = System.IO.Path.Combine(
            OS.GetUserDataDir(), "sts2_companion_tier_cache.json");
    }

    /// <summary>
    /// 비동기 초기화. 모드 로드 직후 호출.
    /// 캐시가 신선하면 캐시 사용, 만료되면 네트워크 요청.
    /// </summary>
    public async Task InitializeAsync()
    {
        // 1. 캐시 시도
        if (TryLoadCache(out var cached) && cached is not null)
        {
            ApplyToDatabase(cached);
            StatusText = $"온라인 데이터 로드됨 ({cached.FetchedAt[..10]})";
            IsLoaded   = true;
            OnDataUpdated?.Invoke();
            CompanionLogger.Log($"Tier cache loaded from disk ({cached.Cards.Count} cards, {cached.Relics.Count} relics)");

            // 캐시가 낡았으면 백그라운드에서 갱신
            if (IsCacheStale(cached))
                _ = FetchAndUpdateAsync();
            return;
        }

        // 2. 네트워크 요청
        await FetchAndUpdateAsync();
    }

    /// <summary>네트워크에서 최신 데이터 가져오기</summary>
    public async Task FetchAndUpdateAsync()
    {
        StatusText = "온라인 데이터 갱신 중...";
        OnDataUpdated?.Invoke();

        try
        {
            CompanionLogger.Log("Fetching tier data from QuestceSpire + SpireCodex...");

            // 두 API를 병렬 호출
            var statsTask = FetchQuestceStatsAsync();
            var relicsTask = FetchSpireCodexRelicsAsync();

            await Task.WhenAll(statsTask, relicsTask);

            var stats  = statsTask.Result;
            var relics = relicsTask.Result;

            if (stats is null)
            {
                StatusText = "데이터 로드 실패 (네트워크 오류)";
                CompanionLogger.Warn("QuestceSpire fetch returned null.");
                return;
            }

            var cache = BuildCache(stats, relics);
            SaveCache(cache);
            ApplyToDatabase(cache);

            CurrentCache = cache;
            IsLoaded     = true;
            StatusText   = $"온라인 데이터 로드됨 — {stats.TotalRuns:N0}개 런 기반 " +
                           $"({cache.FetchedAt[..10]})";

            OnDataUpdated?.Invoke();
            CompanionLogger.Log($"Tier data updated: {cache.Cards.Count} cards, {cache.Relics.Count} relics.");
        }
        catch (Exception ex)
        {
            StatusText = "데이터 로드 실패";
            CompanionLogger.Warn($"OnlineDataService fetch failed: {ex.Message}");
        }
    }

    // ── 내부: API 호출 ────────────────────────────────────────────────────

    private async Task<StatsPayload?> FetchQuestceStatsAsync()
    {
        // 전체 통계 (캐릭터 필터 없음 — 한 번에 모두)
        string url = $"{QuestceBase}/stats?min_samples={MinSamples}";
        try
        {
            return await _http.GetFromJsonAsync<StatsPayload>(url);
        }
        catch (Exception ex)
        {
            CompanionLogger.Warn($"QuestceSpire fetch error: {ex.Message}");
            return null;
        }
    }

    private async Task<List<SpireRelic>> FetchSpireCodexRelicsAsync()
    {
        // Spire Codex에서 렐릭 전체 목록 (이름 매핑용)
        string url = $"{SpireCodexBase}/relics";
        try
        {
            return await _http.GetFromJsonAsync<List<SpireRelic>>(url)
                   ?? new List<SpireRelic>();
        }
        catch (Exception ex)
        {
            CompanionLogger.Warn($"SpireCodex relics fetch error: {ex.Message}");
            return new List<SpireRelic>();
        }
    }

    // ── 내부: 캐시 빌드 ──────────────────────────────────────────────────

    private static TierCache BuildCache(StatsPayload stats, List<SpireRelic> spireRelics)
    {
        // SpireCodex 렐릭 이름 사전 (ID → 이름)
        var relicNames = spireRelics.ToDictionary(
            r => r.Id, r => r.Name, StringComparer.OrdinalIgnoreCase);

        var cards = stats.CardStats
            .Where(s => s.SampleSize >= MinSamples)
            .Select(s =>
            {
                double delta = s.WinRateWhenPicked - s.WinRateWhenSkipped;
                return new OnlineTierEntry(
                    Id:           s.CardId,
                    Name:         FormatName(s.CardId),
                    CharacterId:  s.Character,
                    Tier:         TierMapper.ForCard(delta, s.SampleSize),
                    WinRateDelta: delta,
                    WinRatePicked: s.WinRateWhenPicked,
                    SampleSize:   s.SampleSize,
                    Source:       "questcespire"
                );
            })
            .ToList();

        var relics = stats.RelicStats
            .Where(s => s.SampleSize >= MinSamples)
            .Select(s =>
            {
                double delta = s.WinRateWhenPicked - s.WinRateWhenSkipped;
                relicNames.TryGetValue(s.RelicId, out string? displayName);
                return new OnlineTierEntry(
                    Id:           s.RelicId,
                    Name:         displayName ?? FormatName(s.RelicId),
                    CharacterId:  s.Character,
                    Tier:         TierMapper.ForRelic(delta, s.SampleSize),
                    WinRateDelta: delta,
                    WinRatePicked: s.WinRateWhenPicked,
                    SampleSize:   s.SampleSize,
                    Source:       "questcespire"
                );
            })
            .ToList();

        return new TierCache(
            FetchedAt:   DateTime.UtcNow.ToString("O"),
            GameVersion: "early-access",
            Cards:       cards,
            Relics:      relics
        );
    }

    // ── 내부: CardDatabase 적용 ──────────────────────────────────────────

    private static void ApplyToDatabase(TierCache cache)
    {
        // 렐릭 티어 업데이트
        var relicUpdates = cache.Relics
            .ToDictionary(
                r => r.Name,
                r => TierMapper.ToRelicTier(r.Tier),
                StringComparer.OrdinalIgnoreCase);

        CardDatabase.UpdateRelicTiers(relicUpdates);

        // 카드 승률 메타 업데이트
        var cardMeta = cache.Cards.ToDictionary(
            c => c.Id,
            c => c,
            StringComparer.OrdinalIgnoreCase);

        CardDatabase.UpdateCardMeta(cardMeta);
    }

    // ── 내부: 캐시 직렬화 ────────────────────────────────────────────────

    private bool TryLoadCache(out TierCache? cache)
    {
        cache = null;
        if (!System.IO.File.Exists(_cachePath)) return false;
        try
        {
            string json = System.IO.File.ReadAllText(_cachePath);
            cache = JsonSerializer.Deserialize<TierCache>(json);
            return cache is not null;
        }
        catch
        {
            return false;
        }
    }

    private void SaveCache(TierCache cache)
    {
        try
        {
            string json = JsonSerializer.Serialize(cache,
                new JsonSerializerOptions { WriteIndented = false });
            System.IO.File.WriteAllText(_cachePath, json);
        }
        catch (Exception ex)
        {
            CompanionLogger.Warn($"Cache save failed: {ex.Message}");
        }
    }

    private static bool IsCacheStale(TierCache cache)
    {
        if (!DateTime.TryParse(cache.FetchedAt, out var fetchedAt)) return true;
        return (DateTime.UtcNow - fetchedAt).TotalHours >= CacheValidHours;
    }

    /// <summary>SCREAMING_SNAKE_CASE ID → Title Case 이름 변환 (fallback용)</summary>
    private static string FormatName(string id) =>
        string.Join(" ", id.Replace('_', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));

    public void Dispose() => _http.Dispose();
}
