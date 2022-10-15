using DMSH.UI;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class BonusStatsTextPool : GenericTypedGameObjectPool<UI_BonusStatText, BonusStatsTextPool>
    {
        [SerializeField]
        private int m_precacheCount = 8;
        public override int PrecacheCount => m_precacheCount;
    }
}