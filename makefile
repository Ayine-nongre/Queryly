APP_NAME=Queryly
APP_PROJECT=src/Queryly.CLI/Queryly.CLI.csproj
FRAMEWORK=net8.0

WIN_RID=win-x64
LINUX_RID=linux-x64

PUBLISH_DIR=bin/Release/$(FRAMEWORK)

all: clean build-windows build-linux pack

clean:
	dotnet clean $(APP_PROJECT)
	rm -rf $(PUBLISH_DIR)/$(WIN_RID)
	rm -rf $(PUBLISH_DIR)/$(LINUX_RID)
	rm -f $(APP_NAME)-win.zip
	rm -f $(APP_NAME)-linux.tar.gz

build-windows:
	dotnet publish $(APP_PROJECT) -c Release \
		-r $(WIN_RID) \
		--self-contained true \
		/p:PublishSingleFile=true \
		/p:IncludeNativeLibrariesForSelfExtract=true

build-linux:
	dotnet publish $(APP_PROJECT) -c Release \
		-r $(LINUX_RID) \
		--self-contained true \
		/p:PublishSingleFile=true \
		/p:IncludeNativeLibrariesForSelfExtract=true

pack:
	# Windows zip
	cd $(PUBLISH_DIR)/$(WIN_RID)/publish && zip -r ../../../../$(APP_NAME)-win.zip .

	# Linux tar.gz
	cd $(PUBLISH_DIR)/$(LINUX_RID)/publish && tar -czf ../../../../$(APP_NAME)-linux.tar.gz .
