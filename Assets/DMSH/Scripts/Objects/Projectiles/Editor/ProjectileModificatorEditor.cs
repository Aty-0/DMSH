using DMSH.Scripts.Objects.Projectiles;

using System;

using UnityEditor;

using UnityEngine;

namespace DMSH.Objects.Projectiles.Editor
{
    [CustomPropertyDrawer(typeof(ProjectileModificator))]
    public class ProjectileModificatorEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldsCount = 1;
            
            var modificatorType = property.FindPropertyRelative(nameof(ProjectileModificator.Type));
            var modificatorTypeEnum = (ProjectileModificatorEnum) modificatorType.enumValueIndex;
            switch (modificatorTypeEnum)
            {
                case ProjectileModificatorEnum.BounceFromWalls:
                    // count + walls flags
                    fieldsCount += 2;
                    break;
                
                case ProjectileModificatorEnum.Unset:
                    break;
                
                default:
                    throw new NotImplementedException($"Modificator type {modificatorTypeEnum} not implemented in editor!");
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
            var modificatorTypeRect = new Rect(position.x, position.y + offset, position.width, 16);

            var modificatorTypeEnum = property.FindPropertyRelative(nameof(ProjectileModificator.Type));
            EditorGUI.PropertyField(modificatorTypeRect, modificatorTypeEnum);
            var modificatorEnumValue = (ProjectileModificatorEnum) modificatorTypeEnum.enumValueIndex;
            switch (modificatorEnumValue)
            {
                case ProjectileModificatorEnum.BounceFromWalls:
                    var wallFlagRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(wallFlagRect, property.FindPropertyRelative(nameof(ProjectileModificator.AffectedWalls)));
                    
                    var repeatTypeRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(repeatTypeRect, property.FindPropertyRelative(nameof(ProjectileModificator.BounceCount)));
                    break;
                
                case ProjectileModificatorEnum.Unset:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }


            EditorGUI.indentLevel = identBefore;
            EditorGUI.EndProperty();
        }
    }
}