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
            var fieldsCount = 6;

#pragma warning disable CS0618
            var angleConverterProperty = property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatType));
#pragma warning restore CS0618
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
            var spriteRect = new Rect(position.x, position.y + offset * ++labelCount, position.width - 60, 16);
            var spriteColorRect = new Rect(position.x + position.width - 50, position.y + offset * labelCount, 50, 16);
            var spawnOffsetRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);

            var repeatTypeRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);


            EditorGUI.PropertyField(toNextStepRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.ToNextStepAfter)));
            EditorGUI.PropertyField(patternRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.bulletFlyPattern)));
            EditorGUI.PropertyField(angleOffsetRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.AngleOffset)));
            EditorGUI.PropertyField(spawnOffsetRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.SpawnOffset)));
            EditorGUI.PropertyField(spriteRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.BulletSprite)));
            EditorGUI.PropertyField(spriteColorRect, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.BulletSpriteColor)), GUIContent.none);

#pragma warning disable CS0618
            var angleConverterProperty = property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatType));
#pragma warning restore CS0618
            EditorGUI.PropertyField(repeatTypeRect, angleConverterProperty);
            var angleConversionValue = (BulletSpawnPatternScriptableObject.RepeatEnumType) angleConverterProperty.enumValueIndex;
            switch (angleConversionValue)
            {
                case BulletSpawnPatternScriptableObject.RepeatEnumType.No:
                case BulletSpawnPatternScriptableObject.RepeatEnumType.PingPong:
                    break;
                
                case BulletSpawnPatternScriptableObject.RepeatEnumType.ForTimes:
                    var timesCount = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
#pragma warning disable CS0618
                    EditorGUI.PropertyField(timesCount, property.FindPropertyRelative(nameof(BulletSpawnPatternScriptableObject.ProjectileSpawnStepStruct.RepeatCount)));
#pragma warning restore CS0618
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.indentLevel = identBefore;
            EditorGUI.EndProperty();
        }
    }
}