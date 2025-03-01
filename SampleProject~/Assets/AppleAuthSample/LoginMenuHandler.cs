using System;
using UnityEngine;
using UnityEngine.UI;

namespace AppleAuthSample
{
    [Serializable]
    public class LoginMenuHandler
    {
        public GameObject Parent;
        public GameObject SignInWithAppleParent;
        public Button SignInWithAppleButton;
        public GameObject LoadingMessageParent;
        public Transform LoadingIconTransform;
        public Text LoadingMessageLabel;

        public void SetVisible(bool visible)
        {
            this.Parent.SetActive(visible);
        }

        public void SetLoadingMessage(bool visible,  string message)
        {
            this.LoadingMessageParent.SetActive(visible);
            this.LoadingMessageLabel.text = message;
        }

        public void SetSignInWithAppleButton(bool visible, bool enabled)
        {
            this.SignInWithAppleParent.SetActive(visible);
            this.SignInWithAppleButton.enabled = enabled;
        }

        public void UpdateLoadingMessage(float deltaTime)
        {
            if (!this.LoadingMessageParent.activeSelf)
                return;
        
            this.LoadingIconTransform.Rotate(0.0f, 0.0f, -360.0f * deltaTime);
        }
    }
}
