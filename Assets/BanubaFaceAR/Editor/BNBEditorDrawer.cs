using UnityEngine;
using UnityEditor;

namespace BNB
{
    [CustomPropertyDrawer(typeof(Morphing.WeightsUpdaterAttribute))]
    public class WeightsArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            try {
                int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                var posName = (BNB.Morphing.WeightsUpdater.ID) pos;
                EditorGUI.Slider(rect, property, -1, 1, new GUIContent(posName.ToString()));
            } catch {
                EditorGUI.ObjectField(rect, property, label);
            }
        }
    }

    [CustomPropertyDrawer(typeof(Morphing.MorphsArrayAttribute))]
    public class MorphsArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            try {
                int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);

                var posName = (BNB.Morphing.IMorphDraw.MorphType) pos;
                EditorGUI.ObjectField(rect, property, new GUIContent(posName.ToString()));
            } catch {
                EditorGUI.ObjectField(rect, property, label);
            }
        }
    }
}
