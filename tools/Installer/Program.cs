using System.Runtime.InteropServices;
using STS2Companion.Installer;

// ── 헤더 ──────────────────────────────────────────────────────────────────
UI.Banner();

// ── 1. 저장소 루트 (STS2CompanionMod.csproj 위치) 찾기 ───────────────────
string repoRoot = FindRepoRoot();
UI.Info($"저장소 경로: {repoRoot}");

// ── 2. STS2 게임 경로 찾기 ─────────────────────────────────────────────
string? gamePath = ResolveGamePath(args);

if (gamePath is null)
{
    UI.Error("STS2 게임 경로를 찾지 못했습니다.");
    UI.Info("직접 경로를 지정하려면:");
    UI.Info("  sts2-companion-install --path \"/path/to/Slay the Spire 2\"");
    Environment.Exit(1);
}

UI.Ok($"게임 경로: {gamePath}");

// ── 3. 명령 파싱 ──────────────────────────────────────────────────────────
string command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "deploy";

var installer = new ModInstaller(repoRoot, gamePath);

switch (command)
{
    case "deploy":
    case "install":
        ShowCurrentStatus(installer);
        UI.Separator();

        bool ok = installer.Deploy();
        UI.Separator();
        if (ok)
        {
            UI.Ok("=== 설치 완료 ===");
            UI.Info("게임을 실행하고 메인 메뉴 → Mods → STS2 Companion 을 활성화하세요.");
        }
        else
        {
            UI.Error("설치 실패");
            Environment.Exit(1);
        }
        break;

    case "uninstall":
    case "remove":
        installer.Uninstall();
        break;

    case "status":
        ShowCurrentStatus(installer);
        break;

    case "detect":
        // 경로 감지만 출력
        Console.WriteLine(gamePath);
        break;

    default:
        UI.Error($"알 수 없는 명령: {command}");
        PrintUsage();
        Environment.Exit(1);
        break;
}

// ── 헬퍼 함수들 ───────────────────────────────────────────────────────────

static string FindRepoRoot()
{
    // 실행 파일 위치에서 상위로 올라가며 STS2CompanionMod.csproj 탐색
    string? dir = AppContext.BaseDirectory;
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir, "STS2CompanionMod.csproj")))
            return dir;
        dir = Path.GetDirectoryName(dir);
    }

    // 현재 디렉토리에서도 탐색
    dir = Directory.GetCurrentDirectory();
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir, "STS2CompanionMod.csproj")))
            return dir;
        dir = Path.GetDirectoryName(dir);
    }

    UI.Error("STS2CompanionMod.csproj를 찾을 수 없습니다. 저장소 루트에서 실행하세요.");
    Environment.Exit(1);
    return "";
}

static string? ResolveGamePath(string[] args)
{
    // --path 인수 우선
    int pathIdx = Array.IndexOf(args, "--path");
    if (pathIdx >= 0 && pathIdx + 1 < args.Length)
    {
        string manual = args[pathIdx + 1];
        if (SteamDetector.IsValidSts2Path(manual))
            return manual;
        UI.Error($"지정한 경로가 유효하지 않음: {manual}");
        return null;
    }

    // STS2_PATH 환경변수
    string? envPath = Environment.GetEnvironmentVariable("STS2_PATH");
    if (envPath is not null && SteamDetector.IsValidSts2Path(envPath))
    {
        UI.Info("환경변수 STS2_PATH 사용");
        return envPath;
    }

    // Steam 자동 감지
    UI.Step("Steam 라이브러리에서 STS2 탐색 중...");
    string? detected = SteamDetector.FindSts2Path();
    if (detected is not null)
        UI.Ok("자동 감지 성공");

    return detected;
}

static void ShowCurrentStatus(ModInstaller installer)
{
    if (installer.IsInstalled())
    {
        string ver = installer.InstalledVersion() ?? "알 수 없음";
        UI.Info($"현재 설치 버전: {ver}");
    }
    else
    {
        UI.Info("현재 설치 없음");
    }
}

static void PrintUsage()
{
    Console.WriteLine();
    Console.WriteLine("사용법: sts2-companion-install [명령] [옵션]");
    Console.WriteLine();
    Console.WriteLine("명령:");
    Console.WriteLine("  deploy      빌드 + 설치 (기본값)");
    Console.WriteLine("  uninstall   모드 제거");
    Console.WriteLine("  status      설치 상태 확인");
    Console.WriteLine("  detect      게임 경로만 출력");
    Console.WriteLine();
    Console.WriteLine("옵션:");
    Console.WriteLine("  --path <경로>   STS2 게임 경로 직접 지정");
    Console.WriteLine();
    Console.WriteLine("예시:");
    Console.WriteLine("  sts2-companion-install");
    Console.WriteLine("  sts2-companion-install --path \"C:\\Steam\\steamapps\\common\\Slay the Spire 2\"");
    Console.WriteLine("  sts2-companion-install uninstall");
}

// ── UI 헬퍼 ───────────────────────────────────────────────────────────────
static class UI
{
    private static readonly bool _noColor =
        Environment.GetEnvironmentVariable("NO_COLOR") is not null ||
        Console.IsOutputRedirected;

    public static void Banner()
    {
        SetColor(ConsoleColor.Cyan);
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║   STS2 Companion Mod Installer v1.0  ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Reset();
        Console.WriteLine();
    }

    public static void Step(string msg)  { SetColor(ConsoleColor.Yellow);  Console.WriteLine($"  ▶ {msg}"); Reset(); }
    public static void Ok(string msg)    { SetColor(ConsoleColor.Green);   Console.WriteLine($"  ✓ {msg}"); Reset(); }
    public static void Info(string msg)  {                                   Console.WriteLine($"    {msg}");         }
    public static void Warn(string msg)  { SetColor(ConsoleColor.Yellow);  Console.WriteLine($"  ! {msg}"); Reset(); }
    public static void Error(string msg) { SetColor(ConsoleColor.Red);     Console.WriteLine($"  ✗ {msg}"); Reset(); }
    public static void Separator()       { Console.WriteLine("  " + new string('─', 38)); }

    private static void SetColor(ConsoleColor c) { if (!_noColor) Console.ForegroundColor = c; }
    private static void Reset()                  { if (!_noColor) Console.ResetColor(); }
}
