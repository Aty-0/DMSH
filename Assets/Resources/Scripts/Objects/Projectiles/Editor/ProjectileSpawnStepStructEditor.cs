using System;

using UnityEditor;

using UnityEngine;

namespace DMSH.Objects.Projectiles.Editor
{
    [CustomPropertyDrawer(typeof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct))]
    public class ProjectileSpawnStepStructEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldsCount = 4;

            var angleConverterProperty = property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatType));
            var angleConversionValue = (BulletSpawnPatternScriptableObject.RepeatEnumType) angleConverterProperty.enumValueIndex;
            switch (angleConversionValue)
            {
                case BulletSpawnPatternScriptableObject.RepeatEnumType.No:
                case BulletSpawnPatternScriptableObject.RepeatEnumType.PingPong:
                    break;
                
                case BulletSpawnPatternScriptableObject.RepeatEnumType.ForTimes:
                    fieldsCount++;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return EditorGUIUtility.singleLineHeight * fieldsCount + EditorGUIUtility.singleLineHeight + 6;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = string.Empty;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(position, label);

            var identBefore = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // lifetime
            const float offset = 18;
            var labelCount = 1;
            var toNextStepRect = new Rect(position.x, position.y + offset, position.width, 16);
            var patternRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
            var angleOffsetRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
            var repeatTypeRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);

            EditorGUI.PropertyField(toNextStepRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.ToNextStepAfter)));
            EditorGUI.PropertyField(patternRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.bulletFlyPattern)));
            EditorGUI.PropertyField(angleOffsetRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.AngleOffset)));

            var angleConverterProperty = property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatType));
            EditorGUI.PropertyField(repeatTypeRect, angleConverterProperty);
            var angleConversionValue = (BulletSpawnPatternScriptableObject.RepeatEnumType) angleConverterProperty.enumValueIndex;
            switch (angleConversionValue)
            {
                case BulletSpawnPatternScriptableObject.RepeatEnumType.No:
                case BulletSpawnPatternScriptableObject.RepeatEnumType.PingPong:
                    break;
                
                case BulletSpawnPatternScriptableObject.RepeatEnumType.ForTimes:
                    var timesCount = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(timesCount, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatCount)));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }


            EditorGUI.indentLevel = identBefore;
            EditorGUI.EndProperty();
        }
    }
}