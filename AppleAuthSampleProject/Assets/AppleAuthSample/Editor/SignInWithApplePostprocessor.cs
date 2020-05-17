#if UNITY_IOS || UNITY_TVOS

using AppleAuth.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace AppleAuthSample.Editor
{
    public static class SignInWithApplePostprocessor
    {
        private const int CallOrder = 1;

        [PostProcessBuild(CallOrder)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS && target != BuildTarget.tvOS)
                return;
            
            var projectPath = PBXProject.GetPBXProjectPath(path);
#if UNITY_2019_3_OR_NEWER
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
            manager.WriteToFile();
#else
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
            manager.AddSignInWithAppleWithCompatibility();
            manager.WriteToFile();
#endif
        }
    }
}

#endif
