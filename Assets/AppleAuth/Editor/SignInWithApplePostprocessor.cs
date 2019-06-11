using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace AppleAuth.Editor
{
    public class SignInWithApplePostprocessor
    {
        private const int CallOrder = 1;
        private const string RequiredFramework = "AuthenticationServices.framework";
        private const string EntitlementsArrayKey = "com.apple.developer.applesignin";
        private const string DefaultAccessLevel = "Default";

        [PostProcessBuild(CallOrder)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);
            var targetName = PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName(targetName);
            
            // Add new required framework
            project.AddFrameworkToProject(targetGuid, RequiredFramework, true);
            
            // Add entry to default entitlements file
            AddAppleSignInEntitlement(targetGuid, project, path);
            
            project.WriteToFile(projectPath);
        }

        private static void AddAppleSignInEntitlement(string targetGuid, PBXProject project, string path)
        {
            var relativePath = project.GetBuildPropertyForConfig(targetGuid, "CODE_SIGN_ENTITLEMENTS");
            var shouldSetEntitlementsBuildProperty = false;
            if (string.IsNullOrEmpty(relativePath))
            {
                relativePath = Path.Combine("./", PBXProject.GetUnityTargetName() + ".entitlements");
                shouldSetEntitlementsBuildProperty = true;
            }
            
            var absolutePath = Path.Combine(path, relativePath);
            var filename = Path.GetFileName(absolutePath);
            
            var entitlements = new PlistDocument();
            if (File.Exists(absolutePath))
                entitlements.ReadFromFile(absolutePath);
            
            var plistArray = new PlistElementArray();
            plistArray.AddString(DefaultAccessLevel);
            entitlements.root[EntitlementsArrayKey] = plistArray;
            entitlements.WriteToFile(absolutePath);
            
            if (shouldSetEntitlementsBuildProperty)
                project.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativePath);

            if (project.FindFileGuidByProjectPath(relativePath) == null)
                project.AddFile(absolutePath, filename);
        }
        
    }
    
}