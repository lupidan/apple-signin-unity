using UnityEditor;

namespace UnityPackageGeneration.Editor
{
    public static class CreateUnityPackage
    {
        [MenuItem("Tools/Generate Apple Auth Unity Package")]
        public static void GenerateUnityPackage()
        {
            AssetDatabase.ExportPackage(
                new [] {"Assets/AppleAuth", "Assets/AppleAuthSample"},
                "AppleSignInUnity.unitypackage", 
                ExportPackageOptions.Recurse);
        }
    }
}
