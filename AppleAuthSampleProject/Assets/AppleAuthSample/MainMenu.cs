using AppleAuth.IOS;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;
using AppleAuth.IOS.NativeMessages;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private const string AppleUserIdKey = "AppleUserId";
    
    private IAppleAuthManager _appleAuthManager;

    public LoginMenuHandler LoginMenu;
    public GameMenuHandler GameMenu;

    private void Start()
    {
        // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
        var deserializer = new PayloadDeserializer();
        // Creates an Apple Authentication manager with the deserializer
        this._appleAuthManager = new AppleAuthManager(deserializer);

        this.InitializeLoginMenu();
    }

    private void Update()
    {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        this._appleAuthManager.Update();
        
        this.LoginMenu.UpdateLoadingMessage(Time.deltaTime);
    }

    public void SignInWithAppleButtonPressed()
    {
        this.SetupLoginMenuForAppleSignIn();
        this.SignInWithApple();
    }

    private void InitializeLoginMenu()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        
        // Check if the current platform supports Sign In With Apple
        if (!this._appleAuthManager.IsCurrentPlatformSupported)
        {
            this.SetupLoginMenuForUnsupportedPlatform();
            return;
        }
        
        // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
        this._appleAuthManager.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log("Received revoked callback " + result);
            this.SetupLoginMenuForSignInWithApple();
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        });

        // If we have an Apple User Id available, get the credential status for it
        if (PlayerPrefs.HasKey(AppleUserIdKey))
        {
            var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
            this.SetupLoginMenuForCheckingCredentials();
            this.CheckCredentialStatusForUserId(storedAppleUserId);
        }
        // If we do not have an stored Apple User Id, attempt a quick login
        else
        {
            this.SetupLoginMenuForQuickLoginAttempt();
            this.AttemptQuickLogin();
        }
    }

    private void SetupLoginMenuForUnsupportedPlatform()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        this.LoginMenu.SetSignInWithAppleButton(visible: false, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Unsupported platform");
    }
    
    private void SetupLoginMenuForSignInWithApple()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: true);
        this.LoginMenu.SetLoadingMessage(visible: false, message: string.Empty);
    }
    
    private void SetupLoginMenuForCheckingCredentials()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Checking Apple Credentials");
    }
    
    private void SetupLoginMenuForQuickLoginAttempt()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Attempting Quick Login");
    }
    
    private void SetupLoginMenuForAppleSignIn()
    {
        this.LoginMenu.SetVisible(visible: true);
        this.GameMenu.SetVisible(visible: false);
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Signing In with Apple");
    }
    
    private void SetupGameMenu(string appleUserId, ICredential credential)
    {
        this.LoginMenu.SetVisible(visible: false);
        this.GameMenu.SetVisible(visible: true);
        this.GameMenu.SetupAppleData(appleUserId, credential);
    }

    private void CheckCredentialStatusForUserId(string appleUserId)
    {
        // If there is an apple ID available, we should check the credential state
        this._appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    // If it's authorized, login with that user id
                    case CredentialState.Authorized:
                        this.SetupGameMenu(appleUserId, null);
                        return;
                    
                    // If it was revoked, or not found, we need a new sign in with apple attempt
                    // Discard previous apple user id
                    case CredentialState.Revoked:
                    case CredentialState.NotFound:
                        this.SetupLoginMenuForSignInWithApple();
                        PlayerPrefs.DeleteKey(AppleUserIdKey);
                        return;
                }
            },
            error =>
            {
                Debug.LogWarning("Error while trying to get credential state " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }
    
    private void AttemptQuickLogin()
    {
        var quickLoginArgs = new AppleAuthQuickLoginArgs();
        
        // Quick login should succeed if the credential was authorized before and not revoked
        this._appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                // If it's an Apple credential, save the user ID, for later logins
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);    
                }

                this.SetupGameMenu(credential.User, credential);
            },
            error =>
            {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                Debug.LogWarning("Quick Login Failed " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }
    
    private void SignInWithApple()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        
        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                this.SetupGameMenu(credential.User, credential);
            },
            error =>
            {
                Debug.LogWarning("Sign in with Apple failed " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }
}
