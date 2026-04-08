using Godot;
using STS2CompanionMod.Analysis;

namespace STS2CompanionMod.Overlay;

/// <summary>
/// 현재 덱의 아키타입 매칭 결과를 표시하는 패널.
/// 상위 3개 아키타입을 점수 바와 함께 보여주고, 추천 픽 카드를 표시.
/// </summary>
internal sealed class DeckPanel
{
    public Control Root { get; }

    private readonly VBoxContainer   _archetypeList;
    private readonly Label           _titleLabel;
    private readonly Label           _ascensionLabel;
    private readonly Label           _noDataLabel;

    // 표시할 최대 아키타입 수
    private const int MaxArchetypes = 3;

    public DeckPanel()
    {
        Root = new Control();
        Root.Name = "DeckPanel";
        Root.CustomMinimumSize = new Vector2(280, 0);

        var bg = new PanelContainer();
        bg.AddThemeStyleboxOverride("panel", StyleHelper.MakePanelStyle());
        bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        Root.AddChild(bg);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 8);
        bg.AddChild(vbox);

        // 헤더
        var header = new HBoxContainer();
        vbox.AddChild(header);

        _titleLabel = new Label { Text = "덱 아키타입 분석" };
        StyleHelper.SetLabelStyle(_titleLabel, StyleHelper.Gold, 15, bold: true);
        header.AddChild(_titleLabel);

        _ascensionLabel = new Label { Text = "A0" };
        StyleHelper.SetLabelStyle(_ascensionLabel, StyleHelper.TextMuted, 13);
        _ascensionLabel.HorizontalAlignment = HorizontalAlignment.Right;
        _ascensionLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        header.AddChild(_ascensionLabel);

        var separator = new HSeparator();
        vbox.AddChild(separator);

        // 아키타입 목록
        _archetypeList = new VBoxContainer();
        _archetypeList.AddThemeConstantOverride("separation", 6);
        vbox.AddChild(_archetypeList);

        _noDataLabel = new Label { Text = "덱 카드가 없습니다." };
        StyleHelper.SetLabelStyle(_noDataLabel, StyleHelper.TextMuted, 13);
        _noDataLabel.Visible = false;
        vbox.AddChild(_noDataLabel);
    }

    /// <summary>
    /// 분석 결과로 패널 내용 갱신.
    /// </summary>
    public void Update(List<ArchetypeMatchResult> results, int ascensionLevel, string characterName)
    {
        _ascensionLabel.Text = $"{characterName} · A{ascensionLevel}";

        // 기존 아키타입 행 모두 제거
        foreach (var child in _archetypeList.GetChildren())
            child.QueueFree();

        if (results.Count == 0 || results.All(r => r.Score < 0.01f))
        {
            _noDataLabel.Visible = true;
            return;
        }

        _noDataLabel.Visible = false;

        int count = Math.Min(results.Count, MaxArchetypes);
        for (int i = 0; i < count; i++)
            _archetypeList.AddChild(BuildArchetypeRow(results[i], i == 0));
    }

    private Control BuildArchetypeRow(ArchetypeMatchResult result, bool isTop)
    {
        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel",
            StyleHelper.MakePanelStyle(StyleHelper.BgCard, cornerRadius: 4f));

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 4);
        panel.AddChild(vbox);

        // 아키타입 이름 + 점수 라벨
        var headerRow = new HBoxContainer();
        vbox.AddChild(headerRow);

        var nameLabel = new Label { Text = isTop ? $"▶ {result.Archetype.Name}" : result.Archetype.Name };
        StyleHelper.SetLabelStyle(nameLabel,
            isTop ? StyleHelper.Gold : StyleHelper.TextPrimary, 14);
        nameLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        headerRow.AddChild(nameLabel);

        var scoreLabel = new Label
        {
            Text = $"{result.Score:P0}  {result.ScoreLabel}"
        };
        StyleHelper.SetLabelStyle(scoreLabel,
            StyleHelper.ForMatchScore(result.Score), 12);
        headerRow.AddChild(scoreLabel);

        // 점수 진행 바
        var barBg = new PanelContainer();
        barBg.CustomMinimumSize = new Vector2(0, 6);
        barBg.AddThemeStyleboxOverride("panel",
            StyleHelper.MakePanelStyle(new Color(0.2f, 0.2f, 0.25f), cornerRadius: 3f));
        vbox.AddChild(barBg);

        var barFill = new Control();
        barFill.CustomMinimumSize = new Vector2(
            Math.Clamp(result.Score, 0f, 1f) * 260f, 6f);
        barFill.AddThemeStyleboxOverride("panel",
            StyleHelper.MakeBarStyle(StyleHelper.ForMatchScore(result.Score)));
        barBg.AddChild(barFill);

        // 핵심 카드 현황: "코어 2/5"
        var coreRow = new HBoxContainer();
        vbox.AddChild(coreRow);

        var coreLabel = new Label
        {
            Text = $"코어 {result.CoreMatched}/{result.CoreTotal}  "
        };
        StyleHelper.SetLabelStyle(coreLabel, StyleHelper.TextMuted, 12);
        coreRow.AddChild(coreLabel);

        // 확보된 코어 카드 (녹색)
        foreach (var card in result.MatchedCoreCards.Take(3))
        {
            var cl = new Label { Text = $"[{card}] " };
            StyleHelper.SetLabelStyle(cl, StyleHelper.TierGood, 11);
            coreRow.AddChild(cl);
        }

        // 다음 추천 픽
        if (result.SuggestedNextPicks.Count > 0)
        {
            var pickRow = new HBoxContainer();
            vbox.AddChild(pickRow);

            var pickHint = new Label { Text = "다음 픽: " };
            StyleHelper.SetLabelStyle(pickHint, StyleHelper.TextMuted, 11);
            pickRow.AddChild(pickHint);

            foreach (var card in result.SuggestedNextPicks)
            {
                var cl = new Label { Text = $"{card}  " };
                StyleHelper.SetLabelStyle(cl, StyleHelper.TierCore, 11);
                pickRow.AddChild(cl);
            }
        }

        // 승천 단계별 노트 (최상위 아키타입만 표시)
        if (isTop)
        {
            var noteLabel = new Label
            {
                Text       = result.Archetype.AscensionNotes.GetFor(0), // ascension passed via Update
                AutowrapMode = TextServer.AutowrapMode.WordSmart,
            };
            // 실제 ascension은 Update에서 전달되어야 하므로 임시로 Low 노트
            StyleHelper.SetLabelStyle(noteLabel, StyleHelper.TextMuted, 11);
            vbox.AddChild(noteLabel);
        }

        return panel;
    }
}
