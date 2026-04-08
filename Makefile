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

## macOS ARM64 standalone 인스톨러
publish-macos:
	@mkdir -p $(DIST_DIR)
	dotnet publish "$(INSTALLER_PROJ)" -c Release \
	  -r osx-arm64 --self-contained true \
	  -p:PublishSingleFile=true \
	  -p:EnableCompressionInSingleFile=true \
	  -o $(DIST_DIR)/osx-arm64 --nologo
	@echo "✓ macOS 인스톨러: $(DIST_DIR)/osx-arm64/sts2-companion-install"

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
