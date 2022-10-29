using DMSH.Scripts.Objects.Projectiles;

using Scripts.Utils.Pools;

using UnityEngine;

namespace DMSH.Objects.Projectiles
{
    public struct ProjectileStateStruct
    {
        public int StepIndex;
        public float Lifetime;

        public int RelaunchedTimes;
        // public float RelaunchLifetime;

        public Vector2 InitialDirection;
        public Vector2 CalculatedDirection;

        // public float InitialAngle;
        // public float CalculatedAngle;

        public GenericReusableList<ProjectileModificatorStateStruct>.PooledList ModificatorStates;

        public ProjectileStateStruct Reset()
        {
            if (ModificatorStates != null)
            {
                ModificatorStates.Dispose();
                ModificatorStates = null;
            }

            return new ProjectileStateStruct
            {
                StepIndex = -1
            };
        }

        public ProjectileStateStruct FillModificators(ProjectileModificator[] projectileModificators)
        {
            if (projectileModificators == null || projectileModificators.Length == 0)
                return this;

            ModificatorStates = GenericReusableList<ProjectileModificatorStateStruct>.GetOrCreateList();
            for (var i = 0; i < projectileModificators.Length; i++)
            {
                ModificatorStates.Add(new ProjectileModificatorStateStruct(projectileModificators[i]));
            }

            return this;
        }
    }
}