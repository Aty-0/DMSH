using UnityEngine;

namespace DMSH.Objects.Projectiles
{
    public struct ProjectileStateStruct
    {
        public int StepIndex;
        public float Lifetime;

        public int RelaunchedTimes;
        public float RelaunchLifetime;
        
        public Vector2 InitialDirection;
        public Vector2 CalculatedDirection;
        
        public float InitialAngle;
        public float CalculatedAngle;

        public static ProjectileStateStruct CreateEmpty() => new ProjectileStateStruct
        {
            StepIndex = -1
        };
    }
}