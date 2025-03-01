using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AppleAuthSample.UnityPackageGeneration.Editor
{
    public static class CreateUnityPackage
    {
        [MenuItem("Tools/Generate Apple Auth Unity Package")]
        public static void GenerateUnityPackage()
        {
            var originalFolder = Path.Combine("Packages", "com.lupidan.apple-signin-unity");
            var destinationFolder = Path.Combine("Assets", "AppleAuth");
            var assetsToMove = new[] {"docs", "Editor", "Runtime"};
            var unityPackagePath = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "AppleSignInUnity.unitypackage"));

            using (PackageAssetMover.MoveFiles(originalFolder, destinationFolder, assetsToMove))
            {
                var assetPathNames = new[]
                {
                    Path.Combine("Assets", "AppleAuth"),
                    Path.Combine("Assets", "AppleAuthSample")
                };

                
                var flags = ExportPackageOptions.Recurse;
                
                AssetDatabase.ExportPackage(
                    assetPathNames,
                    unityPackagePath, 
                    flags);
            }

            var deleted = AssetDatabase.DeleteAsset(destinationFolder);
            if (!deleted)
            {
                throw new Exception($"[{nameof(CreateUnityPackage)}] Couldn't delete {destinationFolder}");
            }

            EditorUtility.DisplayDialog(
                "Unity package generated",
                $"Unity package generated successfully at {unityPackagePath}.",
                "OK");
        }
    }
}
