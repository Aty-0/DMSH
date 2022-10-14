using DMSH.Objects;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class ProjectilePool : GenericTypedGameObjectPool<Bullet, ProjectilePool>
    {
        [SerializeField]
        private int m_precacheCount = 256;
        public override int PrecacheCount => m_precacheCount;
    }
}