<p align="center">
  <img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SignInWithApple.png" alt="Sign in With Apple"/><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/UnityIcon.png" alt="Unity 3D"/>
</p>

# Sign in with Apple Unity Plugin

by **Daniel Lupiañez Casares**


[![Sponsor](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub)](https://github.com/sponsors/lupidan)


[![Release](https://img.shields.io/github/v/release/lupidan/apple-signin-unity?style=for-the-badge!)](https://github.com/lupidan/apple-signin-unity/releases)
[![License](https://img.shields.io/github/license/lupidan/apple-signin-unity.svg)](https://github.com/lupidan/apple-signin-unity/blob/master/LICENSE.md)
[![CHANGELOG](https://img.shields.io/badge/-CHANGELOG-informational)](https://github.com/lupidan/apple-signin-unity/blob/master/CHANGELOG.md)


[![openupm](https://img.shields.io/npm/v/com.lupidan.apple-signin-unity?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.lupidan.apple-signin-unity/)


[![Stars](https://img.shields.io/github/stars/lupidan/apple-signin-unity.svg?style=social)](https://github.com/lupidan/apple-signin-unity/stargazers)
[![Followers](https://img.shields.io/github/followers/lupidan.svg?style=social)](https://github.com/lupidan?tab=followers)


<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN01.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN01.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN02.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN02.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN03.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/SCRN03.png" alt="Screenshot2" height="400"/></a>
</p>



  * [Overview](#overview)
  * [Features](#features)
    + [Native Sign in with Apple](#native-sign-in-with-apple)
  * [Installation](#installation)
    + [Unity Package Manager](#unity-package-manager)
        - [Install via Git URL](#install-via-git-url)
        - [Install via OpenUPM](#install-via-openupm)
    + [Unity Package File](#unity-package-file)
  * [Plugin setup (iOS/tvOS)](#plugin-setup-iostvos)
    + [Programmatic setup with a Script](#programmatic-setup-with-a-script)
    + [Manual entitlements setup](#manual-entitlements-setup)
    + [Enabling Apple capability](#enabling-apple-capability)
    + [Final notes regarding setup](#final-notes-regarding-setup)
  * [Plugin setup (macOS)](#plugin-setup-macos)
  * [Implement Sign in With Apple](#implement-sign-in-with-apple)
    + [Initializing](#initializing)
    + [Perform Sign In With Apple](#perform-sign-in-with-apple)
    + [Quick login](#quick-login)
    + [Checking credential status](#checking-credential-status)
    + [Listening to credentials revoked notification](#listening-to-credentials-revoked-notification)
    + [Nonce and State support for Authorization Requests](#nonce-and-state-support-for-authorization-requests)
  * [FAQ](#faq)
    + [Does it support landscape orientations?](#does-it-support-landscape-orientations)
    + [How can I Logout? Does the plugin provide any Logout option?](#how-can-i-logout-does-the-plugin-provide-any-logout-option)
    + [I am not getting a full name, or an email, even though I am requesting them in the LoginWithAppleId call](#i-am-not-getting-a-full-name-or-an-email-even-though-i-am-requesting-them-in-the-loginwithappleid-call)
    + [Is it possible to NOT request the user's email or full name?](#is-it-possible-to-not-request-the-users-email-or-full-name)
    + [Does the plugin use UnitySendMessage?](#does-the-plugin-use-unitysendmessage)
    + [Why do I need to call Update manually on the AppleAuthManager instance?](#why-do-i-need-to-call-update-manually-on-the-appleAuthManager-instance)
    + [What deserialization library does it use by default?](#what-deserialization-library-does-it-use-by-default)
    + [Any way to get a refresh token after the first user authorization?](#any-way-to-get-a-refresh-token-after-the-first-user-authorization)
    + [I am getting a CFBundleIdentifier Collision error when uploading my app to the macOS App Store](#i-am-getting-a-cfbundleIdentifier-collision-error-when-uploading-my-app-to-the-macos-app-store)

## Overview
Sign in with Apple plugin to use with Unity 3D game engine.

This plugin supports the following platforms:
* **iOS**
* **macOS** Intel `x86_64` AND Apple Silicon `arm64`(Experimental) ([NOTES](./docs/macOS_NOTES.md))
* **tvOS** (Experimental)
* **visionOS** (Experimental)

The main purpose for this plugin is to expose Apple's newest feature, Sign in with Apple, to the Unity game engine.

On WWDC19, Apple announced **Sign in with Apple**, and on top of that, they announced that every iOS/tvOS/macOS Application
that used any kind of Third party sign-ins (like *Sign in with Facebook*, or *Sign in with Google*), will have to support
Sign in with Apple in order to get approved for the App Store, making it **mandatory**.

## Features
### Native Sign in with Apple
- Support for iOS
- Support for macOS: Intel `x86_64` AND Apple Silicon `arm64`(Experimental) ([NOTES](./docs/macOS_NOTES.md))
- Support for tvOS (Experimental)
- Support for visionOS (Experimental)
- Supports Sign in with Apple, with customizable scopes (Email and Full name).
- Supports Get Credential status (Authorized, Revoked and Not Found).
- Supports Quick login (including iTunes Keychain credentials).
- Supports adding Sign In with Apple capability to Xcode project programatically in a PostBuild script.
- Supports listening to Credentials Revoked notifications.
- Supports setting custom Nonce and State for authorization requests when Signing In, and attempting a Quick Login.

- NSError mapping so no details are missing.
- NSPersonNameComponents support (for ALL different styles).
- Customizable serialization (uses Unity default serialization, but you can add your own implementation)

## Installation

> Current stable version is 1.5.0

There are two options available to install this plugin. Either using the Unity Package Manager, or the traditional `.unitypackage` file.

### Unity Package Manager

#### Install via Git URL

Available starting from Unity 2020.3.

Just add this line to the `Packages/manifest.json` file of your Unity Project:

```json
"dependencies": {
    "com.lupidan.apple-signin-unity": "https://github.com/lupidan/apple-signin-unity.git?path=Source#1.5.0",
}
```

#### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.lupidan.apple-signin-unity
```

### Unity Package File
1. Download the most recent Unity package release [here](https://github.com/lupidan/apple-signin-unity/releases)
2. Import the downloaded Unity package in your app. There are two main folders:

* The `AppleAuth` folder contains the **main plugin**.
* The `AppleAuthSample` folder contains **sample code** to use as a reference, or to test the plugin.

![Import detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/ImportPlugin.png)

## Plugin setup (iOS/tvOS)

To be able to use Apple's platform and framework for Authenticating with an Apple ID, we need to set up our Xcode project. Two different options are available to set up the entitlements required to enable Apple ID authentication with the iOS SDK.

### Programmatic setup with a Script

*RECOMMENDED*

This plugin **provides an extension method** for `ProjectCapabilityManager` ([docs](https://docs.unity3d.com/ScriptReference/iOS.Xcode.ProjectCapabilityManager.html)), used to add this entitlement programatically after an Xcode build has finished.

Simply create a Post Processing build script ([more info](https://docs.unity3d.com/ScriptReference/Callbacks.PostProcessBuildAttribute.html)) that performs the call. If you already have a post process build script, it should be simple to add to your code.

The provided extension method is `AddSignInWithAppleWithCompatibility`.

Sample code:
```csharp
using AppleAuth.Editor;

public static class SignInWithApplePostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        var projectPath = PBXProject.GetPBXProjectPath(path);
        var project = new PBXProject();
        project.ReadFromString(System.IO.File.ReadAllText(projectPath));
        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
        manager.AddSignInWithAppleWithCompatibility();
        manager.WriteToFile();
    }
}
```

### Manual entitlements setup

The other option is to manually setup all the entitlements in our Xcode project. Note that when making an iOS Build from Unity into the same folder, if you choose the option to overwrite, you will need to perform the Manual setup again.

1. In your generated Xcode project. Select the main app Unity-iPhone target and select the option *Signing And Capabilities*. You should see there an option to add a capability from a list. Just locate *Sign In With Apple* and add it to your project.

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddEntitlements.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddEntitlements.png" alt="Add Entitlements" width="400"/></a>
</p>


2. This should have added an Entitlements file to your project. Locate it on the project explorer (it should be a file with the extension `.entitlements`). Inside it you should see an entry like this one:

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/EntitlementsDetail.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/EntitlementsDetail.png"/></a>
</p>

3. You need to import the `AuthenticationServices.framework` library in the Build Phases->Link Binary with Libraries. **If you are targeting older iOS versions**, mark the library as `Optional`.

    For **Unity 2019.3** onwards, add it to the UnityFramework target

    For **previous Unity versions**, add it to the main Unity-iPhone target

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddFramework20193.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddFramework20193.png" alt="Add Framework 2019.3" width="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddFrameworkPrevious.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AddFrameworkPrevious.png" alt="Add Framework Previous versions" width="400"/></a>
</p>

### Enabling Apple capability

You will also need to **setup everything** in the Apple's developer portal. More information can be found [here](https://help.apple.com/developer-account/#/devde676e696). Please remember this plugin only supports native Sign In With Apple on iOS (no REST API support).

There is also a [Getting Started site](https://developer.apple.com/sign-in-with-apple/get-started/).

### Final notes regarding setup

The `AuthenticationServices.framework` should be added as Optional, to support previous iOS versions, avoiding crashes at startup.

The provided extension method uses reflection to integrate with the current tools Unity provides. It has been tested with Unity 2020.x, 2021.x, 2022.x and 6000.0. But if it fails on your particular Unity version, feel free to open a issue, specifying the Unity version.

## Plugin setup (visionOS)

On visionOS, the setup is pretty much the same as the iOS/tvOS setup.
However, due to how Unity's visionOS project is exported, you need to adapt the Postprocess code to find the proper xcode project filename:

if (target == BuildTarget.VisionOS)
{
    projectPath = projectPath.Replace("Unity-iPhone.xcodeproj", "Unity-VisionOS.xcodeproj");
}

## Plugin setup (macOS)

An unsigned precompiled `.bundle` file is available. The precompiled `.bundle` should support both architectures: Intel `x86_64` & Apple Silicon `arm64`(Experimental). The bundle should be automatically included in your macOS builds.
However that `.bundle` needs to be modified to avoid issues when uploading it to the MacOS App Store.

In particular, the bundle identifier of that `.bundle` needs to be modified to a custom one.

To automate the process, there is a helper method that will change the bundle identifier to one based on your project's application identifier.
You should call this method on a Postprocess build script of your choice.

```csharp
using AppleAuth.Editor;

public static class SignInWithApplePostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.StandaloneOSX)
            return;

        AppleAuthMacosPostprocessorHelper.FixManagerBundleIdentifier(target, path);
    }
}
```

The Xcode project with the source code to generate a new bundle file is available at `MacOSAppleAuthManager/MacOSAppleAuthManager.xcodeproj`

To support the feature, **the app needs to be codesigned correctly**, including the required entitlements. For more information regarding macOS codesign, please follow this [link](./docs/macOS_NOTES.md).

## Implement Sign in With Apple

> Currently, it seems Sign In With Apple does not work properly in the simulator. This needs testing on a device with an iOS 13 version.

An overall flow of how the native Sign In With Apple flow could work is presented in this diagram.

![Frameworks detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/AppleSignInFlow_v3.png)

### Initializing
```csharp
private IAppleAuthManager appleAuthManager;

void Start()
{
    ...
   // If the current platform is supported
   if (AppleAuthManager.IsCurrentPlatformSupported)
   {
       // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
       var deserializer = new PayloadDeserializer();
       // Creates an Apple Authentication manager with the deserializer
       this.appleAuthManager = new AppleAuthManager(deserializer);    
   }
    ...
}

void Update()
{
    ...
    // Updates the AppleAuthManager instance to execute
    // pending callbacks inside Unity's execution loop
    if (this.appleAuthManager != null)
    {
        this.appleAuthManager.Update();
    }
    ...
}
```

### Perform Sign In With Apple

> :warning: You will receive users's email and name **ONLY THE FIRST TIME THE USER LOGINS**. Any further login attempts **will have a NULL Email and FullName**, unless you [revoke the credentials](#how-can-i-logout-does-the-plugin-provide-any-logout-option)

If you want to Sign In and request the Email and Full Name for a user, you can do it like this:

```csharp
var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

this.appleAuthManager.LoginWithAppleId(
    loginArgs,
    credential =>
    {
        // Obtained credential, cast it to IAppleIDCredential
        var appleIdCredential = credential as IAppleIDCredential;
        if (appleIdCredential != null)
        {
            // Apple User ID
            // You should save the user ID somewhere in the device
            var userId = appleIdCredential.User;
            PlayerPrefs.SetString(AppleUserIdKey, userId);

            // Email (Received ONLY in the first login)
            var email = appleIdCredential.Email;

            // Full name (Received ONLY in the first login)
            var fullName = appleIdCredential.FullName;

            // Identity token
            var identityToken = Encoding.UTF8.GetString(
                appleIdCredential.IdentityToken,
                0,
                appleIdCredential.IdentityToken.Length);

            // Authorization code
            var authorizationCode = Encoding.UTF8.GetString(
                appleIdCredential.AuthorizationCode,
                0,
                appleIdCredential.AuthorizationCode.Length);

            // And now you have all the information to create/login a user in your system
        }
    },
    error =>
    {
        // Something went wrong
        var authorizationErrorCode = error.GetAuthorizationErrorCode();
    });
```

### Quick login

This should be the **first thing to try** when the user first runs the application.

If the user has previously authorized the app to login with Apple, this will open a native dialog to re-confirm the login, and obtain an Apple User ID.

If the credentials were never given, or they were revoked, the Quick login will fail.

![Frameworks detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/93e79cacc665fcd3930bda9e27e8cb27631ff1f7/Img/QuickLogin.png)

```csharp
var quickLoginArgs = new AppleAuthQuickLoginArgs();

this.appleAuthManager.QuickLogin(
    quickLoginArgs,
    credential =>
    {
        // Received a valid credential!
        // Try casting to IAppleIDCredential or IPasswordCredential

        // Previous Apple sign in credential
        var appleIdCredential = credential as IAppleIDCredential; 

        // Saved Keychain credential (read about Keychain Items)
        var passwordCredential = credential as IPasswordCredential;
    },
    error =>
    {
        // Quick login failed. The user has never used Sign in With Apple on your app. Go to login screen
    });
```

Note that, if this succeeds, you will **ONLY** receive the Apple User ID (no email or name, even if it was previously requested).

#### IOS Keychain Support
When performing a quick login, if the SDK detects [IOS Keychain credentials](https://developer.apple.com/documentation/security/keychain_services/keychain_items?language=objc) for your app, it will return those.

Just cast the credential to `IPasswordCredential` to get the login details for the user.

### Checking credential status

This is used to verify that an Apple User ID is still valid.

Given an `userId` from a previous successful sign in.
You can check the credential state of that user ID like so:

```csharp
this.appleAuthManager.GetCredentialState(
    userId,
    state =>
    {
        switch (state)
        {
            case CredentialState.Authorized:
                // User ID is still valid. Login the user.
                break;
            
            case CredentialState.Revoked:
                // User ID was revoked. Go to login screen.
                break;
            
            case CredentialState.NotFound:
                // User ID was not found. Go to login screen.
                break;
        }
    },
    error =>
    {
        // Something went wrong
    });
```

### Listening to credentials revoked notification

It may be that your user suddenly decides to revoke the authorization that was given previously. You should be able to listen to the incoming notification by registering a callback for it.


```csharp
this.appleAuthManager.SetCredentialsRevokedCallback(result =>
{
    // Sign in with Apple Credentials were revoked.
    // Discard credentials/user id and go to login screen.
});
```

To clear the callback, and stop listening to notifications, simply set it to `null`

```csharp
this.appleAuthManager.SetCredentialsRevokedCallback(null);
```

### Nonce and State support for Authorization Requests

Both methods, `LoginWithAppleId` and `QuickLogin`, use a custom structure containing arguments for the authorization request.

An optional `Nonce` and an optional `State` can be set for both structures when constructing them:

```csharp
// Your custom Nonce string
var yourCustomNonce = "RANDOM_NONCE_FORTHEAUTHORIZATIONREQUEST";
var yourCustomState = "RANDOM_STATE_FORTHEAUTHORIZATIONREQUEST";

// Arguments for a normal Sign In With Apple Request
var loginArgs = new AppleAuthLoginArgs(
    LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
    yourCustomNonce,
    yourCustomState);

// Arguments for a Quick Login
var quickLoginArgs = new AppleAuthQuickLoginArgs(yourCustomNonce, yourCustomState);
```

The `State` is returned later in the received Apple ID credential, allowing you to validate that the request was generated in your device.

The `Nonce` is embedded in the IdentityToken, included in the received Apple ID credential. It is important to generate a new random `Nonce` for every request. This is useful for services that provide a built in solution for **Sign In With Apple**, like [Firebase](https://firebase.google.com/docs/auth/ios/apple?authuser=0)

Some tentative guide is available for Firebase integration [here](./docs/Firebase_NOTES.md)

More info about State and Nonce can be found in [this WWDC 2020 session](https://developer.apple.com/videos/play/wwdc2020/10173/) (check at 2m35s)

## FAQ
+ [Does it support landscape orientations](#does-it-support-landscape-orientations)
+ [How can I Logout? Does the plugin provide any Logout option?](#how-can-i-logout-does-the-plugin-provide-any-logout-option)
+ [I am not getting a full name, or an email, even though I am requesting them in the LoginWithAppleId call](#i-am-not-getting-a-full-name-or-an-email-even-though-i-am-requesting-them-in-the-loginwithappleid-call)
+ [Is it possible to NOT request the user's email or full name?](#is-it-possible-to-not-request-the-users-email-or-full-name)
+ [Does the plugin use UnitySendMessage?](#does-the-plugin-use-unitysendmessage)
+ [Why do I need to call Update manually on the AppleAuthManager instance?](#why-do-i-need-to-call-update-manually-on-the-appleAuthManager-instance)
+ [What deserialization library does it use by default?](#what-deserialization-library-does-it-use-by-default)
+ [Any way to get a refresh token after the first user authorization?](#any-way-to-get-a-refresh-token-after-the-first-user-authorization)

### Does it support landscape orientations?
On **iOS 13.0**, Apple does not support landscape orientation for this feature. For more details, check this [issue](https://github.com/lupidan/apple-signin-unity/issues/5). 

### How can I Logout? Does the plugin provide any Logout option?

On **iOS 13**  Apple does not provide any method to *"logout"* programatically. If you want to *"logout"* and re-test account creation, you need to revoke the credentials through settings.

Go to `Settings` => `Click your iTunes user` => `Password & Security` => `Apple ID logins`. There you can select the app and click on `Stop using Apple ID`.

After this, the credentials are effectively revoked, your app will receive a [Credentials Revoked notification](#listening-to-credentials-revoked-notification). This will allow you to re-test account creation.

### I am not getting a full name, or an email, even though I am requesting them in the LoginWithAppleId call

This probably means that you already used Sign In with apple at some point. Apple will give you the email/name **ONLY ONCE**. Once the credential is created, __it's your app/game's responsibility to send that information somewhere__, so an account is created with the given user identifier.

If a credential was already created, you will only receive a user identifier, so it will work similarly to a Quick Login.

If you want to test new account scenarios, you need to [revoke](#how-can-i-logout-does-the-plugin-provide-any-logout-option) your app credentials for that Apple ID through the settings menu.

### Is it possible to NOT request the user's email or full name?

Yes, just provide `LoginOptions.None` when calling `LoginWithAppleId` and the user will not be asked for their email or full name.
This will skip that entire login step and make it more smooth. It is recommended if the user's email or full name is not used.

```csharp
appleAuthManager.LoginWithAppleId(LoginOptions.None, credential => { ... }, error => { ... });
```

### Does the plugin use UnitySendMessage?

No. The plugin uses callbacks in a static context with request identifiers using JSON strings. Callbacks are scheduled inside `AppleAuthManager`, and calling `Update` on it will execute those pending callbacks.

### Why do I need to call Update manually on the AppleAuthManager instance?

Callbacks from iOS SDK are executed in their own thread (normally the main thread), and outside Unity's engine control. Meaning that you can't update the UI, or worse, if your callback throws an Exception (like a simple NRE), it will crash the Game completely.

It's recommended to update the instance of `AppleAuthManager` regularly in a MonoBehaviour of your choice.

### What deserialization library does it use by default?

If you initialize the `AppleAuthManager` with the built-in `PayloadDeserializer`, it uses Unity JSON serialization system, so **no extra libraries are added**.

You can also implement your own deserialization by implementing an `IPayloadDeserializer`.

### Any way to get a refresh token on iOS to verify a user?

**NO**, That's not how Apple wants you to do it. This is how they want you to verify a user:

_On iOS:_ The first login gives you the Apple User ID and an Authorization Code to send to your backend. Checking the validity of a user on the device, from now on, should be done with `GetCredentialState` and that Apple User ID.

_On the server:_ It receives the data from the first login on iOS to create the user. Uses that received Authorization Code to get a refresh token for the user. Refreshes the token once a day.
[More info here](http://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_rest_api/verifying_a_user)

### I am getting a CFBundleIdentifier Collision error when uploading my app to the macOS App Store:

If you are experiencing an error like this when uploading your macOS app to the App Store
> The info.plist CFBundleIdentifier value 'com.lupidan.MacOSAppleAuthManager" of 'appname.app/Contents/Plugins/MacOSAppleAuthManager.bundle' is already in use by another application"

It probably means that your [postprocess build script for macOS](#plugin-setup-macos) is not setup correctly.

You should call `AppleAuthMacosPostprocessorHelper.FixManagerBundleIdentifier` to fix the plugin's bundle identifier to a custom one for your app.

You can find more details about the bug [here](https://github.com/lupidan/apple-signin-unity/issues/72)

