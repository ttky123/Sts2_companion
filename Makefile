# STS2 Companion Mod — 빌드 & 배포 Makefile

INSTALLER_DIR  = tools/Installer
INSTALLER_PROJ = $(INSTALLER_DIR)/Installer.csproj
DIST_DIR       = dist

.PHONY: build deploy clean publish publish-all installer

## ── 모드 빌드 ──────────────────────────────────────────────────────────────

build:
	dotnet build -c Release --nologo

## dotnet이 설치된 환경에서 직접 배포 (레거시)
deploy: build
	dotnet run --project "$(INSTALLER_PROJ)" -c Release -- deploy

## ── Standalone 인스톨러 빌드 ───────────────────────────────────────────────

## Linux x64 standalone 인스톨러
publish-linux:
	@mkdir -p $(DIST_DIR)
	dotnet publish "$(INSTALLER_PROJ)" -c Release \
	  -r linux-x64 --self-contained true \
	  -p:PublishSingleFile=true \
	  -p:EnableCompressionInSingleFile=true \
	  -o $(DIST_DIR)/linux-x64 --nologo
	@echo "✓ Linux 인스톨러: $(DIST_DIR)/linux-x64/sts2-companion-install"

## Windows x64 standalone 인스톨러
publish-windows:
	@mkdir -p $(DIST_DIR)
	dotnet publish "$(INSTALLER_PROJ)" -c Release \
	  -r win-x64 --self-contained true \
	  -p:PublishSingleFile=true \
	  -p:EnableCompressionInSingleFile=true \
	  -o $(DIST_DIR)/win-x64 --nologo
	@echo "✓ Windows 인스톨러: $(DIST_DIR)/win-x64/sts2-companion-install.exe"

## macOS ARM64 (M1/M2/M3/M4) standalone 인스톨러
publish-macos-arm:
	@mkdir -p $(DIST_DIR)/osx-arm64
	dotnet publish "$(INSTALLER_PROJ)" -c Release \
	  -r osx-arm64 --self-contained true \
	  -p:PublishSingleFile=true \
	  -p:EnableCompressionInSingleFile=true \
	  -o $(DIST_DIR)/osx-arm64 --nologo
	@echo "✓ macOS ARM64 인스톨러: $(DIST_DIR)/osx-arm64/sts2-companion-install"

## macOS Intel (x86_64) standalone 인스톨러
publish-macos-intel:
	@mkdir -p $(DIST_DIR)/osx-x64
	dotnet publish "$(INSTALLER_PROJ)" -c Release \
	  -r osx-x64 --self-contained true \
	  -p:PublishSingleFile=true \
	  -p:EnableCompressionInSingleFile=true \
	  -o $(DIST_DIR)/osx-x64 --nologo
	@echo "✓ macOS Intel 인스톨러: $(DIST_DIR)/osx-x64/sts2-companion-install"

## macOS Universal Binary (ARM64 + Intel — lipo로 합치기)
publish-macos: publish-macos-arm publish-macos-intel
	@mkdir -p $(DIST_DIR)/osx-universal
	@if command -v lipo >/dev/null 2>&1; then \
	  lipo -create \
	    $(DIST_DIR)/osx-arm64/sts2-companion-install \
	    $(DIST_DIR)/osx-x64/sts2-companion-install \
	    -output $(DIST_DIR)/osx-universal/sts2-companion-install; \
	  chmod +x $(DIST_DIR)/osx-universal/sts2-companion-install; \
	  echo "✓ macOS Universal 인스톨러: $(DIST_DIR)/osx-universal/sts2-companion-install"; \
	else \
	  echo "  lipo 없음 (비macOS 빌드 환경) — ARM64/Intel 개별 파일만 생성됨"; \
	fi

## 세 플랫폼 모두 빌드
publish-all: publish-linux publish-windows publish-macos
	@echo ""
	@echo "✓ 전체 빌드 완료:"
	@ls -lh $(DIST_DIR)/*/sts2-companion-install* 2>/dev/null || true

## ── 청소 ──────────────────────────────────────────────────────────────────

clean:
	dotnet clean --nologo
	@rm -rf bin/ obj/ $(DIST_DIR)/
	@find $(INSTALLER_DIR) -name bin -o -name obj | xargs rm -rf 2>/dev/null; true
