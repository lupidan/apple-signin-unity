# Changelog

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

[1.1.0]: https://github.com/lupidan/apple-signin-unity/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/lupidan/apple-signin-unity/compare/v0.4.0...v1.0.0
[0.4.0]: https://github.com/lupidan/apple-signin-unity/compare/0.3.0...v0.4.0
[0.3.0]: https://github.com/lupidan/apple-signin-unity/compare/0.2...0.3.0
[0.2]: https://github.com/lupidan/apple-signin-unity/releases/tag/0.2
