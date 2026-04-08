using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace STS2Companion.Installer;

/// <summary>
/// Steam 설치 경로와 libraryfolders.vdf를 파싱하여
/// STS2 게임 디렉토리를 자동으로 찾는다.
/// </summary>
public static class SteamDetector
{
    public const int  Sts2AppId    = 2868840;
    public const string Sts2FolderName = "Slay the Spire 2";

    // ── Steam 루트 후보 경로 ──────────────────────────────────────────────

    private static IEnumerable<string> GetSteamRootCandidates()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // 1. 레지스트리 (가장 신뢰할 수 있는 소스)
            string? regPath = ReadSteamRegistry();
            if (regPath is not null) yield return regPath;

            // 2. 일반 기본 경로
            yield return @"C:\Program Files (x86)\Steam";
            yield return @"C:\Program Files\Steam";

            string localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            yield return Path.Combine(localApp, "Steam");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            yield return Path.Combine(home, "Library", "Application Support", "Steam");
        }
        else
        {
            // Linux — 여러 가지 배포판별 경로
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            yield return Path.Combine(home, ".steam", "steam");
            yield return Path.Combine(home, ".steam", "Steam");
            yield return Path.Combine(home, ".local", "share", "Steam");
            yield return Path.Combine(home, "snap", "steam", "common", ".steam", "steam");
            // Flatpak Steam
            yield return Path.Combine(home, ".var", "app", "com.valvesoftware.Steam",
                                       ".steam", "steam");
        }
    }

    private static string? ReadSteamRegistry()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")
                         ?? Registry.LocalMachine.OpenSubKey(
                                @"SOFTWARE\WOW6432Node\Valve\Steam");
            return key?.GetValue("SteamPath") as string
                ?? key?.GetValue("InstallPath") as string;
        }
        catch { return null; }
    }

    // ── VDF 파싱 ─────────────────────────────────────────────────────────

    /// <summary>
    /// libraryfolders.vdf에서 모든 Steam 라이브러리 경로를 추출한다.
    /// </summary>
    public static List<string> ParseLibraryFolders(string steamRoot)
    {
        string vdfPath = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdfPath)) return new List<string>();

        string content = File.ReadAllText(vdfPath);
        var paths = new List<string> { Path.Combine(steamRoot, "steamapps") };

        // "path"  "/some/path/to/library" 패턴 매칭
        var pathMatches = Regex.Matches(content,
            @"""path""\s+""([^""]+)""", RegexOptions.IgnoreCase);

        foreach (Match m in pathMatches)
        {
            string libPath = m.Groups[1].Value.Replace(@"\\", Path.DirectorySeparatorChar.ToString());
            string steamapps = Path.Combine(libPath, "steamapps");
            if (Directory.Exists(steamapps) && !paths.Contains(steamapps))
                paths.Add(steamapps);
        }

        return paths;
    }

    // ── STS2 경로 탐색 ───────────────────────────────────────────────────

    /// <summary>
    /// 모든 Steam 라이브러리를 뒤져 STS2 게임 폴더를 찾는다.
    /// AppID manifest 우선, 없으면 폴더명으로 fallback.
    /// </summary>
    public static string? FindSts2Path()
    {
        foreach (string steamRoot in GetSteamRootCandidates())
        {
            if (!Directory.Exists(steamRoot)) continue;

            var libraries = ParseLibraryFolders(steamRoot);
            foreach (string lib in libraries)
            {
                // 방법 1: appmanifest_{AppID}.acf 파일로 정확히 찾기
                string manifest = Path.Combine(lib, $"appmanifest_{Sts2AppId}.acf");
                if (File.Exists(manifest))
                {
                    string? installDir = ParseInstallDir(File.ReadAllText(manifest));
                    if (installDir is not null)
                    {
                        string gamePath = Path.Combine(lib, "common", installDir);
                        if (Directory.Exists(gamePath)) return gamePath;
                    }
                }

                // 방법 2: 폴더 이름으로 탐색 (manifest 없을 때)
                string byName = Path.Combine(lib, "common", Sts2FolderName);
                if (Directory.Exists(byName)) return byName;
            }
        }

        return null;
    }

    /// <summary>appmanifest_*.acf에서 installdir 값 추출</summary>
    private static string? ParseInstallDir(string acfContent)
    {
        var m = Regex.Match(acfContent,
            @"""installdir""\s+""([^""]+)""", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value : null;
    }

    /// <summary>STS2 경로인지 검증 (핵심 파일 존재 여부 확인)</summary>
    public static bool IsValidSts2Path(string path) =>
        Directory.Exists(path) && (
            // Godot 기반 게임의 특징적인 파일 확인
            Directory.Exists(Path.Combine(path, "mods")) ||
            File.Exists(Path.Combine(path, "Slay the Spire 2.exe")) ||
            File.Exists(Path.Combine(path, "Slay the Spire 2.x86_64")) ||
            File.Exists(Path.Combine(path, "Slay the Spire 2.app", "Contents",
                "MacOS", "Slay the Spire 2"))
        );
}
