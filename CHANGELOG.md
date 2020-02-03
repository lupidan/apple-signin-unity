# Changelog

## [Unreleased] v1.1.0
### Added
- Adds a CHANGELOG.md file
- Adds new v2 diagram files (`.drawio` and `.png`)
- Adds structure containing arguments for Quick Login `AppleAuthQuickLoginArgs`.
The structure contains an optional `Nonce`.
- Adds structure containing arguments for Normal Login `AppleAuthLoginArgs`.
The structure contains the mandatory `LoginOptions` an optional `Nonce`.
- Adds support in native code to receive and set a `Nonce` for
the Authorization Requests in both Quick Login and Sign in With Apple

### Changed
- `QuickLogin` now requires a `AppleAuthQuickLoginArgs` to perform the call
- `LoginWithAppleId` now requires a `AppleAuthLoginArgs` to perform the call
- Updates main package file to include both `CHANGELOG.md` and `CHANGELOG.md.meta files`
- Updates the sample project to better resemble the expected Apple flow
- Updates README.md with up to date documentation

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

[Unreleased]: https://github.com/lupidan/apple-signin-unity/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/lupidan/apple-signin-unity/compare/v0.4.0...v1.0.0
[0.4.0]: https://github.com/lupidan/apple-signin-unity/compare/0.3.0...v0.4.0
[0.3.0]: https://github.com/lupidan/apple-signin-unity/compare/0.2...0.3.0
[0.2]: https://github.com/lupidan/apple-signin-unity/releases/tag/0.2
