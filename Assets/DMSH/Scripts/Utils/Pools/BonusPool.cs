using DMSH.Objects.Bonuses;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class BonusPool : GenericTypedGameObjectPool<Bonus, BonusPool>
    {
        [SerializeField]
        private IBonusBehaviour m_1000ScoreBonus;
        public IBonusBehaviour Score1000Bonus => m_1000ScoreBonus;
        
        [SerializeField]
        private IBonusBehaviour m_wewaponBonus;
        public IBonusBehaviour WeaponBonus => m_1000ScoreBonus;
    }
}