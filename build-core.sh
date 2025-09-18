#!/bin/bash
set -e

cd ./src/StringTemplates
dotnet clean
dotnet build -c Release
dotnet pack -c Release

# To run:
# chmod +x build-core.sh
