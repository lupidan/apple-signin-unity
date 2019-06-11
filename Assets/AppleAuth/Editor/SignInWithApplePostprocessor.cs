using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace AppleAuth.Editor
{
    public class SignInWithApplePostprocessor
    {
        private const int CallOrder = 1;

        [PostProcessBuild(CallOrder)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            var projectPath = PBXProject.GetPBXProjectPath(path);
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
            manager.AddSignInWithApple();
            manager.WriteToFile();
        }
    }
    
}