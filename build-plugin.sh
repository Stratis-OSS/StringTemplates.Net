#!/bin/bash
set -e

if [ -z "$1" ]; then
    echo "Usage: $0 <plugin-name>" >&2
    echo "Example: $0 Configuration" >&2
    exit 1
fi

PLUGIN="$1"
PROJECT_DIR="./src/plugins/StringTemplates.Plugins.${PLUGIN}"

if [ ! -d "$PROJECT_DIR" ]; then
    echo "Error: plugin directory not found: $PROJECT_DIR" >&2
    exit 1
fi

cd "$PROJECT_DIR"
dotnet clean
dotnet build -c Release
dotnet pack -c Release

# To run:
# chmod +x build-plugin.sh
# ./build-plugin.sh <plugin-name>
