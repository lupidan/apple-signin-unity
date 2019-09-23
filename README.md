
<p align="center">
  <img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SignInWithApple.png" alt="Sign in With Apple"/><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/UnityIcon.png" alt="Unity 3D"/>
</p>

# Sign in with Apple Unity Plugin
[![Release](https://img.shields.io/github/v/release/lupidan/apple-signin-unity?style=for-the-badge!)](/releases/latest)

[![Stars](https://img.shields.io/github/stars/lupidan/apple-signin-unity.svg?style=social)](https://gitHub.com/lupidan/apple-signin-unity/stargazers/)
[![Followers](https://img.shields.io/github/followers/lupidan.svg?style=social)](https://github.com/lupidan?tab=followers)
[![License](https://img.shields.io/github/license/lupidan/apple-signin-unity.svg)](https://github.com/lupidan/apple-signin-unity/blob/master/LICENSE.md)

[![Donate](https://img.shields.io/static/v1.svg?label=Donate&message=%20%40lupidan&color=red&logo=paypal&style=popout)](https://paypal.me/lupidan)
[![Twitter](https://img.shields.io/twitter/follow/lupi_dan.svg?style=social)](https://twitter.com/intent/user?screen_name=lupi_dan)


<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN01.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN01.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN03.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN04.png" alt="Screenshot2" height="400"/></a>
</p>

by **Daniel Lupiañez Casares**

## Overview
Sign in with Apple plugin to use with Unity 3D game engine.

The main purpose for this plugin is to expose iOS newest feature, Sign in with Apple, to the Unity game engine.

On WWDC19, Apple announced **Sign in with Apple**, and on top of that, they announced that every iOS Application
that used any kind of Third party sign-ins (like *Sign in with Facebook*, or *Sign in with Google*), will have to support
Sign in with Apple in order to get approved for the App Store, making it **mandatory**.

## Features
### Native Sign in with Apple
- Supports Sign in with Apple, with customizable scopes (Email and Full name).
- Supports Get Credential status (Authorized, Revoked and Not Found).
- Supports Quick login (including iTunes Keychain credentials).
- Supports adding Sign In with Apple capability to Xcode project programatically in a PostBuild script.
- Supports listening to Credentials Revoked notifications.
- NSError mapping so no details are missing.
- NSPersonNameComponents support (for ALL different styles).
- Customizable callback execution (Immediate or On Demand in an Update loop f.ex)
- Customizable serialization (uses Unity default serialization, but you can add your own implementation)

## Installation
### Option 1: Unity Package manager
Available starting from Unity 2018.3.

Just add this line to the `Packages/manifest.json` file of your Unity Project. It will make the plugin available to use in your code to the latest master.
```json
"dependencies": {
    "com.lupidan.apple-signin-unity": "https://github.com/lupidan/apple-signin-unity.git",
}
```

If you want to use a specific [release](https://github.com/lupidan/apple-signin-unity/releases) in your code, just add `#release` at the end, like so:
```json
"dependencies": {
    "com.lupidan.apple-signin-unity": "https://github.com/lupidan/apple-signin-unity.git#0.4.0",
}
```

### Option 2: Unity Package file
1. Download the most recent Unity package release [here](https://github.com/lupidan/apple-signin-unity/releases)
2. Import the downloaded Unity package in your app. There are two main folders:

* The `AppleAuth` folder contains the **main plugin**.
* The `AppleAuthSample` folder contains **sample code** to use as a reference, or to test the plugin.

![Import detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/ImportPlugin.png)

## Plugin setup

To be able to use Apple's platform and framework for Authenticating with an Apple ID, we need to set up our Xcode project. Two different options are available to set up the entitlements required to enable Apple ID authentication with the iOS SDK.

### Option 1)  Programmatic setup with a Script

*RECOMMENDED*

This plugin **provides an extension method** for `ProjectCapabilityManager` ([docs](https://docs.unity3d.com/ScriptReference/iOS.Xcode.ProjectCapabilityManager.html)), used to add this entitlement programatically after an Xcode build has finished.

Simply create a Post Processing build script ([more info](https://docs.unity3d.com/ScriptReference/Callbacks.PostProcessBuildAttribute.html)) that performs the call. If you already have a post process build script, it should be simple to add to your code.

The provided extension method is `AddSignInWithApple`. No arguments are required.

Sample code:
```csharp
public static class SignInWithApplePostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        var projectPath = PBXProject.GetPBXProjectPath(path);
        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
        
        // Adds required Entitlements entry, and framework programatically
        manager.AddSignInWithApple();
        
        manager.WriteToFile();
    }
}
```

### Option 2) Manual entitlements setup

The other option is to manually setup all the entitlements in our Xcode project. Note that when making an iOS Build from Unity into the same folder, if you choose the option to overwrite, you will need to perform the Manual setup again.

1. In your generated Xcode project. Select your product and select the option *Signing And Capabilities*. You should see there an option to add a capability from a list. Just locate *Sign In With Apple* and add it to your project.

2. This should have added an Entitlements file to your project. Locate it on the project explorer (it should be a file with the extension `.entitlements`). Inside it you should see an entry like this one:

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/EntitlementsDetail.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/EntitlementsDetail.png"/></a>
</p>

3. You need to import the `AuthenticationServices.framework` library in the Build Phases->Link Binary with Libraries. **If you are targeting older iOS versions**, mark the library as `Optional`

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/FrameworksDetail.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/FrameworksDetail.png"/></a>
</p>

### Final notes regarding setup

**NOTE:** The `AuthenticationServices.framework` should be added as Optional, to support previous iOS versions, avoiding crashes at startup.

**NOTE 2:** The provided extension method uses reflection to integrate with the current tools Unity provides. It has been tested with Unity 2018.x and 2019.x. But if it fails on your particular Unity version, feel free to open a issue, specifying the Unity version.

## Implement Sign in With Apple

> Currently, it seems Sign In With Apple does not work properly in the simulator. This needs testing on a device with an iOS 13 version.

An overall flow of how the native Sign In With Apple flow works is presented in this diagram.
There is no official documentation about it, the only available source for this is the WWDC 2019 talk. You can watch it [here](https://developer.apple.com/videos/play/wwdc2019/706/)

![Frameworks detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/AppleSignInFlow_v1.png)

### Initializing
```csharp
private IAppleAuthManager appleAuthManager;
private OnDemandMessageHandlerScheduler scheduler;

void Start()
{
    ...
    // Creates the Scheduler to execute the pending callbacks on demand
    this.scheduler = new OnDemandMessageHandlerScheduler();
    // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
    var deserializer = new PayloadDeserializer();
    // Creates an Apple Authentication manager with the scheduler and the deserializer
    this.appleAuthManager = new AppleAuthManager(deserializer, scheduler);
    ...
}

void Update()
{
    ...
    // Updates the scheduler to execute pending response callbacks
    // This ensures they are executed inside Unity's Update loop
    this.scheduler.Update();
    ...
}
```

### Perform Sign In With Apple
If you want to Sign In and request the Email and Full Name for a user, you can do it like this:
```csharp
this._appleAuthManager.LoginWithAppleId(
    LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
    credential =>
    {
        // Obtained credential, cast it to IAppleIDCredential
        var appleIdCredential = credential as IAppleIDCredential;
        // You should save the user ID somewhere in the device
        // And now you have all the information to create/login a user in your system
        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
    },
    error =>
    {
        // Something went wrong
    });
```

### Checking credential status
Given an `userId` from a previous successful sign in. You can check the credential state of that user ID like so:
```csharp
this.appleAuthManager.GetCredentialState(
    userId,
    state =>
    {
        switch (state)
        {
            case CredentialState.Authorized:
                // User ID is still valid. Perform login
                break;
            
            case CredentialState.Revoked:
                // User ID was revoked. Try Quick Login
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

### Quick login

> If you were supporting credentials being stored in the [keychain](https://developer.apple.com/documentation/security/keychain_services/keychain_items?language=objc) for your users, the Quick login should (in theory) return those credentials as a IPasswordCredential.

This should be tried if your saved User Id from apple was revoked. According to Apple, when going with this approach you should see something similar to this:

![Frameworks detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/QuickLogin.png)

```csharp
this.appleAuthManager.QuickLogin(
    credential =>
    {
        // Received a valid credential!
        // Try casting to IAppleIDCredential or IPasswordCredential
        var appleIdCredential = credential as IAppleIDCredential; // Previous Apple sign in credential
        var passwordCredential = credential as IPasswordCredential; // Saved Keychain credential (read about Keychain Items)
    },
    error =>
    {
        // Something went wrong. Go to login screen
    });
```

### Listening to credentials revoked notification

It may be that your user suddenly decides to revoke the authorization that was given previously. You should be able to listen to the incoming notification by registering a callback. If the callback passed is null, you will be clearing it and stop listening to it.


```csharp
// If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
this._appleAuthManager.SetCredentialsRevokedCallback(result =>
{
	// Sign in with Apple Credentials were revoked
});
```

## Some more info
#### About JSON communication
This plugin does **NOT** use UnitySendMessage, meaning that there will be no need to instantiate any components in
GameObject instances. Just create an instance of the [main class] and keep it alive wherever you would like to use/receive
the data from the sign in.

The communication between the native Objective-C and C# is made through a static context using JSON serialization and deserialization.

#### About Customizable deserialization
By default, this plugin supports Unity's JSON Serialization system, so no extra libraries are added. A few workarounds had to be made to support it.
However, if your app/game uses a different serialization library (JSON.net, MiniJSON, etc...), you can create you custom deserializer.

As long as you implement the IPayloadDeserializer interface, you can pass that interface to the main NativeAppleAuth Constructor to use your own solution.

#### About customizable callback scheduling
Pretty much all the calls are async. This means that the native callback has to be executed back.
It's recommended to schedule the callbacks (or execute them) from a MonoBehaviour of your choice.

