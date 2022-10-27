using DMSH.Characters;

using GD.MinMaxSlider;

using Scripts.Utils;
using Scripts.Utils.Unity;

using System;

using UnityEngine;

namespace DMSH.Objects.Projectiles
{
    [CreateAssetMenu(menuName = "DMSH/Projectile/ProjectileFlyPattern")]
    public class ProjectileFlyBehaviorScriptableObject : ScriptableObject
    {
        [TextArea, SerializeField]
        private string m_description;

        [SerializeField]
        private ProjectileBehaviorStepStruct[] m_steps;

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

        public bool TryGetCurrentStep(Bullet projectile, out ProjectileBehaviorStepStruct step)
        {
            step = default;
            if (m_steps == null || m_steps.Length == 0)
                return false;

            if (m_steps.Length <= projectile.ProjectileState.StepIndex
                || projectile.ProjectileState.StepIndex < 0)
                return false;

            step = m_steps[projectile.ProjectileState.StepIndex];
            return true;
        }

        public void Tick(Bullet projectile)
        {
            if (m_steps == null || m_steps.Length == 0)
                return;

            ref var currentState = ref projectile.ProjectileState;

            // launch new step
            var startNewStep = false;
            var stepIndexIsChanged = false;
            if (currentState.StepIndex == -1)
            {
                // initial step, start It
                currentState.StepIndex = 0;
                stepIndexIsChanged = true;
                startNewStep = true;
            }
            else
            {
                currentState.Lifetime -= Time.deltaTime;

                if (currentState.StepIndex < m_steps.Length)
                {
                    if (currentState.Lifetime <= 0)
                    {
                        if (CanRiseStepLogic(ref currentState, m_steps[currentState.StepIndex]))
                        {
                            stepIndexIsChanged = true;
                            currentState.StepIndex++;
                        }

                        startNewStep = true;
                    }
                    else if (m_steps[currentState.StepIndex].RelaunchType == ProjectileRelaunchTypeEnum.RelaunchEveryTickForLifetime)
                    {
                        startNewStep = true;
                    }
                }
            }

            if (startNewStep && currentState.StepIndex < m_steps.Length)
            {
                StartNewStep(projectile, ref currentState, m_steps[currentState.StepIndex], stepIndexIsChanged);
            }

            if (currentState.StepIndex >= m_steps.Length)
                return;

            // tick new step
            var step = m_steps[currentState.StepIndex];

            var lerpVal = step.RelaunchType == ProjectileRelaunchTypeEnum.RelaunchEveryTickForLifetime
                ? 1
                : step.LifetimeDirectionModifier.Evaluate(1 - (currentState.Lifetime / step.Lifetime));

            projectile.BulletDirection = Vector2.Lerp(
                currentState.InitialDirection,
                currentState.CalculatedDirection,
                lerpVal);
        }

        // private

        private static bool CanRiseStepLogic(ref ProjectileStateStruct state, ProjectileBehaviorStepStruct step)
        {
            switch (step.RelaunchType)
            {
                case ProjectileRelaunchTypeEnum.RelaunchInfinity:
                    return false;

                case ProjectileRelaunchTypeEnum.RelaunchTimes:
                    state.RelaunchedTimes++;
                    var canMoveToNext = step.RelaunchTimes <= state.RelaunchedTimes;
                    if (canMoveToNext)
                    {
                        state.RelaunchedTimes = 0;
                    }

                    return canMoveToNext;

                case ProjectileRelaunchTypeEnum.RelaunchEveryTickForLifetime:
                    break;

                case ProjectileRelaunchTypeEnum.None:
                default:
                    break;
            }

            state.RelaunchedTimes = 0;
            return true;
        }

        private static void StartNewStep(Bullet projectile, ref ProjectileStateStruct state, ProjectileBehaviorStepStruct step, bool applyLifetime)
        {
            Vector2 directionFromAngle;
            switch (step.AngleConverter)
            {
                case ProjectileDirectionTypeEnum.InPlayerDirection:
                    directionFromAngle = (PlayerController.Player.transform.position - projectile.transform.position).normalized;
                    break;

                case ProjectileDirectionTypeEnum.InPlayerDirection_PlusAngle:
                    var preAngle = !projectile.IsEnemyBullet
                        ? new Vector2(0, -1)
                        : new Vector2(0, 1);
                    var resultAngle = preAngle + MathUtils.RadianToVector2(step.Angle / 180 * Mathf.PI);
                    directionFromAngle = resultAngle.normalized;
                    break;

                case ProjectileDirectionTypeEnum.DirectAngle:
                    directionFromAngle = MathUtils.RadianToVector2(step.Angle / 180 * Mathf.PI);
                    break;

                case ProjectileDirectionTypeEnum.DirectAngle_WithRandomFactor:
                    var angleOffset = UnityEngine.Random.Range(0, 100) >= 50
                        ? -UnityEngine.Random.Range(step.RandomFactorLeft.x, step.RandomFactorLeft.y)
                        : UnityEngine.Random.Range(step.RandomFactorRight.x, step.RandomFactorRight.y);
                    directionFromAngle = MathUtils.RadianToVector2((step.Angle + angleOffset) / 180 * Mathf.PI);
                    break;

                case ProjectileDirectionTypeEnum.DirectDirection:
                    directionFromAngle = step.Direction;
                    break;

                default:
                    Debug.LogError($"Projectile type ({step.AngleConverter}) not implemented! fallback to default", projectile);
                    goto case ProjectileDirectionTypeEnum.InPlayerDirection;
            }

            if (step.DirectionSpeedModifier != 0)
            {
                directionFromAngle *= step.DirectionSpeedModifier;
            }

            // set state
            state.InitialDirection = projectile.BulletDirection;
            state.CalculatedDirection = directionFromAngle;

            if (applyLifetime)
            {
                state.Lifetime = step.Lifetime;
            }

            if (step.SwitchOnMask.Index is > 0 and < 32)
            {
                projectile.gameObject.layer = step.SwitchOnMask.Index;
            }
        }

