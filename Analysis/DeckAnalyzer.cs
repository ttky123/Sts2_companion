using STS2CompanionMod.Data;

namespace STS2CompanionMod.Analysis;

/// <summary>
/// 현재 덱 카드 목록을 받아 각 아키타입과의 매칭 점수를 계산한다.
/// </summary>
public static class DeckAnalyzer
{
    // 코어 카드 매칭 가중치 (good 카드보다 훨씬 중요)
    private const float CoreWeight = 1.0f;
    private const float GoodWeight = 0.35f;

    /// <summary>
    /// 덱을 분석하여 해당 캐릭터의 아키타입 매칭 결과를 점수 순으로 반환.
    /// </summary>
    /// <param name="deckCardNames">현재 덱에 있는 카드 이름 목록 (중복 포함)</param>
    /// <param name="characterId">현재 캐릭터 ID (예: "Ironclad")</param>
    /// <param name="ascensionLevel">현재 승천 단계 0~20</param>
    public static List<ArchetypeMatchResult> Analyze(
        IEnumerable<string> deckCardNames,
        string characterId,
        int ascensionLevel)
    {
        // 중복 제거하여 세트로 변환 (업그레이드 표시 + 제거)
        var deckSet = deckCardNames
            .Select(NormalizeCardName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var archetypes = ArchetypeDatabase.GetForCharacter(characterId);
        var results = new List<ArchetypeMatchResult>(archetypes.Count);

        foreach (var archetype in archetypes)
        {
            var result = CalculateMatch(deckSet, archetype, ascensionLevel);
            results.Add(result);
        }

        // 점수 내림차순 정렬, 동점이면 아키타입 실효 점수로 보조 정렬
        results.Sort((a, b) =>
        {
            int cmp = b.Score.CompareTo(a.Score);
            if (cmp != 0) return cmp;
            return b.Archetype.AscensionViability.GetFor(ascensionLevel)
                .CompareTo(a.Archetype.AscensionViability.GetFor(ascensionLevel));
        });

        return results;
    }

    private static ArchetypeMatchResult CalculateMatch(
        HashSet<string> deckSet,
        ArchetypeData archetype,
        int ascensionLevel)
    {
        var matchedCore    = new List<string>();
        var missingCore    = new List<string>();
        var matchedGood    = new List<string>();

        foreach (var card in archetype.CoreCards)
        {
            if (deckSet.Contains(NormalizeCardName(card)))
                matchedCore.Add(card);
            else
                missingCore.Add(card);
        }

        foreach (var card in archetype.GoodCards)
        {
            if (deckSet.Contains(NormalizeCardName(card)))
                matchedGood.Add(card);
        }

        // 원시 점수: 코어 충족률 + 좋은 카드 보너스
        float coreScore = archetype.CoreCards.Length > 0
            ? (float)matchedCore.Count / archetype.CoreCards.Length
            : 0f;
        float goodBonus = archetype.GoodCards.Length > 0
            ? (float)matchedGood.Count / archetype.GoodCards.Length * GoodWeight
            : 0f;

        // 승천 단계 가중치 (실효 점수 1~5 → 0.6~1.0 배율)
        float ascensionMult = 0.5f + archetype.AscensionViability.GetFor(ascensionLevel) * 0.1f;

        float rawScore = (coreScore * CoreWeight + goodBonus) * ascensionMult;
        float score    = Math.Clamp(rawScore, 0f, 1f);

        // 다음 픽 추천: 미확보 코어 카드 중 최대 3개
        var suggestedPicks = missingCore.Take(3).ToList();

        return new ArchetypeMatchResult(
            Archetype:        archetype,
            Score:            score,
            CoreMatched:      matchedCore.Count,
            CoreTotal:        archetype.CoreCards.Length,
            MatchedCoreCards: matchedCore,
            MissingCoreCards: missingCore,
            MatchedGoodCards: matchedGood,
            SuggestedNextPicks: suggestedPicks
        );
    }

    /// <summary>
    /// 카드 이름 정규화: 업그레이드(+) 표시 제거, 공백 트림.
    /// 예: "Inflame+" → "Inflame"
    /// </summary>
    public static string NormalizeCardName(string name) =>
        name.TrimEnd('+').Trim();
}
