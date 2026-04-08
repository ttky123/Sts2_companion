using System.Diagnostics;

namespace STS2Companion.Installer;

/// <summary>
/// 모드 DLL 빌드 및 게임 디렉토리 설치를 담당.
/// </summary>
public sealed class ModInstaller
{
    private readonly string _repoRoot;   // STS2CompanionMod.csproj 위치
    private readonly string _gamePath;   // STS2 게임 폴더
    private readonly string _modDir;     // <gamePath>/mods/STS2Companion

    public ModInstaller(string repoRoot, string gamePath)
    {
        _repoRoot = repoRoot;
        _gamePath = gamePath;
        _modDir   = Path.Combine(gamePath, "mods", "STS2Companion");
    }

    // ── 빌드 ──────────────────────────────────────────────────────────────

    /// <summary>dotnet build -c Release 실행 후 성공 여부 반환</summary>
    public bool Build()
    {
        string csproj = Path.Combine(_repoRoot, "STS2CompanionMod.csproj");
        if (!File.Exists(csproj))
        {
            UI.Error($"프로젝트 파일을 찾을 수 없음: {csproj}");
            return false;
        }

        UI.Step("모드 빌드 중...");
        int exit = RunProcess("dotnet",
            $"build \"{csproj}\" -c Release --nologo -v q");

        if (exit != 0)
        {
            UI.Error("빌드 실패. 위 오류 메시지를 확인하세요.");
            return false;
        }

        UI.Ok("빌드 완료");
        return true;
    }

    // ── 설치 ──────────────────────────────────────────────────────────────

    /// <summary>빌드된 DLL + mod.json 을 게임 mods 폴더에 복사</summary>
    public bool Install()
    {
        string dllSrc  = Path.Combine(_repoRoot, "bin", "Release", "net9.0",
                                      "STS2CompanionMod.dll");
        string jsonSrc = Path.Combine(_repoRoot, "mod.json");

        if (!File.Exists(dllSrc))
        {
            UI.Error($"DLL을 찾을 수 없음: {dllSrc}\n빌드를 먼저 실행하세요.");
            return false;
        }

        UI.Step($"설치 위치: {_modDir}");

        try
        {
            Directory.CreateDirectory(_modDir);
            File.Copy(dllSrc,  Path.Combine(_modDir, "STS2CompanionMod.dll"), overwrite: true);
            File.Copy(jsonSrc, Path.Combine(_modDir, "mod.json"),             overwrite: true);
            UI.Ok("파일 복사 완료");
            return true;
        }
        catch (Exception ex)
        {
            UI.Error($"파일 복사 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>빌드 + 설치 합쳐서 실행</summary>
    public bool Deploy() => Build() && Install();

    // ── 제거 ──────────────────────────────────────────────────────────────

    public void Uninstall()
    {
        if (!Directory.Exists(_modDir))
        {
            UI.Warn("설치된 모드를 찾을 수 없음.");
            return;
        }
        Directory.Delete(_modDir, recursive: true);
        UI.Ok($"모드 제거 완료: {_modDir}");
    }

    // ── 상태 확인 ─────────────────────────────────────────────────────────

    public bool IsInstalled()      => File.Exists(Path.Combine(_modDir, "STS2CompanionMod.dll"));
    public string? InstalledVersion()
    {
        string json = Path.Combine(_modDir, "mod.json");
        if (!File.Exists(json)) return null;
        var m = System.Text.RegularExpressions.Regex.Match(
            File.ReadAllText(json), @"""version""\s*:\s*""([^""]+)""");
        return m.Success ? m.Groups[1].Value : null;
    }

    // ── 내부 ──────────────────────────────────────────────────────────────

    private static int RunProcess(string exe, string args)
    {
        var psi = new ProcessStartInfo(exe, args)
        {
            UseShellExecute  = false,
            RedirectStandardOutput = false,
            RedirectStandardError  = false,
        };
        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException($"'{exe}' 실행 실패");
        proc.WaitForExit();
        return proc.ExitCode;
    }
}
