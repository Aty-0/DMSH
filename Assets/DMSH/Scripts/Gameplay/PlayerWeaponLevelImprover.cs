using DMSH.Characters;
using DMSH.Objects.Projectiles;

using System;

using UnityEngine;

namespace DMSH.Gameplay
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Weapon))]
    public class PlayerWeaponLevelImprover : MonoBehaviour
    {
        [SerializeField]
        private BulletSpawnPatternScriptableObject[] m_weaponLevels;

        public float WeaponUpgradeGain { get; private set; }
        public int CurrentWeaponLevel => Array.IndexOf(m_weaponLevels, _weapon.CurrentFirePattern);

        private Weapon _weapon;
        private bool _weaponMaxUpgrade;

        // unity

        protected void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        protected void Start()
        {
            _weapon.SetFirePattern(m_weaponLevels[0]);
            _weaponMaxUpgrade = m_weaponLevels.Length == 1;
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (m_weaponLevels == null)
            {
                Debug.LogWarning($"Null patterns for player {name}!", this);
                return;
            }

            switch (m_weaponLevels.Length)
            {
                case 0:
                    Debug.LogWarning($"No patterns for player {name}!", this);
                    break;
                case 1:
                    Debug.LogWarning($"Only one pattern for player {name}!", this);
                    break;
            }
        }
#endif

        // public's

        public void AddWeaponUpgradeGain(float gain)
        {
            if (_weaponMaxUpgrade == true)
                return;

            WeaponUpgradeGain += gain;

            if (WeaponUpgradeGain >= 100.0f)
            {
                WeaponUpgradeGain = 0.0f;
                GoToNextWeaponType();
            }
        }

        // private's

        private void GoToNextWeaponType()
        {
            if (_weaponMaxUpgrade)
                return;

            var currentPatternIndex = Array.IndexOf(m_weaponLevels, _weapon.CurrentFirePattern);

            var nextIndex = currentPatternIndex + 1;
            
            _weaponMaxUpgrade = nextIndex >= m_weaponLevels.Length;
            if (_weaponMaxUpgrade)
                return;

            _weapon.SetFirePattern(m_weaponLevels[nextIndex]);
        }
    }
}