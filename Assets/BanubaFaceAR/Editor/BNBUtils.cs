using UnityEngine;
using UnityEditor;

namespace BNB
{
    [InitializeOnLoad]
    public class AddBNBLayers
    {
        static AddBNBLayers()
        {
            string[] BNBLayers =
                {
                    "morph_draw",
                    "mask",
                    "face",
                    "face_uv",
                    "static_pos"
                };

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty layers = tagManager.FindProperty("layers");
            if (layers == null || !layers.isArray) {
                Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
                return;
            }


            var startIndex = layers.arraySize - 1;
            var message = "";
            foreach (var layer in BNBLayers) {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(startIndex--);
                if (layerSP.stringValue != layer) {
                    layerSP.stringValue = layer;
                }
                message += layer + ", ";
            }

            tagManager.ApplyModifiedProperties();

            Debug.LogWarning(string.Format(
                @"Banuba effects and assets imported with this package require some layers for rendering:
            {0}.They will be added to the end of your layers",
                message.Remove(message.Length - 2, 2)
            ));
        }
    }
}