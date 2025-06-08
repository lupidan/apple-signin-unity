# Changelog
## [Unreleased]
### Added
### Changed
### Removed

## [1.5.0] - 2025-06-08
### Breaking ⚠️⚠️⚠️
- **GitHub import URL updated.** The package must now be imported using a `?path=Source` URL suffix, and tags no longer include a `v` prefix. This is a breaking change for users importing directly from GitHub.

#### Previous GitHub import (before 1.5.0):
```jsonc
"dependencies": {
    "com.lupidan.apple-signin-unity": "https://github.com/lupidan/apple-signin-unity.git#v1.4.4"
}
```

#### Updated GitHub import (starting from 1.5.0):
```jsonc
"dependencies": {
    "com.lupidan.apple-signin-unity": "https://github.com/lupidan/apple-signin-unity.git?path=Source#1.5.0"
}
```

### Changed
- Fixed issue with native files not being included in visionOS builds.
- Fixed issue with header files not being included in tvOS builds.
- Reworked package structure and folders to more closely follow Unity's package layout guidelines.
- Updated macOS helper method in `AppleAuthMacosPostprocessorHelper.FixManagerBundleIdentifier` to better locate the `.app` or `.xcodeproj` bundle.

### Removed
- Removed unnecessary `.meta` files inside the `MacOSAppleAuthManager.bundle` to avoid unintended asset import or warnings in Unity.

## [1.4.4] - 2024-11-03
### Changed
- Updates `AddSignInWithAppleWithCompatibility` to support new public constructor for `PBXCapabilityType` introduced in Unity 6000.0.23f1
- Updates `AddSignInWithAppleWithCompatibility` to automatically handle the target to add the compatibility and add the framework. It´s no longer required to provide manually the Unity Framework target.
- Added basic support for visionOS
- Increased minimum Unity Version to 2020.3.48f1

## [1.4.3] - 2023-09-30
### Changed
- Updates `AddSignInWithAppleWithCompatibility` to support new public constructor for `PBXCapabilityType` introduced in Unity 2022.3.10
- Increases minimum target for the macOS bundle to 10.13

### Added
- Add `ToString` override to `AppleError`

## [1.4.2] - 2020-07-17
### Changed
- Handles empty `NSPersonNameComponents` sent by Apple when not requesting a name, to be `nil` natively.
- Updated `MacOSAppleAuthManager.bundle` with the updated native code

### Removed
- Removes `FixSerializationForFullPersonName` and any usage of it when deserializing to avoid NRE

## [1.4.1] - 2020-11-28
### Added
- Updates plugin's main `MacOSAppleAuthManager.bundle` to support Apple Silicon `arm64` architecture

### Changed
- Updates some elements in the dedicated macOS documentation file that were incorrect

## [1.4.0] - 2020-10-18
### Added
- Adds static class `AppleAuthMacosPostprocessorHelper`, so now there should always be an AppleAuth.Editor namespace independent of the current platform.
- Adds static method to `AppleAuthMacosPostprocessorHelper`, `FixManagerBundleIdentifier` is a method to change the plugin's bundle identifier to a custom one based on the current project's application identifier. This should avoid CFBundleIdentifier collision errors when uploading to the MacOS App Store.
- Adds enum value for `LoginOptions` to not request full name or email, `LoginOptions.None`.

### Changed
- Updates sample code Postprocessor script to support the new recommended post processing for macOS builds

## [1.3.0] - 2020-07-18
### Added
- Adds support to set the `State` when making a Login or a Quick Login request to sign in with Apple. 
- Improves deserialization for the data.

### Changed
- Makes the parsed classes `internal` to force the usage of the interfaces.
- Minor changes for lower C# compatibility
- `GetAuthorizationErrorCode` no longer returns a nullable reference type. If the error can't be obtained, it returns `Unknown` instead.

