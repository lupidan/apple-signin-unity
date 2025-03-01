using System;
using System.IO;
using UnityEditor;

namespace UnityPackageGeneration.Editor
{
    internal class PackageAssetMover
    {
        public static IDisposable MoveFiles(
            string originalFolder,
            string destinationFolder,
            params string[] assetsToMove)
        {
            var packageAssetMover = new PackageAssetMover(originalFolder, destinationFolder, assetsToMove);
            packageAssetMover.MoveToDestination();
            return new ActionDisposable(() => packageAssetMover.MoveBackToOrigin());
        }

        private readonly string _originalFolder;
        private readonly string _destinationFolder;
        private readonly string[] _assetsToMove;

        private PackageAssetMover(
            string originalFolder,
            string destinationFolder,
            params string[] assetsToMove)
        {
            _originalFolder = originalFolder;
            _destinationFolder = destinationFolder;
            _assetsToMove = assetsToMove;
        }

        private void MoveToDestination() => Move(_originalFolder, _destinationFolder);
        private void MoveBackToOrigin() => Move(_destinationFolder, _originalFolder);

        private void Move(string originalFolder, string destinationFolder)
        {
            if (!AssetDatabase.IsValidFolder(destinationFolder))
            {
                var parentFolder = Path.GetDirectoryName(destinationFolder);
                if (string.IsNullOrEmpty(parentFolder))
                {
                    throw new Exception(
                        $"[{nameof(CreateUnityPackage)}] Can't determine parent folder from {destinationFolder}");
                }

                var newFolderName = Path.GetFileName(destinationFolder);
                if (string.IsNullOrEmpty(newFolderName))
                {
                    throw new Exception(
                        $"[{nameof(CreateUnityPackage)}] Can't determine folder name from {destinationFolder}");
                }

                var directoryGuid = AssetDatabase.CreateFolder(
                    parentFolder,
                    newFolderName);

                if (string.IsNullOrEmpty(directoryGuid))
                {
                    throw new Exception($"[{nameof(CreateUnityPackage)}] Couldn't create {destinationFolder}");
                }
            }

            foreach (var assetToMove in _assetsToMove)
            {
                var error = AssetDatabase.MoveAsset(
                    Path.Combine(originalFolder, assetToMove),
                    Path.Combine(destinationFolder, assetToMove));

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception(
                        $"[{nameof(CreateUnityPackage)}] Couldn't move {assetToMove} from {originalFolder} to {destinationFolder} due to:\n{error}");
                }
            }
        }
    }
}
