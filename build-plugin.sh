#!/bin/bash
set -e

cd ./src/plugins/StringTemplates.Plugins.Configuration
dotnet clean
dotnet build -c Release
dotnet pack -c Release

# To run:
# chmod +x build-plugin.sh
