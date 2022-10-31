using UnityEngine;

using DMSH.Path;
using DMSH.Gameplay;

using Scripts.Objects.Bonuses;
using Scripts.Utils.Pools;

using Random = UnityEngine.Random;

namespace DMSH.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Weapon))]
    public class Enemy : MovableObject, IPooled
    {
        public const int MAX_RANDOM_DROP_SCORE_BONUS = 4;
        public const int MAX_RANDOM_DROP_WEAPON_BONUS = 4;
        
        [Header("Weapon")]
        public Weapon Weapon;

        [Header("Enemy")]
        public bool ignoreHits = true;
        [SerializeField]
        protected SpriteRenderer _spriteRenderer = null;
        [SerializeField]
        protected bool weakType = false;
        [SerializeField]
        protected bool onLastPointWillDestroy = false;
        [SerializeField]
        protected bool _isDead = false;
        [SerializeField]
        protected int _lifes = 1;
        [SerializeField]
        protected float _maxHealth = 0.0f;
        [SerializeField]
        protected float _health = 0.0f;
        [SerializeField]
        protected float _reduceHealth = 1.5f;
        [SerializeField]
        protected bool _showDamageStatusText = true;

        [Header("Bonus")]
        [SerializeField]
        private BonusDropTypeEnum m_bonusDropType;

        [Header("Misc")]
        [SerializeField]
        protected ParticleSystem _deathParticle;

        [Header("Sounds")]
        [SerializeField]
        protected AudioSource _deathAudioSource;
        [SerializeField]
        protected AudioSource _damageAudioSource;

        protected void Awake()
        {
            _deathParticle = GetComponentInChildren<ParticleSystem>();
        }

        protected void OnEnable()
        {
            ignoreHits = true;

            _health = _maxHealth;

            EnemyStart();
        }

        protected void OnDisable()
        {
            PathSystem = null;
        }
        
        public override void OnReachedPointEvent(EnemyScriptedBehavior enemyScriptedBehavior)
        {
            switch (enemyScriptedBehavior)
            {
                case EnemyScriptedBehavior.StartShot:
                {
                    StartShot();
                    break;
                }
                case EnemyScriptedBehavior.StopShot:
                {
                    StopShot();
                    break;
                }
            }
        }

        public override void OnReachedLastPoint()
        {
            if (onLastPointWillDestroy)
            {
                Kill(false, true);
            }
        }

        public void StartShot()
        {
            if (!weakType)
            {
                Weapon.Shot();
            }
        }

        public void StopShot()
        {
            if (!weakType)
            {
                Weapon.StopShooting();
            }
        }

        public override void UnSpawn()
        {
            Kill(false, true);
        }

        public void Kill(bool givePlayerScore, bool unspawn = false)
        {
            Weapon.StopShooting();

            _lifes = 0;
            _health = 0;
            _isDead = true;

            if (!unspawn)
            {
                OnDieCompletely();
            }

            if (givePlayerScore)
            {
                PlayerController.Player.Score += 1000;
            }

            if (_deathAudioSource != null)
            {
                _deathAudioSource.Play();
            }

            if (_deathParticle != null && !unspawn)
            {
                if (_pathSystem != null)
                {
                    _pathSystem.DetachObject(this);
                }

                var particleModule = _deathParticle.main;
                particleModule.startColor = _spriteRenderer.color;
                _deathParticle.Play();
                Release();
            }
            else
            {
                Release();
            }
        }

        private void CreateDamageStatusText(string text)
        {
            BonusStatsTextPool
                .GetOrCreate()
                .SpawnAt(transform.position, text);
        }

        public void Damage()
        {
            if (_isDead)
                return;

            if (ignoreHits)
                return;

            if (_damageAudioSource != null)
            {
                _damageAudioSource.Play();
            }

            OnDamage();
            if (_health <= 0.0f)
            {
                _health = _maxHealth;
                _lifes--;

                OnDie();

                if (_lifes == 0)
                {
                    Kill(true);
                }

                if (_showDamageStatusText)
                {
                    CreateDamageStatusText("Dead");
                }
            }
            else
            {
                if (_showDamageStatusText)
                {
                    CreateDamageStatusText($"-{_reduceHealth} HP");
                }

                _health -= _reduceHealth;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == PlayerController.Player.gameObject)
            {
                if (weakType)
                {
                    Kill(false);
                }
            }

            else if (ProjectilePool.TryGetByGo(collision.gameObject, out var bullet))
            {
                bullet.Release();
                Damage();
            }
        }

        protected virtual void EnemyStart()
        {
        }

        public virtual void OnDamage()
        {
        }

        public virtual void OnDie()
        {
        }

        public virtual void OnDieCompletely()
        {
            DropBonus();
        }

        public void DropBonus()
        {
            switch (m_bonusDropType)
            {
                case BonusDropTypeEnum.Score:
                    for (int i = 0; i <= Random.Range(0, MAX_RANDOM_DROP_SCORE_BONUS); i++)
                    {
                        BonusPool
                            .GetOrCreate()
                            .SpawnAt(transform.position, BonusPool.Get.Score1000Bonus);
                    }

                    break;

                case BonusDropTypeEnum.Weapon:
                    for (int i = 0; i <= Random.Range(0, MAX_RANDOM_DROP_WEAPON_BONUS); i++)
                    {
                        BonusPool
                            .GetOrCreate()
                            .SpawnAt(transform.position, BonusPool.Get.WeaponBonus);
                    }

                    break;
            }
        }

        public void CopyTo(Enemy target)
        {
            target._spriteRenderer.sprite = _spriteRenderer.sprite;
            target.transform.localScale = transform.localScale;
            target.speed = speed;
            target._currentPoint = 0;
            target._currentCurvePoint = 0;
            target.MoveOffset = MoveOffset;
            target._lifes = _lifes;
            target._maxHealth = _maxHealth;
            target._reduceHealth = _reduceHealth;
            target._showDamageStatusText = _showDamageStatusText;
            target.m_bonusDropType = m_bonusDropType;

            Weapon.CopyTo(target.Weapon);
        }

        public void Release()
        {
            if (!EnemyPool.TryRelease(this))
            {
                Debug.LogError($"Spawned enemy without pool! Will call direct destroy", this);
                Destroy(gameObject);
            }
        }
    }
}