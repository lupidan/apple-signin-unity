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
                throw new Exception(GetMessage("FixManagerBundleIdentifier should only be called when building for macOS"));
            }

            try
            {
                var macosAppleAuthManagerInfoPlistPath = GetInfoPlistPath(path);
                var macosAppleAuthManagerInfoPlist = File.ReadAllText(macosAppleAuthManagerInfoPlistPath);
                var regex = new Regex(@"\<key\>CFBundleIdentifier\<\/key\>\s*\<string\>(com\.lupidan)\.MacOSAppleAuthManager\<\/string\>");
                var match = regex.Match(macosAppleAuthManagerInfoPlist);
                if (!match.Success)
                {
                    throw new Exception(GetMessage("Can't locate CFBundleIdentifier in MacOSAppleAuthManager's Info.plist"));
                }
                
                var modifiedMacosAppleAuthManagerInfoPlist = macosAppleAuthManagerInfoPlist
                        .Remove(match.Groups[1].Index, match.Groups[1].Length)
                        .Insert(match.Groups[1].Index, PlayerSettings.applicationIdentifier);

                File.WriteAllText(macosAppleAuthManagerInfoPlistPath, modifiedMacosAppleAuthManagerInfoPlist);
                Debug.Log(GetMessage($"Renamed MacOSAppleAuthManager.bundle bundle identifier from \"com.lupidan.MacOSAppleAuthManager\" -> \"{PlayerSettings.applicationIdentifier}.MacOSAppleAuthManager\""));
            }
            catch (Exception exception)
            {
                throw new Exception(GetMessage(
                    $"Error while fixing MacOSAppleAuthManager.bundle bundle identifier :: {exception.Message}"));
            }
        }

        private static string GetMessage(string message) => $"{nameof(AppleAuthMacosPostprocessorHelper)}: {message}";

        private static string GetInfoPlistPath(string path)
        {
            var bundleDirectories = Directory.GetDirectories(
                path,
                "MacOSAppleAuthManager.bundle",
                SearchOption.AllDirectories);

            if (bundleDirectories.Length == 0)
            {
                throw new Exception(GetMessage("Can't locate any MacOSAppleAuthManager.bundle"));
            }

            if (bundleDirectories.Length > 1)
            {
                var allPaths = string.Join("\n", bundleDirectories);
                throw new Exception(GetMessage($"Located multiple MacOSAppleAuthManager.bundle!\n{allPaths}"));
            }
            
            var bundlePath = bundleDirectories[0];
            var infoPlistPath = Path.Combine(
                bundlePath,
                "Contents",
                "Info.plist");

            if (!File.Exists(infoPlistPath))
            {
                throw new Exception(GetMessage("Can't locate MacOSAppleAuthManager's Info.plist"));
            }
            
            Debug.Log(GetMessage($"Located Info.plist at {infoPlistPath}"));
            
            return infoPlistPath;
        }
    }
}
