using AppleAuth.IOS;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Interfaces;
using AppleAuth.IOS.NativeMessages;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private const string AppleUserIdKey = "AppleUserId";
    
    private IAppleAuthManager _appleAuthManager;
    private OnDemandMessageHandlerScheduler _scheduler;
    
    public LoginMenuHandler LoginMenu;
    public GameMenuHandler GameMenu;

    private void Start()
    {
        // Creates the Scheduler to execute the pending callbacks on demand
        this._scheduler = new OnDemandMessageHandlerScheduler();
        // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
        var deserializer = new PayloadDeserializer();
        // Creates an Apple Authentication manager with the scheduler and the deserializer
        this._appleAuthManager = new AppleAuthManager(deserializer, this._scheduler);

        this.SetupLoginMenu();
    }

    private void Update()
    {
        // Updates the scheduler to execute pending response callbacks
        // This ensures they are executed inside Unity's Update loop
        this._scheduler.Update();
        
        this.LoginMenu.UpdateLoadingMessage(Time.deltaTime);
    }

    public void SignInWithAppleButtonPressed()
    {
        this.SetupLoginMenuForAppleSignIn();
        this.SignInWithApple();
    }

    public void LogoutButtonPressed()
    {
        this.DeleteCredentials();
        this.SetupLoginMenu();
    }

    private void SetupLoginMenu()
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
            PlayerPrefs.DeleteKey(AppleUserIdKey);
            this.SetupLoginMenu();
        });

        // If we have stored a Apple User Id, attempt to get the credentials for it first
        if (PlayerPrefs.HasKey(AppleUserIdKey))
        {
            var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
            this.SetupLoginMenuForCheckingCredentials();
            this.CheckCredentialStatusForUserId(storedAppleUserId);
        }
        // If we do not have a user ID, we just show the button to Sign In with Apple
        else
        {
            this.SetupLoginMenuForSignInWithApple();
        }
    }

    private void SetupLoginMenuForUnsupportedPlatform()
    {
        this.LoginMenu.SetSignInWithAppleButton(visible: false, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Unsupported platform");
    }
    
    private void SetupLoginMenuForSignInWithApple()
    {
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: true);
        this.LoginMenu.SetLoadingMessage(visible: false, message: string.Empty);
    }
    
    private void SetupLoginMenuForCheckingCredentials()
    {
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Checking Apple Credentials");
    }
    
    private void SetupLoginMenuForQuickLoginAttempt()
    {
        this.LoginMenu.SetSignInWithAppleButton(visible: true, enabled: false);
        this.LoginMenu.SetLoadingMessage(visible: true, message: "Attempting Quick Login");
    }
    
    private void SetupLoginMenuForAppleSignIn()
    {
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
        this._appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    case CredentialState.Authorized:
                        this.SetupGameMenu(appleUserId, null);
                        return;
                    
                    case CredentialState.Revoked:
                        this.SetupLoginMenuForQuickLoginAttempt();
                        this.AttemptQuickLogin(appleUserId);
                        return;
                    
                    case CredentialState.NotFound:
                        this.SetupLoginMenuForSignInWithApple();
                        this.DeleteCredentials();
                        return;
                }
            },
            error =>
            {
                Debug.LogWarning("Error while trying to get credential state " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }
    
    private void AttemptQuickLogin(string appleUserId)
    {
        this._appleAuthManager.QuickLogin(
            credential =>
            {
                this.SetupGameMenu(appleUserId, credential);
            },
            error =>
            {
                Debug.LogWarning("Quick Login Failed " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }
    
    private void SignInWithApple()
    {
        this._appleAuthManager.LoginWithAppleId(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            credential =>
            {
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                this.SetupGameMenu(credential.User, credential);
            },
            error =>
            {
                Debug.LogWarning("Sign in with Apple failed " + error.ToString());
                this.SetupLoginMenuForSignInWithApple();
            });
    }

    private void DeleteCredentials()
    {
        PlayerPrefs.DeleteKey(AppleUserIdKey);
    }
}
