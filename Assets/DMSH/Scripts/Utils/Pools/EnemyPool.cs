using DMSH.Characters;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class EnemyPool : GenericTypedGameObjectPool<Enemy, EnemyPool>
    {
        [SerializeField]
        private int m_enemiesPrecacheCount = 32;
        public override int PrecacheCount => m_enemiesPrecacheCount;
    }
}