#if UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
#define UNITY_XCODE_EXTENSIONS_AVAILABLE
#endif

#if UNITY_2020_3
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_XCODE_EXTENSIONS_AVAILABLE
using UnityEditor.iOS.Xcode;
#endif

namespace AppleAuthSample.Editor
{
    public class Unity2020FixPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 100;
        
        public void OnPostprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;
            var path = report.summary.outputPath;

            if (target != BuildTarget.iOS && target != BuildTarget.tvOS)
            {
                return;
            }
            
            // Fix for Xcode 16
            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            project.UpdateBuildProperty(
                project.GetUnityMainTargetGuid(),
                "OTHER_CFLAGS",
                Array.Empty<string>(),
                new[] {"-mno-thumb"});
            
            project.WriteToFile(projectPath);
        }
    }
}
#endif
