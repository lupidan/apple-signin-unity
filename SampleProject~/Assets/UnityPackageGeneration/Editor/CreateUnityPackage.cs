using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace UnityPackageGeneration.Editor
{
    public static class CreateUnityPackage
    {
        private class FileCopyEntry
        {
            public readonly string OriginalFilepath;
            public readonly string DestinationFilepath;

            public FileCopyEntry(string originalFilepath, string destinationFilepath)
            {
                OriginalFilepath = originalFilepath;
                DestinationFilepath = destinationFilepath;
            }
        }
        
        [MenuItem("Tools/Generate Apple Auth Unity Package")]
        public static void GenerateUnityPackage()
        {
            var assetsFolderPath = Directory.GetCurrentDirectory() + "/Assets/";
            var rootFolderPath = Directory.GetCurrentDirectory() + "/../";
            var packageFolderPath = Directory.GetCurrentDirectory() + "/../AppleAuth/";
            
            var rootFolder = new Uri(rootFolderPath, UriKind.Absolute);
            var packageFolder = new Uri(packageFolderPath, UriKind.Absolute);

            var allFiles = Directory.GetFiles(
                packageFolder.AbsolutePath,
                "*",
                SearchOption.AllDirectories);
            
            var pluginFiles = allFiles
                .Where(absoluteFilepath => !absoluteFilepath.EndsWith(".meta"))
                .Select(absoluteFilepath =>
                {
                    var originalFileUri = new Uri(absoluteFilepath);
                    var relativeUri = rootFolder.MakeRelativeUri(originalFileUri);
                    var destinationFileUri = new Uri(
                        assetsFolderPath + relativeUri.ToString(),
                        UriKind.Absolute);
                    
                    return new FileCopyEntry(absoluteFilepath, destinationFileUri.AbsolutePath);
                })
                .ToList();
                
            pluginFiles
                .ForEach(entry =>
                {
                    var directory = Path.GetDirectoryName(entry.DestinationFilepath);
                    if (directory != null && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    FileUtil.CopyFileOrDirectory(entry.OriginalFilepath, entry.DestinationFilepath);
                });
            
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ExportPackage(
                new [] {"Assets/AppleAuth", "Assets/AppleAuthSample"},
                "AppleSignInUnity.unitypackage", 
                ExportPackageOptions.Recurse);

            Directory.Delete(assetsFolderPath + "/AppleAuth/", true);
            FileUtil.DeleteFileOrDirectory(assetsFolderPath + "/AppleAuth.meta");
            AssetDatabase.Refresh();
        }
    }
}
