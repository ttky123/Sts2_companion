using Godot;
using MegaCrit.Sts2.Core.Runs;
using STS2CompanionMod.Analysis;

namespace STS2CompanionMod.Overlay;

/// <summary>
/// 오버레이의 루트 컨테이너.
/// 게임 씬 어디서든 Inject()를 호출하면 화면 우측에 패널이 부착됨.
/// F7 키로 표시/숨기기 가능.
/// </summary>
public static class CompanionOverlay
{
    public static Control?  Root       { get; private set; }
    public static bool      IsVisible  { get; private set; } = true;

    private static DeckPanel?   _deckPanel;
    private static RewardPanel? _rewardPanel;

    // 마지막 분석 결과 캐싱 (매 프레임 재계산 방지)
    private static List<ArchetypeMatchResult> _lastAnalysis = new();
    private static int    _lastDeckHash     = 0;
    private static int    _lastAscension    = -1;
    private static string _lastCharacterId  = "";

    // 분석 쿨다운 (초)
    private const double AnalysisIntervalSec = 2.0;
    private static double _timeSinceLastAnalysis = 0;

    /// <summary>지정된 부모 노드에 오버레이를 주입한다.</summary>
    public static void Inject(Node parent)
    {
        if (Root is not null) return; // 이미 주입됨

        Root      = new Control();
        Root.Name = "STS2CompanionOverlay";

        // 화면 우측에 고정
        Root.SetAnchorsPreset(Control.LayoutPreset.RightWide);
        Root.OffsetLeft  = -290f;
        Root.OffsetRight = -8f;
        Root.OffsetTop   = 8f;

        var vbox = new VBoxContainer();
        vbox.Name = "PanelContainer";
        vbox.AddThemeConstantOverride("separation", 8);
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        Root.AddChild(vbox);

        _deckPanel   = new DeckPanel();
        _rewardPanel = new RewardPanel();

        vbox.AddChild(_deckPanel.Root);
        vbox.AddChild(_rewardPanel.Root);

        parent.AddChild(Root);

        // ProcessFrame 시그널로 매 프레임 업데이트 (virtual _Process 대신)
        Root.GetTree().ProcessFrame += OnProcessFrame;
        Root.TreeExiting += OnTreeExiting;

        CompanionLogger.Log("CompanionOverlay injected.");
    }

    /// <summary>오버레이 표시/숨기기 토글 (F7)</summary>
    public static void Toggle()
    {
        IsVisible      = !IsVisible;
        if (Root is not null) Root.Visible = IsVisible;
    }

    /// <summary>카드 보상 화면이 열렸을 때 호출</summary>
    public static void OnRewardScreenOpened(IEnumerable<string> offeredCards)
    {
        if (_rewardPanel is null || _deckPanel is null) return;

        var state       = TryGetRunState();
        int ascension   = state?.Players[0]?.AscensionLevel ?? 0;
        string charId   = state?.Players[0]?.Character?.Id?.Entry ?? "";

        var scores = RewardScorer.Score(
            offeredCards, _lastAnalysis, charId, ascension);

        _rewardPanel.ShowRewards(scores, ascension, charId);
    }

    /// <summary>카드 보상 화면이 닫혔을 때 호출</summary>
    public static void OnRewardScreenClosed() => _rewardPanel?.Hide();

    // ── 내부 ──────────────────────────────────────────────────────────────

    private static void OnProcessFrame()
    {
        if (!IsVisible || Root is null) return;

        _timeSinceLastAnalysis += Root.GetProcessDeltaTime();
        if (_timeSinceLastAnalysis < AnalysisIntervalSec) return;
        _timeSinceLastAnalysis = 0;

        var state = TryGetRunState();
        if (state is null) return;

        var player     = state.Players.Count > 0 ? state.Players[0] : null;
        if (player is null) return;

        int  ascension = player.AscensionLevel;
        string charId  = player.Character?.Id?.Entry ?? "";
        var deck       = player.DrawPile ?? Enumerable.Empty<object>();

        // 덱 변화 감지용 해시 (카드 이름 목록의 XOR 해시)
        var cardNames  = deck.Select(c => c.ToString() ?? "").OrderBy(x => x).ToList();
        int deckHash   = cardNames.Aggregate(0, (h, n) => h ^ n.GetHashCode());

        if (deckHash == _lastDeckHash && ascension == _lastAscension && charId == _lastCharacterId)
            return; // 변화 없음

        _lastDeckHash     = deckHash;
        _lastAscension    = ascension;
        _lastCharacterId  = charId;

        _lastAnalysis = DeckAnalyzer.Analyze(cardNames, charId, ascension);
        _deckPanel?.Update(_lastAnalysis, ascension, charId);
    }

    private static void OnTreeExiting()
    {
        if (Root is not null)
        {
            Root.GetTree().ProcessFrame -= OnProcessFrame;
            Root.TreeExiting -= OnTreeExiting;
        }
        Root        = null;
        _deckPanel  = null;
        _rewardPanel = null;
    }

    private static RunState? TryGetRunState()
    {
        try { return RunManager.Instance?.DebugOnlyGetState(); }
        catch { return null; }
    }
}

/// <summary>간단한 로거 래퍼</summary>
public static class CompanionLogger
{
    public static void Log(string msg)   => GD.Print($"[STS2Companion] {msg}");
    public static void Warn(string msg)  => GD.PrintErr($"[STS2Companion][WARN] {msg}");
    public static void Error(string msg) => GD.PrintErr($"[STS2Companion][ERR] {msg}");
}
