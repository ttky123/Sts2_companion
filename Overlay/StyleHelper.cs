using Godot;

namespace STS2CompanionMod.Overlay;

/// <summary>
/// Godot StyleBox / 색상 팩토리 헬퍼 — 오버레이 전체 테마 일관성 유지.
/// </summary>
internal static class StyleHelper
{
    // 팔레트
    public static readonly Color BgPanel      = new(0.08f, 0.07f, 0.12f, 0.92f);
    public static readonly Color BgCard       = new(0.13f, 0.11f, 0.18f, 0.95f);
    public static readonly Color BorderNormal = new(0.25f, 0.22f, 0.38f, 1f);
    public static readonly Color TextPrimary  = new(0.93f, 0.91f, 0.96f, 1f);
    public static readonly Color TextMuted    = new(0.56f, 0.55f, 0.68f, 1f);
    public static readonly Color Gold         = new(0.94f, 0.76f, 0.25f, 1f);

    // 등급별 색상
    public static readonly Color TierCore   = new(0.98f, 0.40f, 0.40f, 1f);  // 빨강
    public static readonly Color TierGood   = new(0.40f, 0.85f, 0.45f, 1f);  // 초록
    public static readonly Color TierNormal = new(0.70f, 0.70f, 0.75f, 1f);  // 회색
    public static readonly Color TierSkip   = new(0.45f, 0.45f, 0.50f, 1f);  // 어두운 회색

    // 아키타입 매칭 점수 바 색상
    public static readonly Color BarHigh   = new(0.25f, 0.85f, 0.45f, 1f);
    public static readonly Color BarMid    = new(0.95f, 0.80f, 0.20f, 1f);
    public static readonly Color BarLow    = new(0.85f, 0.38f, 0.25f, 1f);

    public static Color ForTierLabel(string tier) => tier switch
    {
        "핵심"   => TierCore,
        "좋음"   => TierGood,
        "보통"   => TierNormal,
        "건너뜀" => TierSkip,
        _        => TierNormal,
    };

    public static Color ForMatchScore(float score) => score switch
    {
        >= 0.65f => BarHigh,
        >= 0.35f => BarMid,
        _        => BarLow,
    };

    /// <summary>패널 배경용 StyleBoxFlat 생성</summary>
    public static StyleBoxFlat MakePanelStyle(
        Color? bg = null,
        Color? border = null,
        float cornerRadius = 6f)
    {
        var style = new StyleBoxFlat();
        style.BgColor            = bg     ?? BgPanel;
        style.BorderColor        = border ?? BorderNormal;
        style.SetBorderWidthAll(1);
        style.SetCornerRadiusAll((int)cornerRadius);
        style.ContentMarginLeft  = 10;
        style.ContentMarginRight = 10;
        style.ContentMarginTop   = 8;
        style.ContentMarginBottom = 8;
        return style;
    }

    /// <summary>색깔 있는 진행 바 StyleBoxFlat 생성</summary>
    public static StyleBoxFlat MakeBarStyle(Color color)
    {
        var style = new StyleBoxFlat();
        style.BgColor = color;
        style.SetCornerRadiusAll(3);
        return style;
    }

    /// <summary>Label에 색상과 폰트 크기를 설정하는 유틸</summary>
    public static void SetLabelStyle(Label label, Color color, int fontSize = 14, bool bold = false)
    {
        label.AddThemeColorOverride("font_color", color);
        label.AddThemeFontSizeOverride("font_size", fontSize);
    }
}
