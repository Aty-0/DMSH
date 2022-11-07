using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DMSH.Misc;
using DMSH.Objects;
using DMSH.Objects.Projectiles;

using Scripts.Utils.Pools;

namespace DMSH.Gameplay
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon base")]
        [SerializeField]
        private List<BulletSpawnPatternScriptableObject> _firePatterns;

        [SerializeField]
        private BulletSpawnPatternScriptableObject m_firePattern;

        public FireStateStruct PatternFireState;

        [SerializeField]
        private bool m_isEnemy = true;

        public float shotFrequency = 0.07f;
        public bool canBeUsed = true;

        [SerializeField]
        private bool _weaponEnabled = false;

        [SerializeField]
        private AudioSource _audioSource;

        public List<Action> onShot = new List<Action>();

        [Header("Weapon upgrage")]
        public float weaponUpgradeGain = 0.0f;
        [SerializeField]
        private bool _weaponMaxUpgrade = false;

        // When we are not set the shot point 
        // Then we are going to use owner game object and his BoxCollider2D size
        [Header("Shot Point")]
        public Transform ShotPoint = null;

        [SerializeField]
        private bool _useOwner;

        [SerializeField]
        private Collider2D _Collider2D;

        private Vector3 BulletSpawnPosition
        {
            get
            {
                if (_useOwner == false)
                    return ShotPoint.transform.position;

                switch (_Collider2D)
                {
                    case BoxCollider2D boxCollider:
                        return new Vector3(ShotPoint.transform.position.x, ShotPoint.transform.position.y - boxCollider.size.y, 0);

                    default:
                        return transform.position;
                }
            }
        }

        private Coroutine _shotCoroutine;

        // unity

        protected void OnEnable()
        {
            PatternFireState = FireStateStruct.CreateEmpty();

            // To avoid breaking old prefabs
            if(m_firePattern != null)
            {
                _firePatterns.Insert(0, m_firePattern);
            }

            // If point is null then start point will be game object
            if (ShotPoint == null)
            {
                _useOwner = true;
                ShotPoint = transform;
                if (_Collider2D == null)
                {
                    _Collider2D = gameObject.GetComponent<Collider2D>();
                    Debug.Assert(_Collider2D != null);
                }
            }
        }

        protected void Update()
        {
            if (m_firePattern != null)
            {
                m_firePattern.Tick(this);
            }
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (ShotPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(ShotPoint.transform.position, new Vector3(0.2f, 0.2f, 0.2f));

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(BulletSpawnPosition, new Vector3(0.2f, 0.2f, 0.2f));
            }
        }
#endif

        // public API
        public void GoToNextWeaponType()
        {            
            if (m_firePattern == null || _firePatterns.Count <= 1)
            {
                // We don't have any patterns then we don't need to upgrade
                _weaponMaxUpgrade = true;
                return;
            }

            var index = _firePatterns.IndexOf(m_firePattern) + 1;

            if (index >= _firePatterns.Count)
            {
                _weaponMaxUpgrade = true;
                return;
            }

            m_firePattern = _firePatterns[index];
        }

        public void AddWeaponUpgradeGain(float gain)
        {
            if(_weaponMaxUpgrade == true)
            {
                return;
            }

            weaponUpgradeGain += gain;
            if (weaponUpgradeGain >= 100.0f)
            {
                weaponUpgradeGain = 0.0f;
                GoToNextWeaponType();
            }
        }

        [ContextMenu("DebugShoot_Start")]
        public void Shot()
        {
            _weaponEnabled = true;
            _shotCoroutine = StartCoroutine(ShotC());
        }

        [ContextMenu("DebugShoot_Stop")]
        public void StopShooting()
        {
            _weaponEnabled = false;
            if (_shotCoroutine != null)
            {
                StopCoroutine(_shotCoroutine);
                _shotCoroutine = null;
            }
        }

        public Bullet FireBullet()
        {
            foreach (Action action in onShot)
            {
                action?.Invoke();
            }

            var bullet = ProjectilePool.GetOrCreate();
            bullet.transform.SetPositionAndRotation(BulletSpawnPosition, Quaternion.identity);
            bullet.IsEnemyBullet = m_isEnemy;

            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            return bullet;
        }

        public void CopyTo(Weapon target)
        {
            StopShooting();

            target._useOwner = _useOwner;
            target._weaponEnabled = _weaponEnabled;
            target._firePatterns = _firePatterns;
            target.m_firePattern = m_firePattern;
            target.m_isEnemy = m_isEnemy;
            target.shotFrequency = shotFrequency;
            target.canBeUsed = canBeUsed;
            target.weaponUpgradeGain = weaponUpgradeGain;
            target.PatternFireState = PatternFireState;
            if (target.ShotPoint != null)
            {
                target.ShotPoint.localPosition = ShotPoint != null
                    ? ShotPoint.localPosition
                    : Vector3.zero;
            }
        }

        // private

        private IEnumerator ShotC()
        {
            while (!GlobalSettings.IsPaused && _weaponEnabled && canBeUsed)
            {
                if (m_firePattern != null)
                {
                    PatternFireState = FireStateStruct.CreateEmpty();
                    m_firePattern.StartShooting(this);
                    m_firePattern.Tick(this);
                }
                else
                {
                    FireBullet();
                }

                yield return new WaitForSeconds(shotFrequency);
            }
        }
    }
}