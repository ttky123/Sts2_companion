# Game DLL References

이 폴더에 아래 DLL을 복사하거나 `STS2GamePath` 환경변수를 설정하세요.

## 필요한 파일

| 파일 | 게임 내 경로 |
|------|-------------|
| `GodotSharp.dll` | `<게임설치경로>/GodotSharp/GodotSharp.dll` |
| `MegaCrit.Sts2.Core.dll` | `<게임설치경로>/Slay the Spire 2_Data/Managed/MegaCrit.Sts2.Core.dll` |

## 빌드 방법

```bash
# 게임 경로를 환경변수로 지정
export STS2_PATH="$HOME/.steam/steam/steamapps/common/Slay the Spire 2"
dotnet build -c Release

# 빌드 결과물 복사
cp bin/Release/net9.0/STS2CompanionMod.dll "$STS2_PATH/mods/STS2Companion/"
cp mod.json "$STS2_PATH/mods/STS2Companion/"
```

## 배포 구조

```
<게임경로>/mods/STS2Companion/
├── STS2CompanionMod.dll
└── mod.json
```
