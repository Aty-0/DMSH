using System;

using UnityEditor;

using UnityEngine;

namespace DMSH.Objects.Projectiles.Editor
{
    [CustomPropertyDrawer(typeof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct))]
    public class ProjectileBehaviorStepEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldsCount = 5;

            var angleConverterProperty = property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.AngleConverter));
            var angleConversionValue = (ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum) angleConverterProperty.enumValueIndex;
            switch (angleConversionValue)
            {
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.InPlayerDirection:
                    break;

                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.InPlayerDirection_PlusAngle:
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle:
                    fieldsCount++;
                    break;

                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectDirection:
                    fieldsCount++;
                    break;
                
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle_WithRandomFactor:
                    fieldsCount++;
                    fieldsCount++;
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
            var lifetimeRect = new Rect(position.x, position.y + offset, position.width, 16);
            var dirSpeedModifRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
            var lifetimeDirModifRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
            var angleConverterRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
            var switchOnMaskRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);

            EditorGUI.PropertyField(lifetimeRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.Lifetime)));
            EditorGUI.PropertyField(dirSpeedModifRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.DirectionSpeedModifier)));
            EditorGUI.PropertyField(lifetimeDirModifRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.LifetimeDirectionModifier)));

            var angleConverterProperty = property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.AngleConverter));
            EditorGUI.PropertyField(angleConverterRect, angleConverterProperty);
            var angleConversionValue = (ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum) angleConverterProperty.enumValueIndex;
            switch (angleConversionValue)
            {
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.InPlayerDirection:
                    break;

                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.InPlayerDirection_PlusAngle:
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle:
                    var angleRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(angleRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.Angle)));
                    break;
                
                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle_WithRandomFactor:
                    angleRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(angleRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.Angle)));
                    var leftFactorRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(leftFactorRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.RandomFactorLeft)));
                    var rightFactorRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(rightFactorRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.RandomFactorRight)));
                    break;

                case ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectDirection:
                    var directionRect = new Rect(position.x, position.y + offset * ++labelCount, position.width, 16);
                    EditorGUI.PropertyField(directionRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.Direction)));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.PropertyField(switchOnMaskRect, property.FindPropertyRelative(nameof(ProjectileFlyBehaviorScriptableObject.ProjectileBehaviorStepStruct.SwitchOnMask)));

            EditorGUI.indentLevel = identBefore;
            EditorGUI.EndProperty();
        }
    }
}