## [1.2.0] - 2020-05-16
### Added
- Updates native code to support macOS, including NSPersonNameComponents support.
- Adds Xcode project `MacOSAppleAuthManager/MacOSAppleAuthManager.xcodeproj` to generate `MacOSAppleAuthManager.bundle` reusing existing iOS objective-c files. Bundle identifier is `com.lupidan.MacOSAppleAuthManager`. Minimum macOS version supported is 10.9.
- Makes it so compiling the Xcode project automatically updates the `MacOSAppleAuthManager.bundle` inside `AppleAuth/Native/macOS`
- Adds *unsigned precompiled* `MacOSAppleAuthManager.bundle`.
- Adds `LandscapeSampleScene.unity` scene for a Landscape version to use on macOS builds.
- Adds `macOS_NOTES.md` readme dedicated to macOS codesigning.
- Adds details to install the plugin with ![OpenUPM](https://openupm.com/) in `README.md`

### Changed
- Fixes PostProcessing for Unity 2019.3
- Renamed plugin´s extension method for `ProjectCapabilityManager` to avoid conflicts with the method added in Unity 2019.3. New method name is `AddSignInWithAppleWithCompatibility`.
- Namespace `AppleAuth.IOS.NativeMessages` becomes `AppleAuth.NativeMessages`
- Modified slightly implementation for Person name formatting
- Modified native implementation of AppleAuthManager to support macOS

## [1.1.0] - 2020-02-20
### Added
- Adds a CHANGELOG.md file
- Adds support for tvOS (Experimental)
- Adds new v2 diagram files (`.drawio` and `.png`)
- Adds `AppleAuthQuickLoginArgs` struct containing arguments for Quick Login. (With optional `Nonce`)
- Adds `AppleAuthLoginArgs` structure containing arguments for Normal Login like `LoginOptions`. (With optional `Nonce`)
- Adds support in native code to receive and set a `Nonce` for the Authorization Requests in both Quick Login and Sign in With Apple
- Adds `Update` method to `IAppleAuthManager` to update pending callbacks
- Better API version handling in native objective-c code

### Changed
- Namespace `AppleAuth.IOS` becomes `AppleAuth`
- Namespace `AppleAuth.IOS.Enums` becomes `AppleAuth.Enums`
- Namespace `AppleAuth.IOS.Extensions` becomes `AppleAuth.Extensions`
- Namespace `AppleAuth.IOS.Interfaces` becomes `AppleAuth.Interfaces`
- Namespace `AppleAuth.IOS.Interfaces` becomes `AppleAuth.Interfaces`

- `QuickLogin` now requires a `AppleAuthQuickLoginArgs` to perform the call. Other `QuickLogin` method marked as obsolete.
- `LoginWithAppleId` now requires a `AppleAuthLoginArgs` to perform the call. Other `LoginWithAppleId` method marked as obsolete.
- `AppleAuthManager` no longer requires a Scheduler, the scheduling is built in the manager instance with the method `Update`
- When receiving a completely empty `FullPersonName`, the instance is cleared after deserialization.
- Fixes bug when setting credentials revoked callback between multiple instances of  `AppleAuthManager`

- Updates main package file to include both `CHANGELOG.md` and `CHANGELOG.md.meta files`
- Updates the sample project to better resemble the expected Apple flow
- Updates README.md with up to date documentation

### Removed
- Removes Schedulers to simplify the callback handling. `Update` call was moved to `IAppleAuthManager`.

## [1.0.0] - 2020-01-23
- No plugin code changes. Just making the v1.0.0 release. No more "preview package"
- Adding FAQ to the README

## [0.4.0] - 2019-09-24
- Support to stop listening to credential revoked notifications by setting the callback to null
- Implemented a nicer example of the plugin, and rearranged the code

## [0.3.0] - 2019-09-20
- Rearranged files to support for Unity Package Manager
- Made code more compatible with earlier C# versions
- Updated logic for the whole flow
- More documentation in the Readme

## [0.2] - 2019-08-04
- Some more documentation was added
- Added support to listen to Revoked Credentials notifications
- Solved possible crashes that could happen when trying to execute a callback in the Native Message Handler, if the callback was to throw an exception, the application would crash.

[Unreleased]: https://github.com/lupidan/apple-signin-unity/compare/1.5.0...HEAD
[1.5.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.4.5...1.5.0
[1.4.4]: https://github.com/lupidan/apple-signin-unity/compare/v1.4.3...v1.4.4
[1.4.3]: https://github.com/lupidan/apple-signin-unity/compare/v1.4.2...v1.4.3
[1.4.2]: https://github.com/lupidan/apple-signin-unity/compare/v1.4.1...v1.4.2
[1.4.1]: https://github.com/lupidan/apple-signin-unity/compare/v1.4.0...v1.4.1
[1.4.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.3.0...v1.4.0
[1.3.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/lupidan/apple-signin-unity/compare/v0.4.0...v1.0.0
[0.4.0]: https://github.com/lupidan/apple-signin-unity/compare/0.3.0...v0.4.0
[0.3.0]: https://github.com/lupidan/apple-signin-unity/compare/0.2...0.3.0
[0.2]: https://github.com/lupidan/apple-signin-unity/releases/tag/0.2
