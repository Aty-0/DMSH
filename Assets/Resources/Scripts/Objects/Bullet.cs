using DMSH.Characters;

using System.Collections;

using UnityEngine;

using DMSH.Misc;
using DMSH.Path;
using DMSH.Gameplay;
using DMSH.Objects.Projectiles;

using Scripts.Utils.Pools;

namespace DMSH.Objects
{
    public sealed class Bullet : MovableObject, IPooled
    {
        [Header("Bullet.Settings")]
        public bool IsEnemyBullet;

        [SerializeField]
        private ProjectileFlyBehaviorScriptableObject m_pattern;

        public ProjectileFlyBehaviorScriptableObject Pattern
        {
            get => m_pattern;
            set
            {
                if (m_pattern != value)
                {
                    m_pattern = value;
                    if (m_pattern != null)
                    {
                        OnEnable();
                    }
                }
            }
        }

        [SerializeField]
        private bool _collisionDestroyBullet = true;
        public bool IsCollisionDestroyBullet => _collisionDestroyBullet;

        [SerializeField]
        private float _lifeTime = 2.0f;

        public bool IsMovesItself => pathSystem == null;

        // internal
        [Tooltip("Direction in which projectile will fly")]
        [SerializeField]
        private Vector2 _bulletDirection = new(0, 1);

        public Vector2 BulletDirection
        {
            get => _bulletDirection;
            set => _bulletDirection = value;
        }

        internal ProjectileStateStruct ProjectileState;

        [Tooltip("Will rotate projectile around every tick")]
        [SerializeField]
        private float _graphicRotationSpeed = 300.0f;

        [Header("Bullet.References")]
        [SerializeField]
        private TrailRenderer _trailRenderer = null;
        private Timer _timer;

        // unity

        internal void OnEnable()
        {
            ProjectileState = ProjectileStateStruct.CreateEmpty();
            BulletDirection = new Vector2(0, -1);

            // If we are attach pathSystem on start 
            if (IsMovesItself)
            {
                _timer = gameObject.AddComponent<Timer>();
                _timer.EndEvent += Unspawn;
                _timer.time = _lifeTime;
                _timer.StartTimer();
            }
        }

        internal void Start()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        internal void OnDisable()
        {
            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
            }

            if (_timer != null)
            {
                _timer.StopTimer();
                Destroy(_timer);
                _timer = null;
            }
        }

        internal void Update()
        {
            if (m_pattern != null)
            {
                m_pattern.Tick(this);
            }
        }

        internal void FixedUpdate()
        {
            // Basic effect of rotation
            rigidBody2D.MoveRotation(rigidBody2D.rotation + _graphicRotationSpeed * Time.fixedDeltaTime * GlobalSettings.gameActiveAsInt);

            if (IsMovesItself)
            {
                rigidBody2D.MovePosition(rigidBody2D.position + _bulletDirection * speed * Time.fixedDeltaTime * GlobalSettings.gameActiveAsInt);
            }
        }

        internal void OnCollisionEnter2D(Collision2D collision)
        {
            if (_collisionDestroyBullet
                && ((IsEnemyBullet && collision.transform == PlayerController.Player.transform)
                    || (!IsEnemyBullet && collision.transform.CompareTag("Enemy"))))
            {
                Unspawn();
            }
        }

        // public APIs

        public void SqueezeAndDestroy()
        {
            boxCollider2D.enabled = false;
            StartCoroutine(SqueezeAnimation());
        }

        public override void Unspawn()
        {
            Release();
        }

        // private

        // Squeeze bullet 
        // It's just destroy effect for boost or player death
        private IEnumerator SqueezeAnimation()
        {
            var sizeBefore = transform.localScale;
            var trailStartSizeWidthBefore = _trailRenderer.startWidth;
            var trailEndSizeWidthBefore = _trailRenderer.endWidth;
            var animateUntilVector = new Vector3(0.0f, 0.0f, transform.localScale.z);
            var lerpVal = 0f;

            while (transform.localScale != animateUntilVector)
            {
                var smalledLocalScale = Vector3.Lerp(transform.localScale, Vector3.zero, lerpVal);
                var trailStartWidth = Mathf.Lerp(_trailRenderer.startWidth, 0, lerpVal);
                var trailEndWidth = Mathf.Lerp(_trailRenderer.endWidth, 0, lerpVal);

                transform.localScale = new Vector3(smalledLocalScale.x, smalledLocalScale.y, animateUntilVector.z);
                _trailRenderer.endWidth = trailStartWidth;
                _trailRenderer.startWidth = trailEndWidth;

                lerpVal += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }

            Unspawn();

            _trailRenderer.startWidth = trailStartSizeWidthBefore;
            _trailRenderer.endWidth = trailEndSizeWidthBefore;
            transform.localScale = sizeBefore;
        }

        public void Release()
        {
            if (!ProjectilePool.TryRelease(this))
            {
                Debug.LogError($"Spawned bullet without pool! Will call direct destroy", this);
                Destroy(gameObject);
                return;
            }

            if (_timer != null)
            {
                _timer.StopTimer();
                Destroy(_timer);
                _timer = null;
            }

            ProjectileState = ProjectileStateStruct.CreateEmpty();
            Pattern = null;
        }
    }
}