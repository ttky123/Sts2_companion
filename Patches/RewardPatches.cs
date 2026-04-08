using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2CompanionMod.Overlay;

namespace STS2CompanionMod.Patches;

/// <summary>
/// 카드 보상 화면이 열리고 닫힐 때 RewardPanel을 갱신한다.
/// </summary>
[HarmonyPatch]
internal static class RewardPatches
{
    /// <summary>
    /// 카드 보상 룸이 준비될 때 제시된 카드 목록을 오버레이로 전달.
    /// NCardRewardRoom 은 카드 선택지를 보여주는 씬.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCardRewardRoom), "_Ready")]
    private static void CardRewardRoom_Ready(NCardRewardRoom __instance)
    {
        // 오버레이가 없으면 먼저 주입
        CompanionOverlay.Inject(__instance);

        // 제시된 카드 이름 수집
        var offeredCards = GetOfferedCardNames(__instance);
        if (offeredCards.Count == 0) return;

        CompanionOverlay.OnRewardScreenOpened(offeredCards);
        CompanionLogger.Log($"Reward screen: {string.Join(", ", offeredCards)}");
    }

    /// <summary>카드 보상 룸이 종료될 때 RewardPanel 숨김.</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCardRewardRoom), "_ExitTree")]
    private static void CardRewardRoom_Exit(NCardRewardRoom __instance)
    {
        CompanionOverlay.OnRewardScreenClosed();
    }

    // ── 헬퍼 ─────────────────────────────────────────────────────────────

    /// <summary>
    /// NCardRewardRoom 인스턴스에서 제시된 카드 이름 목록을 추출.
    /// 실제 필드명은 디컴파일 결과에 따라 조정 필요.
    /// </summary>
    private static List<string> GetOfferedCardNames(NCardRewardRoom room)
    {
        var names = new List<string>();
        try
        {
            // 리플렉션으로 카드 목록 필드 접근 (필드명은 실제 디컴파일 후 확인 필요)
            var field = room.GetType()
                .GetField("_rewardCards",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            if (field?.GetValue(room) is IEnumerable<CardModel> cards)
            {
                foreach (var card in cards)
                {
                    string name = card?.Id?.Entry ?? card?.ToString() ?? "Unknown";
                    names.Add(name);
                }
            }
        }
        catch (Exception ex)
        {
            CompanionLogger.Warn($"카드 목록 추출 실패: {ex.Message}");
        }
        return names;
    }
}
