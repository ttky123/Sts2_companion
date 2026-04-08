using STS2CompanionMod.Data;

namespace STS2CompanionMod.Analysis;

/// <summary>
/// 카드 보상 선택지를 현재 덱 분석 결과와 비교하여 각 카드에 점수/권장 등급을 부여.
/// </summary>
public static class RewardScorer
{
    // 점수 기준
    private const float ScoreCore        = 0.90f;
    private const float ScoreGood        = 0.65f;
    private const float ScoreUniversal   = 0.50f;
    private const float ScoreNeutral     = 0.30f;
    private const float ScoreAvoid       = 0.05f;

    // 이 점수 이상인 아키타입만 "활성 빌드"로 간주
    private const float ActiveBuildThreshold = 0.25f;

    /// <summary>
    /// 보상으로 제시된 카드들을 평가하여 순위를 반환.
    /// </summary>
    /// <param name="offeredCards">보상으로 선택 가능한 카드 이름 목록</param>
    /// <param name="deckAnalysis">DeckAnalyzer.Analyze()의 결과</param>
    /// <param name="characterId">현재 캐릭터</param>
    /// <param name="ascensionLevel">현재 승천 단계</param>
    public static List<RewardCardScore> Score(
        IEnumerable<string> offeredCards,
        List<ArchetypeMatchResult> deckAnalysis,
        string characterId,
        int ascensionLevel)
    {
        // 활성 빌드: 점수가 임계값 이상인 아키타입
        var activeBuilds = deckAnalysis
            .Where(r => r.Score >= ActiveBuildThreshold)
            .ToList();

        // 범용 좋은 카드 세트
        var universalGood = CardDatabase.UniversallyGoodCards.TryGetValue(characterId, out var ug)
            ? new HashSet<string>(ug, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var universalBad = new HashSet<string>(
            CardDatabase.UniversallyBadCards, StringComparer.OrdinalIgnoreCase);

        var results = new List<RewardCardScore>();

        foreach (var cardName in offeredCards)
        {
            var score = EvaluateCard(
                cardName, activeBuilds, universalGood, universalBad, ascensionLevel);
            results.Add(score);
        }

        results.Sort((a, b) => b.Score.CompareTo(a.Score));
        return results;
    }

    private static RewardCardScore EvaluateCard(
        string cardName,
        List<ArchetypeMatchResult> activeBuilds,
        HashSet<string> universalGood,
        HashSet<string> universalBad,
        int ascensionLevel)
    {
        string normalized = DeckAnalyzer.NormalizeCardName(cardName);

        // 1. 저주/부정 카드는 항상 건너뜀
        if (universalBad.Contains(normalized))
            return new RewardCardScore(cardName, ScoreAvoid, "건너뜀",
                "범용 기피 카드 — 런에 부정적 영향", null);

        // 2. 활성 빌드들에서 코어 카드인지 확인
        foreach (var build in activeBuilds)
        {
            bool isCore = build.Archetype.CoreCards
                .Any(c => string.Equals(DeckAnalyzer.NormalizeCardName(c), normalized,
                    StringComparison.OrdinalIgnoreCase));

            if (isCore && !build.MatchedCoreCards
                    .Any(c => string.Equals(DeckAnalyzer.NormalizeCardName(c), normalized,
                        StringComparison.OrdinalIgnoreCase)))
            {
                // 아직 덱에 없는 핵심 카드
                float bonus = build.Score * 0.1f; // 빌드 완성도 높을수록 가산점
                return new RewardCardScore(
                    cardName,
                    Math.Clamp(ScoreCore + bonus, 0f, 1f),
                    "핵심",
                    $"[{build.Archetype.Name}] 미확보 핵심 카드",
                    build.Archetype.Name);
            }

            bool isGood = build.Archetype.GoodCards
                .Any(c => string.Equals(DeckAnalyzer.NormalizeCardName(c), normalized,
                    StringComparison.OrdinalIgnoreCase));

            if (isGood)
                return new RewardCardScore(cardName, ScoreGood, "좋음",
                    $"[{build.Archetype.Name}] 시너지 카드", build.Archetype.Name);

            // 빌드의 회피 카드인지 확인
            bool shouldAvoid = build.Archetype.AvoidCards
                .Any(c => string.Equals(DeckAnalyzer.NormalizeCardName(c), normalized,
                    StringComparison.OrdinalIgnoreCase));

            if (shouldAvoid && build.Score >= 0.50f)
                return new RewardCardScore(cardName, ScoreAvoid, "건너뜀",
                    $"[{build.Archetype.Name}] 현재 빌드와 상충", build.Archetype.Name);
        }

        // 3. 범용 좋은 카드
        if (universalGood.Contains(normalized))
        {
            // 고승천에서는 범용 카드도 빌드 시너지 없으면 가산점 낮춤
            float universalScore = ascensionLevel >= 15 ? ScoreUniversal - 0.1f : ScoreUniversal;
            return new RewardCardScore(cardName, universalScore, "보통",
                "범용적으로 유용한 카드", null);
        }

        // 4. 해당 없음 — 중립
        string neutralReason = ascensionLevel >= 15
            ? "현재 빌드와 시너지 없음 — 고승천에선 건너뜀 권장"
            : "시너지 없음, 덱 두께 증가 주의";

        return new RewardCardScore(cardName, ScoreNeutral, "보통", neutralReason, null);
    }

    /// <summary>
    /// 렐릭 보상을 등급으로 평가.
    /// </summary>
    public static (RelicTier Tier, string Advice) EvaluateRelic(
        string relicName,
        List<ArchetypeMatchResult> deckAnalysis,
        int ascensionLevel)
    {
        if (CardDatabase.RelicTiers.TryGetValue(relicName, out var tier))
        {
            // 활성 빌드의 핵심 렐릭인지 확인
            var matchingBuild = deckAnalysis
                .Where(r => r.Score >= ActiveBuildThreshold)
                .FirstOrDefault(r => r.Archetype.KeyRelics
                    .Any(rr => string.Equals(rr, relicName, StringComparison.OrdinalIgnoreCase)));

            string advice = matchingBuild is not null
                ? $"[{matchingBuild.Archetype.Name}] 핵심 렐릭 — 강력히 추천"
                : tier switch
                {
                    RelicTier.S => "S 티어 렐릭 — 대부분의 빌드에서 강력",
                    RelicTier.A => "A 티어 렐릭 — 빌드 방향 맞으면 픽",
                    RelicTier.B => "B 티어 렐릭 — 현재 빌드 시너지 확인 후 결정",
                    _           => "낮은 티어 렐릭 — 건너뜀 권장",
                };

            return (tier, advice);
        }

        return (RelicTier.B, "DB에 없는 렐릭 — 효과 직접 확인 필요");
    }
}
