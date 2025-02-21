#if UNITY_EDITOR
using System.IO;
using BNB;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class PreBuildManager : IPreprocessBuildWithReport
{
    private ResourcesJSON _files;

    public int callbackOrder => 0;
    private readonly string _streamingAssetsPath = Application.streamingAssetsPath + "/BanubaFaceAR/";

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        // Do the preprocessing here
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        var resourceDir = Application.dataPath + "/Resources";
        if (!Directory.Exists(resourceDir)) {
            Directory.CreateDirectory(resourceDir);
        }
        _files = new ResourcesJSON();

        var file = resourceDir + "/BNBResourceList.json";
        ProcessDirectory(_streamingAssetsPath);

        File.WriteAllText(file, _files.SaveToString());

        AddAlwaysIncludedShader("Hidden/BlurEffectConeTap");
        AddAlwaysIncludedShader("BNB/CameraStencilMask");
    }

    private void AddAlwaysIncludedShader(string shaderName)
    {
        var shader = Shader.Find(shaderName);
        if (shader == null) {
            return;
        }

        var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
        bool hasShader = false;
        for (int i = 0; i < arrayProp.arraySize; ++i) {
            var arrayElem = arrayProp.GetArrayElementAtIndex(i);
            if (shader == arrayElem.objectReferenceValue) {
                hasShader = true;
                break;
            }
        }

        if (!hasShader) {
            int arrayIndex = arrayProp.arraySize;
            arrayProp.InsertArrayElementAtIndex(arrayIndex);
            var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
            arrayElem.objectReferenceValue = shader;

            serializedObject.ApplyModifiedProperties();

            AssetDatabase.SaveAssets();
        }
    }

    private void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries) {
            var fixedFileName = fileName.Replace(@"\\", @"/").Replace(@"\", @"/");
            ProcessFile(fixedFileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries) {
            ProcessDirectory(subdirectory);
        }
    }

    private void ProcessFile(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden
            || Path.GetExtension(path) == ".meta") {
            return;
        }
        _files.resources.Add(path.Substring(_streamingAssetsPath.Length));
    }
}
#endif
