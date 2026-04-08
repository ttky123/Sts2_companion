using System.Text.Json.Serialization;

namespace STS2CompanionMod.Services;

// ── Spire Codex API (/api/cards, /api/relics) ─────────────────────────────

public record SpireCard(
    [property: JsonPropertyName("id")]          string Id,
    [property: JsonPropertyName("name")]        string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("cost")]        int? Cost,
    [property: JsonPropertyName("type")]        string Type,
    [property: JsonPropertyName("rarity")]      string Rarity,
    [property: JsonPropertyName("color")]       string Color,
    [property: JsonPropertyName("keywords")]    string[]? Keywords
);

public record SpireRelic(
    [property: JsonPropertyName("id")]          string Id,
    [property: JsonPropertyName("name")]        string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("rarity")]      string Rarity,
    [property: JsonPropertyName("rarity_key")]  string RarityKey,
    [property: JsonPropertyName("pool")]        string Pool
);

// ── QuestceSpire Stats API (/api/stats) ───────────────────────────────────

public record StatsPayload(
    [property: JsonPropertyName("version")]      int Version,
    [property: JsonPropertyName("card_stats")]   List<CardStat> CardStats,
    [property: JsonPropertyName("relic_stats")]  List<RelicStat> RelicStats,
    [property: JsonPropertyName("total_runs")]   int TotalRuns,
    [property: JsonPropertyName("last_updated")] string LastUpdated
);

public record CardStat(
    [property: JsonPropertyName("CardId")]             string CardId,
    [property: JsonPropertyName("Character")]          string Character,
    [property: JsonPropertyName("PickRate")]           double PickRate,
    [property: JsonPropertyName("WinRateWhenPicked")]  double WinRateWhenPicked,
    [property: JsonPropertyName("WinRateWhenSkipped")] double WinRateWhenSkipped,
    [property: JsonPropertyName("SampleSize")]         int SampleSize,
    [property: JsonPropertyName("AvgFloorPicked")]     double AvgFloorPicked
);

public record RelicStat(
    [property: JsonPropertyName("RelicId")]            string RelicId,
    [property: JsonPropertyName("Character")]          string Character,
    [property: JsonPropertyName("PickRate")]           double PickRate,
    [property: JsonPropertyName("WinRateWhenPicked")]  double WinRateWhenPicked,
    [property: JsonPropertyName("WinRateWhenSkipped")] double WinRateWhenSkipped,
    [property: JsonPropertyName("SampleSize")]         int SampleSize,
    [property: JsonPropertyName("AvgFloorPicked")]     double AvgFloorPicked
);

// ── 내부 통합 모델 ─────────────────────────────────────────────────────────

/// <summary>온라인 소스에서 취합한 카드/렐릭 티어 정보</summary>
public record OnlineTierEntry(
    string Id,
    string Name,
    string CharacterId,     // "" = 공용
    OnlineTier Tier,
    double WinRateDelta,    // WinRateWhenPicked - WinRateWhenSkipped
    double WinRatePicked,
    int SampleSize,
    string Source           // "questcespire" | "spire-codex"
);

public enum OnlineTier { S, A, B, C, D }

/// <summary>로컬 캐시 파일 포맷</summary>
public record TierCache(
    string FetchedAt,       // ISO 8601
    string GameVersion,
    List<OnlineTierEntry> Cards,
    List<OnlineTierEntry> Relics
);
