using System.Reflection;
using HarmonyLib;
using STS2CompanionMod.Overlay;
using STS2CompanionMod.Services;

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
/// 온라인 데이터:
///   - QuestceSpire API: 커뮤니티 카드/렐릭 승률 (6시간 캐시)
///   - SpireCodex API: 카드/렐릭 메타데이터
///   - 캐시 위치: %APPDATA%/Godot/.../sts2_companion_tier_cache.json
///
/// 인게임 단축키:
///   F7 — 오버레이 표시/숨기기
/// </summary>
[ModInitializer("ModLoaded")]
public static class ModEntry
{
    public static Harmony           Instance    { get; private set; } = null!;
    public static OnlineDataService DataService { get; private set; } = null!;

    public static void ModLoaded()
    {
        try
        {
            // 1. Harmony 패치 등록
            Instance = new Harmony("sts2.companion");
            Instance.PatchAll(Assembly.GetExecutingAssembly());

            // 2. 온라인 데이터 서비스 초기화 (비동기, 게임 로딩 차단 안 함)
            DataService = new OnlineDataService();
            CompanionOverlay.SetDataService(DataService);

            // 캐시 확인 + 필요 시 네트워크 요청을 백그라운드에서 실행
            _ = DataService.InitializeAsync();

            CompanionLogger.Log("=========================================");
            CompanionLogger.Log("  STS2 Companion Mod v1.0 로드 완료");
            CompanionLogger.Log("  온라인 데이터 로딩 중 (백그라운드)...");
            CompanionLogger.Log("  F7 — 오버레이 토글");
            CompanionLogger.Log("=========================================");
        }
        catch (Exception ex)
        {
            CompanionLogger.Error($"모드 초기화 실패: {ex}");
        }
    }
}
