#!/bin/bash
set -euo pipefail

cd "$(dirname "$0")"

usage() {
    echo "Usage: $0 --version <version> --build-number <number> --project <path.xcodeproj> --archive-dir <dir> --output-dir <dir>"
    exit 1
}

VERSION=""
BUILD_NUMBER=""
PROJECT=""
ARCHIVE_DIR=""
OUTPUT_DIR=""

while [[ $# -gt 0 ]]; do
    case "$1" in
        --version)
            VERSION="$2"
            shift 2
            ;;
        --build-number)
            BUILD_NUMBER="$2"
            shift 2
            ;;
        --project)
            PROJECT="$2"
            shift 2
            ;;
        --archive-dir)
            ARCHIVE_DIR="$2"
            shift 2
            ;;
        --output-dir)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        *)
            echo "Unknown argument: $1"
            usage
            ;;
    esac
done

[[ -z "$VERSION" ]] && echo "Missing required --version" && usage
[[ -z "$BUILD_NUMBER" ]] && echo "Missing required --build-number" && usage
[[ -z "$PROJECT" ]] && echo "Missing required --project" && usage
[[ -z "$ARCHIVE_DIR" ]] && echo "Missing required --archive-dir" && usage
[[ -z "$OUTPUT_DIR" ]] && echo "Missing required --output-dir" && usage

FRAMEWORK_IN_ARCHIVE="Products/Library/Frameworks/AppleAuthNative.framework"

echo "Building version $VERSION build $BUILD_NUMBER"

rm -rf "$ARCHIVE_DIR" "$OUTPUT_DIR"
mkdir -p "$ARCHIVE_DIR" "$OUTPUT_DIR"

archive() {
    local scheme="$1"
    local destination="$2"
    local archive_name="$3"

    echo "Archiving $scheme for $destination..."
    xcodebuild archive \
        -project "$PROJECT" \
        -scheme "$scheme" \
        -destination "$destination" \
        -archivePath "$ARCHIVE_DIR/$archive_name" \
        -configuration Release \
        SKIP_INSTALL=NO \
        MARKETING_VERSION="$VERSION" \
        CURRENT_PROJECT_VERSION="$BUILD_NUMBER" \
        2>&1 | tail -20
}

# Static slices that go into the xcframework
archive "AppleAuthNative-iOS" "generic/platform=iOS" "ios"
archive "AppleAuthNative-iOS" "generic/platform=iOS Simulator" "ios-sim"
archive "AppleAuthNative-iOS" "generic/platform=macOS,variant=Mac Catalyst" "ios-maccatalyst"
archive "AppleAuthNative-tvOS" "generic/platform=tvOS" "tvos"
archive "AppleAuthNative-tvOS" "generic/platform=tvOS Simulator" "tvos-sim"
# Requires an Apple Silicon host with the visionOS Simulator runtime installed
# (Xcode > Settings > Platforms, or `xcodebuild -downloadPlatform visionOS`).
# Not runnable on Intel Macs at all -- the visionOS Simulator doesn't ship for them.
archive "AppleAuthNative-visionOS" "generic/platform=visionOS" "visionos"
archive "AppleAuthNative-visionOS" "generic/platform=visionOS Simulator" "visionos-sim"
archive "AppleAuthNative-macOS" "generic/platform=macOS" "macos-static"

# Standalone dynamic macOS framework (not part of the xcframework)
archive "AppleAuthNative-macOS-Dynamic" "generic/platform=macOS" "macos-dynamic"

echo "Creating AppleAuthNative.xcframework..."
xcodebuild -create-xcframework \
    -framework "$ARCHIVE_DIR/ios.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/ios-sim.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/ios-maccatalyst.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/tvos.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/tvos-sim.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/visionos.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/visionos-sim.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -framework "$ARCHIVE_DIR/macos-static.xcarchive/$FRAMEWORK_IN_ARCHIVE" \
    -output "$OUTPUT_DIR/AppleAuthNative.xcframework"

echo "Copying standalone dynamic macOS framework..."
cp -R "$ARCHIVE_DIR/macos-dynamic.xcarchive/Products/Library/Frameworks/AppleAuthNative-dynamic.framework" "$OUTPUT_DIR/AppleAuthNative-dynamic.framework"

echo "Packaging tar.gz archives (preserves symlinks, unlike zip)..."
tar czf "$OUTPUT_DIR/AppleAuthNative.xcframework.tar.gz" -C "$OUTPUT_DIR" AppleAuthNative.xcframework
tar czf "$OUTPUT_DIR/AppleAuthNative-dynamic.framework.tar.gz" -C "$OUTPUT_DIR" AppleAuthNative-dynamic.framework

echo "Done. Output in $OUTPUT_DIR"