        public void RecalculateWithAngleOffset(Bullet projectile, float angleOffset)
        {
            ref var currentState = ref projectile.ProjectileState;
            var step = m_steps[currentState.StepIndex];

            Vector2 directionFromAngle;
            switch (step.AngleConverter)
            {
                case ProjectileDirectionTypeEnum.InPlayerDirection:
                    var preAngle = (Vector2) (PlayerController.Player.transform.position - projectile.transform.position).normalized;
                    var midAngle = preAngle + MathUtils.RadianToVector2(angleOffset);
                    directionFromAngle = midAngle.normalized;
                    break;

                case ProjectileDirectionTypeEnum.InPlayerDirection_PlusAngle:
                    preAngle = !projectile.IsEnemyBullet
                        ? new Vector2(0, -1)
                        : new Vector2(0, 1);
                    var resultAngle = preAngle + MathUtils.RadianToVector2((step.Angle + angleOffset) / 180 * Mathf.PI);
                    directionFromAngle = resultAngle.normalized;
                    break;

                case ProjectileDirectionTypeEnum.DirectAngle:
                    directionFromAngle = MathUtils.RadianToVector2((step.Angle + angleOffset) / 180 * Mathf.PI);
                    break;

                case ProjectileDirectionTypeEnum.DirectAngle_WithRandomFactor:
                    var randomOffset = UnityEngine.Random.Range(0, 100) >= 50
                        ? -UnityEngine.Random.Range(step.RandomFactorLeft.x, step.RandomFactorLeft.y)
                        : UnityEngine.Random.Range(step.RandomFactorRight.x, step.RandomFactorRight.y);

                    directionFromAngle = MathUtils.RadianToVector2((step.Angle + angleOffset + randomOffset) / 180 * Mathf.PI);
                    break;

                case ProjectileDirectionTypeEnum.DirectDirection:
                    directionFromAngle = (step.Direction + MathUtils.RadianToVector2(angleOffset)).normalized;
                    break;

                default:
                    Debug.LogError($"Projectile type ({step.AngleConverter}) not implemented! fallback to default", projectile);
                    goto case ProjectileDirectionTypeEnum.InPlayerDirection;
            }

            if (step.DirectionSpeedModifier != 0)
            {
                directionFromAngle *= step.DirectionSpeedModifier;
            }

            // set state
            currentState.InitialDirection = (projectile.BulletDirection + MathUtils.RadianToVector2(angleOffset)).normalized;
            ;
            currentState.CalculatedDirection = directionFromAngle;
        }

        // data

        [Serializable]
        public struct ProjectileBehaviorStepStruct
        {
            [Tooltip("0 - means infinity")]
            [Min(0)]
            public float Lifetime;

            public float DirectionSpeedModifier;
            public Vector2 Direction;

            public AnimationCurve LifetimeDirectionModifier;
            // TODO public AnimationCurve LifetimeAngleModifier;

            public ProjectileDirectionTypeEnum AngleConverter;
            public Layer SwitchOnMask;

            [Range(0, 360)]
            public float Angle;

            // DirectAngle_WithRandomFactor
            [MinMaxSlider(0, 180)]
            public Vector2Int RandomFactorLeft;
            [MinMaxSlider(0, 180)]
            public Vector2Int RandomFactorRight;

            public ProjectileRelaunchTypeEnum RelaunchType;
            public int RelaunchTimes;
        }

        public enum ProjectileDirectionTypeEnum
        {
            InPlayerDirection = 0,
            InPlayerDirection_PlusAngle = 1,
            DirectAngle = 2,
            DirectDirection = 3,
            DirectAngle_WithRandomFactor = 4,
        }

        public enum ProjectileRelaunchTypeEnum
        {
            /// <summary>Don't relaunch</summary>
            None = 0,

            /// <summary>Relaunch this state after lifetime before explode</summary>
            RelaunchInfinity = 1,

            /// <summary>Relaunch N times and move to next state</summary>
            RelaunchTimes = 2,

            /// <summary>Calc direction every tick for lifetime</summary>
            RelaunchEveryTickForLifetime = 3,
        }
    }
}