using UnityEditor;

public static class CreateUnityPackage
{
    [MenuItem("Tools/Generate Apple Auth Unity Package")]
    public static void GenerateUnityPackage()
    {
        AssetDatabase.ExportPackage("Assets/AppleAuth", "UnityAppleAuth.unitypackage", ExportPackageOptions.Recurse);
    }
}
