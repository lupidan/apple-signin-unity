#if UNITY_IOS || UNITY_TVOS

using System;
using System.Reflection;
using UnityEditor.iOS.Xcode;

namespace AppleAuth.Editor
{
    public static class ProjectCapabilityManagerExtension
    {
        private const string EntitlementsArrayKey = "com.apple.developer.applesignin";
        private const string DefaultAccessLevel = "Default";
        private const string AuthenticationServicesFramework = "AuthenticationServices.framework";
        private const BindingFlags NonPublicInstanceBinding = BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// Extension method for ProjectCapabilityManager to add the Sign In With Apple capability in compatibility mode.
        /// In particular, adds the AuthenticationServices.framework as an Optional framework, preventing crashes in
        /// iOS versions previous to 13.0
        /// </summary>
        /// <param name="manager">The manager to use when adding the Sign In With Apple capability.</param>
        public static void AddSignInWithAppleWithCompatibility(this ProjectCapabilityManager manager)
        {
            var managerType = typeof(ProjectCapabilityManager);
            var capabilityTypeType = typeof(PBXCapabilityType);
            
            var projectField = managerType.GetField("project", NonPublicInstanceBinding);
            var targetGuidField = managerType.GetField("m_TargetGuid", NonPublicInstanceBinding);
            var entitlementFilePathField = managerType.GetField("m_EntitlementFilePath", NonPublicInstanceBinding);
            var getOrCreateEntitlementDocMethod = managerType.GetMethod("GetOrCreateEntitlementDoc", NonPublicInstanceBinding);
            var constructorInfo = capabilityTypeType.GetConstructor(
                NonPublicInstanceBinding, 
                null,
                new[] {typeof(string), typeof(bool), typeof(string), typeof(bool)}, 
                null);
            
            if (projectField == null || targetGuidField == null  || entitlementFilePathField == null ||
                getOrCreateEntitlementDocMethod == null || constructorInfo == null)
                throw new Exception("Can't Add Sign In With Apple programatically in this Unity version");
            
            var entitlementFilePath = entitlementFilePathField.GetValue(manager) as string;
            var entitlementDoc = getOrCreateEntitlementDocMethod.Invoke(manager, new object[] { }) as PlistDocument;
            if (entitlementDoc != null)
            {
                var plistArray = new PlistElementArray();
                plistArray.AddString(DefaultAccessLevel);
                entitlementDoc.root[EntitlementsArrayKey] = plistArray;
            }

            var project = projectField.GetValue(manager) as PBXProject;
            if (project != null)
            {
                var targetGuid = targetGuidField.GetValue(manager) as string;
                var capabilityType = constructorInfo.Invoke(new object[] { "com.apple.signin", true, AuthenticationServicesFramework, true }) as PBXCapabilityType;
                project.AddCapability(targetGuid, capabilityType, entitlementFilePath, false);
                project.AddFrameworkToProject(targetGuid, AuthenticationServicesFramework, true);
            }
        }
    }
}

#endif
