using UnityEditor;

using UnityEngine;

namespace Scripts.Utils.Unity.Editor
{
    [CustomPropertyDrawer(typeof(Layer))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty layerProp = property.FindPropertyRelative("m_layer");
            layerProp.intValue = EditorGUI.LayerField(position, label, layerProp.intValue);
        }
    }
}