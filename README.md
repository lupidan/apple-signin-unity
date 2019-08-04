<p align="center">
  <img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SignInWithApple.png" alt="Sign in With Apple"/><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/UnityIcon.png" alt="Unity 3D"/>
</p>

# Sign in with Apple Unity Plugin
[![Stars](https://img.shields.io/github/stars/lupidan/apple-signin-unity.svg?style=social)](https://gitHub.com/lupidan/apple-signin-unity/stargazers/)
[![Followers](https://img.shields.io/github/followers/lupidan.svg?style=social)](https://github.com/lupidan?tab=followers)
[![License](https://img.shields.io/github/license/lupidan/apple-signin-unity.svg)](https://github.com/lupidan/apple-signin-unity/blob/master/LICENSE.md)

[![Twitter](https://img.shields.io/twitter/follow/lupi_dan.svg?style=social)](https://twitter.com/intent/user?screen_name=lupi_dan)

[![Donate](https://img.shields.io/static/v1.svg?label=Donate&message=%20%40lupidan&color=red&logo=paypal&style=popout)](https://paypal.me/lupidan)

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN04.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN04.png" alt="Screenshot2" height="400"/></a>
</p>

by **Daniel Lupiañez Casares**

## Overview
Sign in with Apple plugin to use with Unity 3D game engine.

The main purpose for this plugin is to expose iOS newest feature, Sign in with Apple, to the Unity game engine.

On WWDC19, Apple announced **Sign in with Apple**, and on top of that, they announced that every iOS Application
that used any kind of Third party sign-ins (like *Sign in with Facebook*, or *Sign in with Google*), will have to support
Sign in with Apple in order to get approved for the App Store, making it **mandatory**.

## Setting up the plugin
### Installing the package
1. Download the most recent Unity package <a href="https://github.com/lupidan/apple-signin-unity/releases/download/0.2/AppleSignInUnity.unitypackage">here</a>
2. Import the downloaded Unity package in your app. There are two main folders:
* The `AppleAuth` folder contains the **main plugin**.
* The `AppleAuthSample` folder contains **sample code** to use as a reference, or to test the plugin.

![Import detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/ImportPlugin.png)

### Set up entitlements

To be able to use Apple's platform and framework for Authenticating with an Apple ID, we need to set up our Xcode project. Two different options are available to set up the entitlements required to enable Apple ID authentication with the iOS SDK.

#### Option 1)  Programmatic setup with a Script

**RECOMMENDED**

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

#### Option 2) Manual setup

The first option is to manually setup all the entitlements in our Xcode project. Note that when making an iOS Build from Unity into the same folder, if you choose the option to overwrite, you will need to perform the Manual setup again. 

1. In your generated Xcode project. Select your product and select the option "Signing And Capabilities". You should see there an option to add a capability from a list. Just locate "Sign In With Apple" and add it to your project.

2. This should have added an Entitlements file to your project. Locate it on the project explorer (it should be a file with the extension `.entitlements`). Inside it you should see an entry like this one:
![Entitlements detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/EntitlementsDetail.png)

3. You need to import the `AuthenticationServices.framework` library in the Build Phases->Link Binary with Libraries. **If you are targeting older iOS versions**, mark the library as `Optional`
![Frameworks detail](https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/FrameworksDetail.png)


#### Final notes

**NOTE:** The `AuthenticationServices.framework` should be added as Optional, to support previous iOS versions, avoiding crashes at startup.

**NOTE 2:** The provided extension method uses reflection to integrate with the current tools Unity provides. It has been tested with Unity 2018.x and 2019.x. But if it fails on your particular Unity version, feel free to open a issue, specifying the Unity version.

## JSON communication
This plugin does **NOT** use UnitySendMessage, meaning that there will be no need to instantiate any components in
GameObject instances. Just create an instance of the [main class] and keep it alive wherever you would like to use/receive
the data from the sign in.

The communication between the native Objective-C and C# is made through a static context using JSON serialization and deserialization.

## Custom deserialization
By default, this plugin supports Unity's JSON Serialization system, so no extra libraries are added. A few workarounds had to be made to support it.
However, if your app/game uses a different serialization library (JSON.net, MiniJSON, etc...), you can create you custom deserializer.

As long as you implement the IPayloadDeserializer interface, you can pass that interface to the main NativeAppleAuth Constructor to use your own solution.

## Current progress

### iOS Native 
- ☒ GameObject-less messaging system based on strings.
- ☒ Get Credential state for a specific User-Id.
- ☒ Sign in with Apple.
- ☒ Silent Login to support iTunes Keychains (to be properly tested).
- ☒ Programatically add new AuthenticationServices.framework and Entitlements entry when building for iOS.
- ☒ NSPersonNameComponents formatting for all different styles.
- ☒ NSError codes mapping into Unity.
- ☒ Customize Sign in With Apple call from Unity. (request email and/or full name)
- ☒ Support to schedule all callbacks in user-configured loops (ex. in an MonoBehaviour's Update loop).
- ☐ Add support for credential revoked notifications

### Rest API
- ☐ TBD
