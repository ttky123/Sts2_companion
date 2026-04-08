namespace STS2CompanionMod.Analysis;

public enum Difficulty { Easy, Medium, Hard }

/// <summary>아키타입 정의 데이터 (불변 레코드)</summary>
public record ArchetypeData(
    string Id,
    string CharacterId,
    string Name,
    string NameEn,
    string Description,
    string WinCondition,
    Difficulty Difficulty,
    string[] CoreCards,
    string[] GoodCards,
    string[] AvoidCards,
    string[] KeyRelics,
    AscensionViability AscensionViability,
    AscensionNotes AscensionNotes
);

public record AscensionViability(int Low, int Mid, int High)
{
    /// <summary>현재 승천 단계의 실효 점수 반환 (1~5)</summary>
    public int GetFor(int ascensionLevel) => ascensionLevel switch
    {
        <= 7  => Low,
        <= 14 => Mid,
        _     => High,
    };
}

public record AscensionNotes(string Low, string Mid, string High)
{
    public string GetFor(int ascensionLevel) => ascensionLevel switch
    {
        <= 7  => Low,
        <= 14 => Mid,
        _     => High,
    };
}

/// <summary>덱 분석 결과 — 아키타입 하나와의 매칭 점수</summary>
public record ArchetypeMatchResult(
    ArchetypeData Archetype,
    float Score,                    // 0.0 ~ 1.0
    int CoreMatched,
    int CoreTotal,
    List<string> MatchedCoreCards,
    List<string> MissingCoreCards,
    List<string> MatchedGoodCards,
    List<string> SuggestedNextPicks // 덱 완성을 위해 다음에 가져가야 할 카드
)
{
    public string ScoreLabel => Score switch
    {
        >= 0.75f => "★★★ 강력",
        >= 0.50f => "★★☆ 진행 중",
        >= 0.25f => "★☆☆ 초기",
        _        => "☆☆☆ 미약",
    };
}

/// <summary>카드 보상 선택지 하나의 평가 결과</summary>
public record RewardCardScore(
    string CardName,
    float Score,         // 0.0 ~ 1.0
    string Tier,         // "핵심", "좋음", "보통", "건너뜀"
    string Reason,
    string? ForArchetype // 어느 아키타입을 위한 픽인지
)
{
    public static string TierFromScore(float score) => score switch
    {
        >= 0.85f => "핵심",
        >= 0.60f => "좋음",
        >= 0.35f => "보통",
        _        => "건너뜀",
    };
}
