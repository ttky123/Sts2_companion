using Godot;
using STS2CompanionMod.Analysis;
using STS2CompanionMod.Data;

namespace STS2CompanionMod.Overlay;

/// <summary>
/// 카드 보상 선택지 화면에서 각 카드의 추천 등급을 표시하는 패널.
/// 카드 보상 UI가 활성화될 때만 표시됨.
/// </summary>
internal sealed class RewardPanel
{
    public Control Root { get; }

    private readonly VBoxContainer _cardList;
    private readonly Label         _titleLabel;
    private readonly Label         _ascensionTipLabel;

    public RewardPanel()
    {
        Root = new Control();
        Root.Name    = "RewardPanel";
        Root.Visible = false; // 보상 화면 열릴 때만 표시
        Root.CustomMinimumSize = new Vector2(260, 0);

        var bg = new PanelContainer();
        bg.AddThemeStyleboxOverride("panel", StyleHelper.MakePanelStyle());
        bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        Root.AddChild(bg);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 8);
        bg.AddChild(vbox);

        _titleLabel = new Label { Text = "보상 카드 평가" };
        StyleHelper.SetLabelStyle(_titleLabel, StyleHelper.Gold, 15, bold: true);
        vbox.AddChild(_titleLabel);

        vbox.AddChild(new HSeparator());

        _cardList = new VBoxContainer();
        _cardList.AddThemeConstantOverride("separation", 5);
        vbox.AddChild(_cardList);

        vbox.AddChild(new HSeparator());

        _ascensionTipLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            Text         = "",
        };
        StyleHelper.SetLabelStyle(_ascensionTipLabel, StyleHelper.TextMuted, 11);
        vbox.AddChild(_ascensionTipLabel);
    }

    /// <summary>보상 카드 평가 결과로 패널 갱신.</summary>
    public void ShowRewards(
        List<RewardCardScore> scores,
        int ascensionLevel,
        string characterId)
    {
        Root.Visible = true;

        foreach (var child in _cardList.GetChildren())
            child.QueueFree();

        foreach (var score in scores)
            _cardList.AddChild(BuildCardRow(score));

        // 승천 가이드 한 줄 팁
        var tip = AscensionGuide.GetTip(ascensionLevel);
        _ascensionTipLabel.Text = $"A{ascensionLevel} 픽 철학: {tip.CardPickPhilosophy}";
    }

    public void Hide() => Root.Visible = false;

    private static Control BuildCardRow(RewardCardScore score)
    {
        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 6);

        // 등급 배지
        var badge = new PanelContainer();
        badge.CustomMinimumSize = new Vector2(52, 0);
        badge.AddThemeStyleboxOverride("panel",
            StyleHelper.MakePanelStyle(
                bg: StyleHelper.ForTierLabel(score.Tier) with { A = 0.18f },
                border: StyleHelper.ForTierLabel(score.Tier),
                cornerRadius: 3f));

        var badgeLabel = new Label { Text = score.Tier };
        StyleHelper.SetLabelStyle(badgeLabel, StyleHelper.ForTierLabel(score.Tier), 12);
        badgeLabel.HorizontalAlignment = HorizontalAlignment.Center;
        badge.AddChild(badgeLabel);
        hbox.AddChild(badge);

        // 카드 이름 + 이유
        var textVBox = new VBoxContainer();
        textVBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        hbox.AddChild(textVBox);

        var nameLabel = new Label { Text = score.CardName };
        StyleHelper.SetLabelStyle(nameLabel, StyleHelper.TextPrimary, 13);
        textVBox.AddChild(nameLabel);

        var reasonLabel = new Label
        {
            Text         = score.Reason,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
        };
        StyleHelper.SetLabelStyle(reasonLabel, StyleHelper.TextMuted, 11);
        textVBox.AddChild(reasonLabel);

        return hbox;
    }
}
