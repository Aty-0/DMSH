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
        public float weaponBoostGain = 0.0f;

        // When we are not set the shot point 
        // Then we are going to use owner game object and his BoxCollider2D size
        [Header("Shot Point")]
        public Transform ShotPoint = null;

        [SerializeField]
        private bool _useOwner;

        [SerializeField]
        private BoxCollider2D _boxCollider2D;

        private Vector3 BulletSpawnPosition => _useOwner == false
            ? ShotPoint.transform.position
            : new Vector3(ShotPoint.transform.position.x, ShotPoint.transform.position.y - _boxCollider2D.size.y, 0);

        private Coroutine _shotCoroutine;

        // unity

        protected void OnEnable()
        {
            PatternFireState = FireStateStruct.CreateEmpty();

            // If point is null then start point will be game object
            if (ShotPoint == null)
            {
                _useOwner = true;
                ShotPoint = transform;
                _boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
                Debug.Assert(_boxCollider2D != null);
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
            Gizmos.color = Color.red;
            Gizmos.DrawCube(ShotPoint.transform.position, new Vector3(0.2f, 0.2f, 0.2f));
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(BulletSpawnPosition, new Vector3(0.2f, 0.2f, 0.2f));
        }
#endif

        // public API

        public void AddWeaponBoost(float gain)
        {
            weaponBoostGain += gain;
            if (weaponBoostGain >= 100.0f)
            {
                weaponBoostGain = 0.0f;
                // _weaponType += 1;
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

        // private

        private IEnumerator ShotC()
        {
            while (GlobalSettings.gameActiveAsBool && _weaponEnabled && canBeUsed)
            {
                if (m_firePattern != null)
                {
                    PatternFireState = FireStateStruct.CreateEmpty();
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