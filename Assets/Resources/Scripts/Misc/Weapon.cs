using UnityEngine;
using DMSH.Objects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DMSH.Misc
{
    // TODO: Weapon diffrent types
    public class Weapon : MonoBehaviour
    {
        public bool         weaponEnabled { get => _weaponEnabled; private set { _weaponEnabled = value; } }
        public int          weaponType{ get => _weaponType; private set { _weaponType = value; } }

        [Header("Weapon base")]
        public Bullet        bulletPrefab = null;
        public float         shotFrequency = 0.07f;
        public bool          canBeUsed = true;
        [SerializeField] private bool        _weaponEnabled = false;
        [SerializeField] private AudioSource _audioSource;

        public List<Action>  onShot = new List<Action>();

        [Header("Weapon upgrage")]
        public float        weaponBoostGain = 0.0f;
        [SerializeField] private int         _weaponType = 0;

        // When we are not set the shot point 
        // Then we are going to use owner game object and his BoxCollider2D size
        [Header("Shot Point")]
        public Transform     shotPoint = null;
        [SerializeField] private bool _useOwner;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private Vector3       _position;

        private Coroutine   _shotCoroutine;

        protected void Start()
        {
            // If point is null then start point will be game object
            if (shotPoint == null)
            {
                _useOwner = true;
                shotPoint = transform;
                _boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
                Debug.Assert(_boxCollider2D);
            }
        }

        public void AddWeaponBoost(float gain)
        {
            weaponBoostGain += gain;
            if (weaponBoostGain >= 100.0f)
            {
                weaponBoostGain = 0.0f;
                _weaponType += 1;
            }
        }

        public void Shot()
        {
            _weaponEnabled = true;
            _shotCoroutine = StartCoroutine(ShotC());
        }

        public void StopShooting()
        {
            _weaponEnabled = false;
            if(_shotCoroutine != null)
                StopCoroutine(_shotCoroutine);
        }

        protected void OnDrawGizmos()
        {
            if (GlobalSettings.debugDrawWeaponPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(shotPoint.transform.position, new Vector3(0.2f, 0.2f, 0.2f));
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(_position, new Vector3(0.2f, 0.2f, 0.2f));
            }
        }
        private void UpdateBulletSpawnPosition()
        {
            if (_weaponEnabled && canBeUsed)
            {
                _position = _useOwner == false ? shotPoint.transform.position :
                    new Vector3(shotPoint.transform.position.x, shotPoint.transform.position.y - _boxCollider2D.size.y, 0);
            }
        }

        protected void FixedUpdate()
        {
            UpdateBulletSpawnPosition();
        }

        private IEnumerator ShotC()
        {
            while (_weaponEnabled && canBeUsed)
            {
                foreach (Action action in onShot)
                    action?.Invoke();

                // First update 
                UpdateBulletSpawnPosition();

                Instantiate(bulletPrefab, _position, Quaternion.identity);

                if(_audioSource)
                    _audioSource.Play();

                yield return new WaitForSeconds(shotFrequency);
            }
        }
    }
}
