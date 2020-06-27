# Working with Firebase

> ⚠️ This is an initial version. Feel free to comment or suggest modifications.

***

## Before you begin
### Add Firebase to your Unity Project
https://firebase.google.com/docs/unity/setup

### Read "Before you begin" Firebase guide to support Sign In With Apple
https://firebase.google.com/docs/auth/ios/apple#before-you-begin

### Add the Sign in with Apple Unity Plugin to your Unity Project
https://github.com/lupidan/apple-signin-unity#installation


***

## Step by step guide

### Step 1: Read how to implement a default Sign In With Apple integration
We will adapt the code in step 4.

https://github.com/lupidan/apple-signin-unity#implement-sign-in-with-apple

### Step 2: Generating a random RAW Nonce
We need to generate some **random string to send to Firebase**, and check that the credentials we received from Apple are correct. Firebase guide recommends this method to generate one:

https://auth0.com/docs/api-auth/tutorials/nonce#generate-a-cryptographically-random-nonce

This is my **personal** take on a C# version of it:
```csharp
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

private static string GenerateRandomString(int length)
{
    if (length <= 0)
    {
        throw new Exception("Expected nonce to have positive length");
    }

    const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
    var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
    var result = string.Empty;
    var remainingLength = length;

    var randomNumberHolder = new byte[1];
    while (remainingLength > 0)
    {
        var randomNumbers = new List<int>(16);
        for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
        {
            cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
            randomNumbers.Add(randomNumberHolder[0]);
        }

        for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
        {
            if (remainingLength == 0)
            {
                break;
            }

            var randomNumber = randomNumbers[randomNumberIndex];
            if (randomNumber < charset.Length)
            {
                result += charset[randomNumber];
                remainingLength--;
            }
        }
    }

    return result;
}
```

### Step 3: Generate the SHA256 of the RAW Nonce

This is the nonce we will **send to Apple** when attempting a Login. We will generate the SHA256 hash of the Raw nonce we generated in Step 2.

```csharp
using System.Security.Cryptography;
using System.Text;

private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
{
    var sha = new SHA256Managed();
    var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
    var hash = sha.ComputeHash(utf8RawNonce);

    var result = string.Empty;
    for (var i = 0; i < hash.Length; i++)
    {
        result += hash[i].ToString("x2");
    }

    return result;
}
```

### Step 4: Assemble all the pieces

Once we have all the pieces ready, we can assenble everything toghether.
Adapting our current `LoginWithAppleId` and `QuickLogin`calls, and performing a common authentication with Firebase, receiving, finally, a `FirebaseUser` if everything goes well.

#### Adapted Login With Apple Id call
```csharp
using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using Firebase.Auth;

public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
{
    var rawNonce = GenerateRandomString(32);
    var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

    var loginArgs = new AppleAuthLoginArgs(
        LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
        nonce);

    this.appleAuthManager.LoginWithAppleId(
        loginArgs,
        credential =>
        {
            var appleIdCredential = credential as IAppleIDCredential;
            if (appleIdCredential != null)
            {
                this.PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
            }
        },
        error =>
        {
            // Something went wrong
        });
}
```

#### Adapted Quick Login call
```csharp
using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using Firebase.Auth;

public void PerformQuickLoginWithFirebase(Action<FirebaseUser> firebaseAuthCallback)
{
    var rawNonce = GenerateRandomString(32);
    var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

    var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce);

    this.appleAuthManager.QuickLogin(
        quickLoginArgs,
        credential =>
        {
            var appleIdCredential = credential as IAppleIDCredential;
            if (appleIdCredential != null)
            {
                this.PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
            }
        },
        error =>
        {
            // Something went wrong
        });
}
```

#### Common code to authenticate with Firebase
```csharp
using System;
using System.Text;
using AppleAuth.Interfaces;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

// Your Firebase authentication client
private FirebaseAuth firebaseAuth;

private void PerformFirebaseAuthentication(
    IAppleIDCredential appleIdCredential,
    string rawNonce,
    Action<FirebaseUser> firebaseAuthCallback)
{
    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
    var firebaseCredential = OAuthProvider.GetCredential(
        "apple.com",
        identityToken,
        rawNonce,
        authorizationCode);

    this.firebaseAuth.SignInWithCredentialAsync(firebaseCredential)
        .ContinueWithOnMainThread(task => HandleSignInWithUser(task, firebaseAuthCallback));
}

private static void HandleSignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
{
    if (task.IsCanceled)
    {
        Debug.Log("Firebase auth was canceled");
        firebaseUserCallback(null);
    }
    else if (task.IsFaulted)
    {
        Debug.Log("Firebase auth failed");
        firebaseUserCallback(null);
    }
    else
    {
        var firebaseUser = task.Result;
        Debug.Log("Firebase auth completed | User ID:" + firebaseUser.UserId);
        firebaseUserCallback(firebaseUser);
    }
}
```
