﻿using DMSH.Gameplay;

using System;

using UnityEngine;

namespace DMSH.Objects.Projectiles
{
    [CreateAssetMenu(menuName = "DMSH/Projectile/ProjectileSpawnPattern")]
    public class BulletSpawnPatternScriptableObject : ScriptableObject
    {
        [TextArea, SerializeField]
        private string m_description;
        
        [SerializeField]
        private ProjectileSpawnStepStruct[] m_steps;

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (m_steps == null || m_steps.Length == 0)
            {
                Debug.LogWarning($"Warning! There a projectile pattern without steps!", this);
            }
        }
#endif

        // public 

        public void StartShooting(Weapon weapon)
        {
            if (m_steps == null || m_steps.Length == 0)
                return;

            ref var currentState = ref weapon.PatternFireState;

            currentState.IsStarted = true;
        }

        public void Tick(Weapon weapon)
        {
            if (m_steps == null || m_steps.Length == 0)
                return;

            ref var currentState = ref weapon.PatternFireState;
            if (!currentState.IsStarted)
                return;

            var startNewStep = false;
            if (currentState.StepIndex == -1)
            {
                // initial step, start It
                currentState.StepIndex = 0;
                startNewStep = true;
            }
            else
            {
                currentState.Lifetime -= Time.deltaTime;

                if (currentState.StepIndex < m_steps.Length && currentState.Lifetime <= 0)
                {
                    currentState.StepIndex++;
                    startNewStep = currentState.StepIndex < m_steps.Length;
                }
            }

            if (startNewStep)
            {
                StartNewStep(weapon, ref currentState, m_steps[currentState.StepIndex]);

                if (currentState.Lifetime <= 0)
                {
                    Tick(weapon);
                }
            }
        }

        private static void StartNewStep(Weapon weapon, ref FireStateStruct state, ProjectileSpawnStepStruct step)
        {
            var firedBullet = weapon.FireBullet();

            // set Mono properties
            firedBullet.Pattern = step.bulletFlyPattern;
            firedBullet.SetSprite(step.BulletSprite, step.BulletSpriteColor);
            if (firedBullet.Pattern != null)
            {
                firedBullet.Pattern.Tick(firedBullet);
                if (step.AngleOffset != 0
                    && firedBullet.Pattern.TryGetCurrentStep(firedBullet, out var patternStep)
                    && patternStep.AngleConverter
                        is ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle
                        or ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectAngle_WithRandomFactor
                        or ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.InPlayerDirection_PlusAngle
                        or ProjectileFlyBehaviorScriptableObject.ProjectileDirectionTypeEnum.DirectDirection)
                {
                    firedBullet.Pattern.RecalculateWithAngleOffset(firedBullet, step.AngleOffset);
                }
            }

            // set state
            state.Lifetime = step.ToNextStepAfter;
        }

        // data

        [Serializable]
        public struct ProjectileSpawnStepStruct
        {
            public float ToNextStepAfter;

            public ProjectileFlyBehaviorScriptableObject bulletFlyPattern;

            public Sprite BulletSprite;
            public Color BulletSpriteColor;

            [Obsolete("Not implemented")]
            public RepeatEnumType RepeatType;
            [Obsolete("Not implemented")]
            public int RepeatCount;

            public float AngleOffset;
        }

        public enum RepeatEnumType
        {
            No = 0,
            ForTimes = 1,
            PingPong = 2,
        }
    }
}