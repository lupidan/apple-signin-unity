#if UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS

using System;
using System.Reflection;
using UnityEditor.iOS.Xcode;

namespace AppleAuth.Editor
{
    public static class ProjectCapabilityManagerExtension
    {
        /// <summary>
        /// Extension method for ProjectCapabilityManager to add the Sign In With Apple capability in compatibility mode.
        /// In particular, adds the AuthenticationServices.framework as an Optional framework, preventing crashes in
        /// iOS versions previous to 13.0
        /// </summary>
        /// <param name="manager">The manager for the main target to use when adding the Sign In With Apple capability.</param>
        public static void AddSignInWithAppleWithCompatibility(this ProjectCapabilityManager manager)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var managerType = typeof(ProjectCapabilityManager);
            
            var projectField = managerType.GetField("project", bindingFlags);
            var entitlementFilePathField = managerType.GetField("m_EntitlementFilePath", bindingFlags);
            var targetGuidField = managerType.GetField("m_TargetGuid", bindingFlags);
            var getOrCreateEntitlementDocMethod = managerType.GetMethod("GetOrCreateEntitlementDoc", bindingFlags);
            if (projectField == null ||
                entitlementFilePathField == null ||
                targetGuidField == null ||
                getOrCreateEntitlementDocMethod == null)
                throw new Exception("Can't Add Sign In With Apple programatically in this Unity version.");
            
            var entitlementFilePath = entitlementFilePathField.GetValue(manager) as string;
            var entitlementDoc = (PlistDocument) getOrCreateEntitlementDocMethod.Invoke(manager, new object[] { });
            if (entitlementDoc != null)
            {
                var plistArray = new PlistElementArray();
                plistArray.AddString("Default");
                entitlementDoc.root["com.apple.developer.applesignin"] = plistArray;
            }

            var project = (PBXProject) projectField.GetValue(manager);
            var emptyCapability = GetEmptyCapabilityWithReflection();
            
            var mainTargetGuid = (string)targetGuidField.GetValue(manager);
#if UNITY_2019_3_OR_NEWER
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
#else
            var frameworkTargetGuid = mainTargetGuid;
#endif
            
            project.AddFrameworkToProject(frameworkTargetGuid, "AuthenticationServices.framework", true);
            project.AddCapability(mainTargetGuid, emptyCapability, entitlementFilePath);
        }
        
        private static PBXCapabilityType GetEmptyCapabilityWithReflection()
        {
            // For Unity version >= 6000.0.23f1
            var constructorInfo = typeof(PBXCapabilityType)
                .GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                    null, 
                    new[] {typeof(bool), typeof(string), typeof(bool)}, 
                    null);

            if (constructorInfo != null)
            {
                return (PBXCapabilityType) constructorInfo
                    .Invoke(new object[] {true, string.Empty, true});
            }
            
            // For Unity version < 6000.0.23f1
            constructorInfo = typeof(PBXCapabilityType)
                .GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                    null, 
                    new[] {typeof(string), typeof(bool), typeof(string), typeof(bool)}, 
                    null);

            if (constructorInfo != null)
            {
                return (PBXCapabilityType) constructorInfo
                    .Invoke(new object[] {"com.lupidan.apple-signin-unity.empty", true, string.Empty, true});
            }

            throw new Exception("Can't create empty capability in this Unity version.");
        }
    }
}

#endif
