namespace STS2CompanionMod.Services;

/// <summary>
/// 커뮤니티 승률(WinRateDelta) → OnlineTier 변환 규칙.
///
/// WinRateDelta = WinRateWhenPicked - WinRateWhenSkipped
/// 이 수치가 크다 = 이 카드/렐릭이 있을 때 유의미하게 승률이 높다.
/// </summary>
public static class TierMapper
{
    // 카드: 델타 기준 티어링 (표본 30 이상)
    private static readonly (double minDelta, OnlineTier tier)[] CardThresholds =
    {
        ( 0.18, OnlineTier.S),
        ( 0.10, OnlineTier.A),
        ( 0.03, OnlineTier.B),
        (-0.03, OnlineTier.C),
        (double.MinValue, OnlineTier.D),
    };

    // 렐릭: 카드보다 기준 완화 (렐릭은 선택지가 적어 델타가 낮게 나오는 경향)
    private static readonly (double minDelta, OnlineTier tier)[] RelicThresholds =
    {
        ( 0.14, OnlineTier.S),
        ( 0.07, OnlineTier.A),
        ( 0.01, OnlineTier.B),
        (-0.05, OnlineTier.C),
        (double.MinValue, OnlineTier.D),
    };

    // 표본이 너무 작으면 B로 처리 (데이터 신뢰도 부족)
    private const int MinSampleSize = 30;

    public static OnlineTier ForCard(double delta, int sampleSize)
    {
        if (sampleSize < MinSampleSize) return OnlineTier.B;
        return MatchThreshold(delta, CardThresholds);
    }

    public static OnlineTier ForRelic(double delta, int sampleSize)
    {
        if (sampleSize < MinSampleSize) return OnlineTier.B;
        return MatchThreshold(delta, RelicThresholds);
    }

    /// <summary>OnlineTier → RelicTier (CardDatabase 호환) 변환</summary>
    public static Analysis.RelicTier ToRelicTier(OnlineTier tier) => tier switch
    {
        OnlineTier.S => Analysis.RelicTier.S,
        OnlineTier.A => Analysis.RelicTier.A,
        OnlineTier.B => Analysis.RelicTier.B,
        _            => Analysis.RelicTier.C,
    };

    /// <summary>승률 퍼센트 표시 문자열 (예: "+12.3%")</summary>
    public static string FormatDelta(double delta) =>
        delta >= 0 ? $"+{delta:P1}" : $"{delta:P1}";

    private static OnlineTier MatchThreshold(
        double delta, (double minDelta, OnlineTier tier)[] thresholds)
    {
        foreach (var (minDelta, tier) in thresholds)
            if (delta >= minDelta) return tier;
        return OnlineTier.D;
    }
}
