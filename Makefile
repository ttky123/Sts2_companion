# STS2 Companion Mod — 빌드 & 배포 Makefile

# 게임 경로 (환경변수 STS2_PATH 또는 기본 Steam 경로)
STS2_PATH ?= $(HOME)/.steam/steam/steamapps/common/Slay the Spire 2
MOD_DIR    = $(STS2_PATH)/mods/STS2Companion
DLL_OUT    = bin/Release/net9.0/STS2CompanionMod.dll

.PHONY: build install deploy clean

## 빌드만
build:
	dotnet build -c Release

## 게임 폴더에 설치
install: build
	@mkdir -p "$(MOD_DIR)"
	@cp "$(DLL_OUT)" "$(MOD_DIR)/STS2CompanionMod.dll"
	@cp mod.json "$(MOD_DIR)/mod.json"
	@echo "✓ 설치 완료: $(MOD_DIR)"

## 빌드 + 설치 (단축키)
deploy: install

## 빌드 결과물 삭제
clean:
	dotnet clean
	@rm -rf bin/ obj/

## Windows Steam 기본 경로용
install-windows:
	$(MAKE) install STS2_PATH="$(LOCALAPPDATA)/../Local/Programs/Slay the Spire 2"
