<p align="center">
  <img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/SignInWithApple.png" alt="Sign in With Apple"/><img src="https://raw.githubusercontent.com/lupidan/apple-signin-unity/master/Img/UnityIcon.png" alt="Unity 3D"/>
</p>
</div>

# Sign in with Apple Unity Plugin
This plugin is still WIP

## Overview
Sign in with Apple plugin to use with Unity 3D game engine.

The main purpose for this plugin is to expose iOS newest feature, Sign in with Apple, to the Unity game engine.

On WWDC19, Apple announced **Sign in with Apple**, and on top of that, they announced that every iOS Application
that used any kind of Third party sign-ins (like *Sign in with Facebook*, or *Sign in with Google*), will have to support
Sign in with Apple in order to get approved for the App Store, making it **mandatory**.

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
- ☐ Programatically add new AuthenticationServices.framework and Entitlements entry when building for iOS.
- ☐ NSPersonNameComponents formatting for all different styles.
- ☐ NSError codes mapping into Unity.
- ☐ Customize Sign in With Apple call from Unity. (request email and/or full name)
- ☐ Support to schedule all callbacks in user-configured loops (ex. in an MonoBehaviour's Update loop).

### Rest API
- ☐ TBD
