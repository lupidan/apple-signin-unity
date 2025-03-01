using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AppleAuth.Editor
{
    public static class AppleAuthMacosPostprocessorHelper
    {
        /// <summary>
        /// Use this script to change the bundle identifier of the plugin's library bundle to replace it with a personalized one for your product.
        /// This should avoid CFBundleIdentifier Collision errors when uploading the app to the macOS App Store
        /// </summary>
        /// <remarks>Basically this should replace the plugin's bundle identifier from "com.lupidan.MacOSAppleAuthManager" to "{your.project.application.identifier}.MacOSAppleAuthManager"</remarks>
        /// <param name="target">The current build target, so it's only executed when building for MacOS</param>
        /// <param name="path">The path of the built .app file</param>
        public static void FixManagerBundleIdentifier(BuildTarget target, string path)
        {
            if (target != BuildTarget.StandaloneOSX)
            {
                Debug.LogError("AppleAuthMacosPostprocessorHelper: FixManagerBundleIdentifier should only be called when building for macOS");
                return;
            }

            const string bundleIdentifierPattern = @"(\<key\>CFBundleIdentifier\<\/key\>\s*\<string\>)(com\.lupidan)(\.MacOSAppleAuthManager\<\/string\>)";
            const string macOSAppleAuthManagerInfoPlistRelativePath = "/Contents/Plugins/MacOSAppleAuthManager.bundle/Contents/Info.plist";

            try
            {
                var macosAppleAuthManagerInfoPlistPath = path + macOSAppleAuthManagerInfoPlistRelativePath;
                var macosAppleAuthManagerInfoPlist = File.ReadAllText(macosAppleAuthManagerInfoPlistPath);
                var modifiedMacosAppleAuthManagerInfoPlist = Regex.Replace(
                    macosAppleAuthManagerInfoPlist,
                    bundleIdentifierPattern,
                    "$1" + PlayerSettings.applicationIdentifier + "$3");

                File.WriteAllText(macosAppleAuthManagerInfoPlistPath, modifiedMacosAppleAuthManagerInfoPlist);
                Debug.Log("AppleAuthMacosPostprocessorHelper: Renamed MacOSAppleAuthManager.bundle bundle identifier from \"com.lupidan.MacOSAppleAuthManager\" -> \"" + PlayerSettings.applicationIdentifier + ".MacOSAppleAuthManager\"");
            }
            catch (Exception exception)
            {
                Debug.LogError("AppleAuthMacosPostprocessorHelper: Error while fixing MacOSAppleAuthManager.bundle bundle identifier :: " + exception.Message);
            }
        }
    }
}
