using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using STS2CompanionMod.Overlay;

namespace STS2CompanionMod.Patches;

/// <summary>
/// 전역 키 입력 처리 패치.
/// NGame._Input() 에 후킹하여 F7 토글을 구현한다.
/// </summary>
[HarmonyPatch]
internal static class InputPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NGame), "_Input")]
    private static void NGame_Input(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventKey keyEvent) return;
        if (!keyEvent.Pressed || keyEvent.Echo) return;

        switch (keyEvent.Keycode)
        {
            case Key.F7:
                CompanionOverlay.Toggle();
                CompanionLogger.Log($"Overlay toggled: {(CompanionOverlay.IsVisible ? "ON" : "OFF")}");
                break;
        }
    }
}
