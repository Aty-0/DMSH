using System;

using UnityEngine;

namespace DMSH.Scripts.Objects.Projectiles
{
    [Serializable]
    public struct ProjectileModificator
    {
        public ProjectileModificatorEnum Type;

        public WallsFlags AffectedWalls;
        [Min(1)]
        public int BounceCount;
    }

    public enum ProjectileModificatorEnum : byte
    {
        Unset = 0,
        BounceFromWalls = 1,
    }

    [System.Flags]
    public enum WallsFlags
    {
        Unset = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Down = 8
    }
}