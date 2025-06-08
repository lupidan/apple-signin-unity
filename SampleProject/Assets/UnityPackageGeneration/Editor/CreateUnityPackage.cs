﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AppleAuthSample.UnityPackageGeneration.Editor
{
    public static class CreateUnityPackage
    {
        private static string RootFolder => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        private static string PackageFolder => Path.GetFullPath(Path.Combine("Packages", "com.lupidan.apple-signin-unity"));
        
        [MenuItem("Tools/Upgrade Plugin Version")]
        public static void UpgradePluginVersion()
        {
            const string versionToUpdateTo = "1.5.0";
            
            var entries = new[]
            {
                (Filepath: Path.Combine(PackageFolder, "package.json"), Regex: new Regex(@"\""version\""\s*:\s*\""([0-9.]*)\""")),
                (Filepath: Path.Combine(PackageFolder, "README.md"), Regex: new Regex(@"Current stable version is (v?[0-9.]*)")),
                (Filepath: Path.Combine(PackageFolder, "README.md"), Regex: new Regex(@"apple-signin-unity\.git\?path=Source#(v?[0-9.]*)")),
                (Filepath: Path.Combine(RootFolder, ".github", "workflows", "fastlane-build.yml"), Regex: new Regex(@"SAMPLE_PROJECT_VERSION:\s*\""([0-9.]*)\""")),
                (Filepath: Path.Combine(PackageFolder, "CHANGELOG.md"), Regex: new Regex(@"## \[(Unreleased)\]")),
                (Filepath: Path.Combine(RootFolder, "SampleProject", "ProjectSettings", "ProjectSettings.asset"), Regex: new Regex(@"bundleVersion:\s*([0-9.]*)")),
                (Filepath: Path.Combine(RootFolder, "Xcode", "MacOSAppleAuthManager.xcodeproj", "project.pbxproj"), Regex: new Regex(@"MARKETING_VERSION = ([0-9.]*);")),
                (Filepath: Path.Combine(PackageFolder, "Runtime", "AppleAuthManager.cs"), Regex: new Regex(@"Using Sign in with Apple Unity Plugin - (v?[0-9.]*)")),
            };
            
            foreach (var entry in entries)
            {
                var fileContents = File.ReadAllText(entry.Filepath);
                var match = entry.Regex.Match(fileContents);
                do
                {
                    if (!match.Success)
                    {
                        throw new Exception(
                            $"[{nameof(CreateUnityPackage)}] Can't locate version to update in {entry.Filepath}");
                    }

                    fileContents = fileContents
                        .Remove(match.Groups[1].Index, match.Groups[1].Length)
                        .Insert(match.Groups[1].Index, versionToUpdateTo);

                    match = match.NextMatch();
                }
                while (match.Success);
                
                File.WriteAllText(entry.Filepath, fileContents);
            }
        }
        
        [MenuItem("Tools/Generate Apple Auth Unity Package")]
        public static void GenerateUnityPackage()
        {
            var originalFolder = PackageFolder;
            var destinationFolder = Path.Combine("Assets", "AppleAuth");
            var unityPackagePath = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "AppleSignInUnity.unitypackage"));
            
            if (Directory.Exists(destinationFolder))
                Directory.Delete(destinationFolder, true);
        
            FileUtil.CopyFileOrDirectoryFollowSymlinks(originalFolder, destinationFolder);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            var assetPathNames = new[]
            {
                Path.Combine("Assets", "AppleAuth"),
                Path.Combine("Assets", "AppleAuthSample")
            };
            
            AssetDatabase.ExportPackage(
                assetPathNames,
                unityPackagePath, 
                flags: ExportPackageOptions.Recurse);

            AssetDatabase.DeleteAsset(destinationFolder);

            EditorUtility.DisplayDialog(
                "Unity package generated",
                $"Unity package generated successfully at {unityPackagePath}.",
                "OK");
        }
    }
}
