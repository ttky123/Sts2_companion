using System.Reflection;
using HarmonyLib;
using STS2CompanionMod.Overlay;

namespace STS2CompanionMod;

/// <summary>
/// STS2 Companion Mod — 진입점.
///
/// 게임이 모드를 로드할 때 [ModInitializer] 어트리뷰트가 붙은
/// ModLoaded() 메서드를 자동으로 호출한다.
///
/// 빌드 및 배포:
///   dotnet build -c Release
///   결과물 STS2CompanionMod.dll 과 mod.json 을
///   <게임경로>/mods/STS2Companion/ 에 복사.
///
/// 인게임 단축키:
///   F7 — 오버레이 표시/숨기기
/// </summary>
[ModInitializer("ModLoaded")]
public static class ModEntry
{
    public static Harmony Instance { get; private set; } = null!;

    public static void ModLoaded()
    {
        try
        {
            Instance = new Harmony("sts2.companion");
            Instance.PatchAll(Assembly.GetExecutingAssembly());

            CompanionLogger.Log("=========================================");
            CompanionLogger.Log("  STS2 Companion Mod v1.0 로드 완료");
            CompanionLogger.Log("  F7 — 오버레이 토글");
            CompanionLogger.Log("=========================================");
        }
        catch (Exception ex)
        {
            CompanionLogger.Error($"모드 초기화 실패: {ex}");
        }
    }
}
