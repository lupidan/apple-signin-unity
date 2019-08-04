using System.Text;
using AppleAuth.IOS;
using AppleAuth.IOS.Enums;
using AppleAuth.IOS.Extensions;
using AppleAuth.IOS.Interfaces;
using AppleAuth.IOS.NativeMessages;
using UnityEngine;
using UnityEngine.UI;

public class MainTestMenu : MonoBehaviour
{
    private const string AppleUserIdKey = "AppleUserId";
    
    private IAppleAuthManager _appleAuth;
    private OnDemandMessageHandlerScheduler _scheduler;

    [SerializeField]
    private Button _mainButton = null;
    
    [SerializeField]
    private Text _mainButtonLabel = null;
    
    [SerializeField]
    private Text _credentialDetailsLabel = null;
    
    private void Start()
    {
        this._scheduler = new OnDemandMessageHandlerScheduler();
        this._appleAuth = new AppleAuthManager(new PayloadDeserializer(), this._scheduler);
        
        this._appleAuth.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log($"Received revoked callback {result}");
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        });
        
        this.CheckUpCredentials();
    }

    private void Update()
    {
        this._scheduler.Update();
    }

    private void CheckUpCredentials()
    {
        this._credentialDetailsLabel.text = "";
        if (!this._appleAuth.IsCurrentPlatformSupported)
        {
            this.SetupUnsupportedPlatform();
            return;
        }

        if (!PlayerPrefs.HasKey(AppleUserIdKey))
        {
            this.SetupSignInWithAppleButton();
            return;
        }

        var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
        this._mainButton.enabled = false;
        this._mainButtonLabel.text = "Checking credentials...";
        this._appleAuth.GetCredentialState(
            storedAppleUserId,
            state =>
            {
                if (state == CredentialState.Authorized)
                    this.SignInSilently();
                else
                    this.SetupSignInWithAppleButton();
            },
            error =>
            {
                this._credentialDetailsLabel.text = "Couldn't get Credential state for stored user ID: " + error.LocalizedDescription;
                this.SetupSignInWithAppleButton();
            });
    }

    private void SetupUnsupportedPlatform()
    {
        this._mainButton.enabled = false;
        this._mainButtonLabel.text = "Unsupported platform";
        this._credentialDetailsLabel.text = "<b>This platform is not supported. Try running the scene in a real device or a simulator</b>";
    }

    private void SetupSignInWithAppleButton()
    {
        this._mainButton.enabled = true;
        this._mainButtonLabel.text = "Sign in with Apple";
        this._mainButton.onClick.RemoveAllListeners();
        this._mainButton.onClick.AddListener(this.SignInWithApple);
    }
    
    private void SetupSignedInWithCredential(ICredential credential)
    {
        this._mainButton.enabled = true;
        this._mainButtonLabel.text = "Delete credentials";
        this._mainButton.onClick.RemoveAllListeners();
        this._mainButton.onClick.AddListener(this.DeleteCredentials);
        
        var appleIdCredential = credential as IAppleIDCredential;
        var passwordCredential = credential as IPasswordCredential;
        if (appleIdCredential != null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("APPLE ID CREDENTIAL RECEIVED");
            stringBuilder.AppendLine("<b>Username:</b> " + appleIdCredential.User);
            stringBuilder.AppendLine("<b>Real user status:</b> " + appleIdCredential.RealUserStatus.ToString());

            if (appleIdCredential.State != null)
                stringBuilder.AppendLine("<b>State:</b> " + appleIdCredential.State);
            
            if (appleIdCredential.IdentityToken != null)
                stringBuilder.AppendLine("<b>Identity token:</b> " + appleIdCredential.IdentityToken.Length + " bytes");
            
            if (appleIdCredential.AuthorizationCode != null)
                stringBuilder.AppendLine("<b>Authorization Code:</b> " + appleIdCredential.AuthorizationCode.Length + " bytes");
            
            if (appleIdCredential.AuthorizedScopes != null)
                stringBuilder.AppendLine("<b>Authorized Scopes:</b> " + string.Join(", ", appleIdCredential.AuthorizedScopes));
            
            if (appleIdCredential.Email != null)
                stringBuilder.AppendLine("<b>Email:</b> " + appleIdCredential.Email);

            if (appleIdCredential.FullName != null)
            {
                var fullName = appleIdCredential.FullName;
                stringBuilder.AppendLine("<b>Name:</b> " + fullName.ToLocalizedString());
                stringBuilder.AppendLine("<b>Name (Short):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Short));
                stringBuilder.AppendLine("<b>Name (Medium):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                stringBuilder.AppendLine("<b>Name (Long):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Long));
                stringBuilder.AppendLine("<b>Name (Abbreviated):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));
                
                if (appleIdCredential.FullName.PhoneticRepresentation != null)
                {
                    var phoneticName = appleIdCredential.FullName.PhoneticRepresentation;
                    stringBuilder.AppendLine("<b>Phonetic name:</b> " + phoneticName.ToLocalizedString());
                    stringBuilder.AppendLine("<b>Phonetic name (Short):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Short));
                    stringBuilder.AppendLine("<b>Phonetic name (Medium):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Medium));
                    stringBuilder.AppendLine("<b>Phonetic name (Long):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Long));
                    stringBuilder.AppendLine("<b>Phonetic name (Abbreviated):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));
                }
            }

            this._credentialDetailsLabel.text = stringBuilder.ToString();
        }
        else if (passwordCredential != null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("USERNAME/PASSWORD RECEIVED (iCloud?)");
            stringBuilder.AppendLine("<b>Username:</b> " + passwordCredential.User);
            stringBuilder.AppendLine("<b>Password:</b> " + passwordCredential.Password);
            this._credentialDetailsLabel.text = stringBuilder.ToString();
        }
        else
        {
            this._credentialDetailsLabel.text = "Unknown credentials for user "+ credential.User;    
        }
    }

    private void SignInWithApple()
    {
        this._mainButton.enabled = false;
        this._mainButtonLabel.text = "Signing in...";
        this._appleAuth.LoginWithAppleId(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            credential =>
            {
                Debug.Log("Sign In with Apple was successful");
                PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                this.SetupSignedInWithCredential(credential);
            },
            error =>
            {
                this._credentialDetailsLabel.text = "Sign In With Apple failed: " + error.LocalizedDescription;
                SetupSignInWithAppleButton();
            });
    }

    private void DeleteCredentials()
    {
        PlayerPrefs.DeleteKey(AppleUserIdKey);
        SetupSignInWithAppleButton();
    }

    private void SignInSilently()
    {
        this._mainButton.enabled = false;
        this._mainButtonLabel.text = "Signing in silently..."; 
        this._appleAuth.LoginSilently(
            credential =>
            {
                Debug.Log("Silent sign in was successful");
                this.SetupSignedInWithCredential(credential);
            },
            error =>
            {
                this._credentialDetailsLabel.text = "Silent Sign In With Apple failed: " + error.LocalizedDescription;
                SetupSignInWithAppleButton();
            });
    }
}
