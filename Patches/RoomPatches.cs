using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2CompanionMod.Overlay;

namespace STS2CompanionMod.Patches;

/// <summary>
/// 각 룸 씬의 _Ready()에 패치를 걸어 오버레이를 주입한다.
/// NGame._Ready에 주입하면 모든 씬에서 지속되지만,
/// 룸별로 주입하면 해당 룸에서만 활성화되어 더 안전하다.
/// </summary>
[HarmonyPatch]
internal static class RoomPatches
{
    /// <summary>전투 씬 — 핵심 분석이 가장 필요한 곳</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCombatRoom), "_Ready")]
    private static void CombatRoom_Ready(NCombatRoom __instance)
    {
        CompanionOverlay.Inject(__instance);
        CompanionLogger.Log("Overlay injected into CombatRoom.");
    }

    /// <summary>지도 씬 — 다음 룸 선택 시 덱 분석 표시</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NMapRoom), "_Ready")]
    private static void MapRoom_Ready(NMapRoom __instance)
    {
        CompanionOverlay.Inject(__instance);
    }

    /// <summary>이벤트 씬 — 이벤트 선택 전 빌드 방향 확인</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NEventRoom), "_Ready")]
    private static void EventRoom_Ready(NEventRoom __instance)
    {
        CompanionOverlay.Inject(__instance);
    }

    /// <summary>상점 씬 — 구매 전 시너지 확인</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NMerchantRoom), "_Ready")]
    private static void MerchantRoom_Ready(NMerchantRoom __instance)
    {
        CompanionOverlay.Inject(__instance);
    }

    /// <summary>휴식 씬 — 업그레이드 우선순위 참고용</summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NRestSiteRoom), "_Ready")]
    private static void RestSiteRoom_Ready(NRestSiteRoom __instance)
    {
        CompanionOverlay.Inject(__instance);
    }
}
