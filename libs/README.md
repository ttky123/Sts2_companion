# Game DLL References

빌드 시 게임 DLL이 필요합니다. OS별 경로가 다릅니다.

## OS별 DLL 위치

### Linux / Windows
| 파일 | 경로 |
|------|------|
| `GodotSharp.dll` | `<게임폴더>/GodotSharp/GodotSharp.dll` |
| `MegaCrit.Sts2.Core.dll` | `<게임폴더>/Slay the Spire 2_Data/Managed/MegaCrit.Sts2.Core.dll` |

### macOS (`.app` 번들 내부)
| 파일 | 경로 |
|------|------|
| `GodotSharp.dll` | `<게임폴더>/Slay the Spire 2.app/Contents/Frameworks/GodotSharp.dll` |
| `MegaCrit.Sts2.Core.dll` | `<게임폴더>/Slay the Spire 2.app/Contents/Frameworks/MegaCrit.Sts2.Core.dll` |

## 빌드 방법

### Linux
```bash
export STS2_PATH="$HOME/.steam/steam/steamapps/common/Slay the Spire 2"
dotnet build -c Release
```

### macOS
```bash
export STS2_PATH="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2"
dotnet build -c Release
```

### Windows (PowerShell)
```powershell
$env:STS2_PATH = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
dotnet build -c Release
```

## mods 폴더 위치

| OS | 경로 |
|----|------|
| Linux / Windows | `<게임폴더>/mods/STS2Companion/` |
| macOS | `<게임폴더>/mods/STS2Companion/` (.app 번들 **밖**) |

> **macOS 주의**: `.app` 번들 내부에 직접 파일을 넣으면 Gatekeeper가
> 코드 서명을 무효화하므로, mods 폴더는 반드시 `.app` 번들 밖에 위치해야 합니다.
