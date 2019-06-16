<p align="center">
  <img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SignInWithApple.png" alt="Sign in With Apple"/><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/UnityIcon.png" alt="Unity 3D"/>
</p>

# Sign in with Apple Unity Plugin
<p>
<img src="https://img.shields.io/github/stars/lupidan/apple-signin-unity.svg?style=social"/> <img src="https://img.shields.io/github/followers/lupidan.svg?style=social"/>
</p>

<p>
<img src="https://img.shields.io/twitter/follow/lupi_dan.svg?style=social"/>
</p>


<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN02.png" alt="Screenshot1" height="400"/></a>
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN04.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SCRN04.png" alt="Screenshot2" height="400"/></a>
</p>

## Overview
Sign in with Apple plugin to use with Unity 3D game engine.

The main purpose for this plugin is to expose iOS newest feature, Sign in with Apple, to the Unity game engine.

On WWDC19, Apple announced **Sign in with Apple**, and on top of that, they announced that every iOS Application
that used any kind of Third party sign-ins (like *Sign in with Facebook*, or *Sign in with Google*), will have to support
Sign in with Apple in order to get approved for the App Store, making it **mandatory**.

## Setting up the plugin
### Installing the package
1. Download the most recent Unity package <a href="https://github.com/lupidan/apple-signin-unity/releases">here</a>
2. Import the downloaded Unity package in your app. There are two main folders:
* The `AppleAuth` folder contains the **main plugin**.
* The `AppleAuthSample` folder contains **sample code** to use as a reference, or to test the plugin.

<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/ImportPlugin.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/ImportPlugin.png" alt="ImportPlugin" height=200/></a>
</p>

### Set up entitlements
To be able to use Apple's platform and framework for Authenticating with an Apple ID, we need to set up our Xcode project.
1. We need to add an entry to the entitlements file to define the accessibility level for the plugin.
<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/EntitlementsDetail.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/EntitlementsDetail.png" alt="ImportPlugin"/></a>
</p>
2. We need to import the `AuthenticationServices.framework` library in the Build Phases->Link Binary with Libraries.
<p align="center">
    <a href="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/FrameworksDetail.png"><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/FrameworksDetail.png" alt="ImportPlugin" height=100/></a>
</p>

However, this can be cumbersome to do every time a new build is generated. That's why this plugin **provides an extension method** for 
`ProjectCapabilityManager`, used to add this entitlement programatically. Simply call the method `AddSignInWithApple`.

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

**NOTE:** The `AuthenticationServices.framework` should be added as Optional, to support previous iOS versions and avoid crashes on startup.

**NOTE 2:** The provided extension method uses reflection to integrate with the current tools Unity provides. If it fails on your particular Unity version, feel free to open a ticket specifying the Unity version.

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
