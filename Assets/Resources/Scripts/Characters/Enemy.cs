using System.Collections;
using UnityEngine;

using DMSH.Path;
using DMSH.Misc;
using DMSH.Objects;

namespace DMSH.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Weapon))]
    public class Enemy : MovableObject
    {
        public const int MAX_RANDOM_DROP_SCORE_BONUS = 4;
        public const int MAX_RANDOM_DROP_WEAPON_BONUS = 4;

        [Header("Weapon")]
        public Weapon weapon;
        
        [Header("Enemy")]
        public bool ignoreHits = true;
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;
        [SerializeField] protected bool weakType = false;
        [SerializeField] protected bool onLastPointWillDestroy = false;
        [SerializeField] protected bool _isDead = false;
        [SerializeField] protected int _lifes = 1;
        [SerializeField] protected float _maxHealth = 0.0f;
        [SerializeField] protected float _health = 0.0f;
        [SerializeField] protected float _reduceHealth = 1.5f;
        [SerializeField] protected bool _showDamageStatusText = true;

        [Header("Bonus")]
        [SerializeField] protected GameObject _bonusWeaponBuff = null;
        [SerializeField] protected GameObject _bonusScoreBuff = null;

        [Header("Misc")]
        [SerializeField] protected PlayerController _playerController = null;
        [SerializeField] protected ParticleSystem _deathParticle = null;
        [SerializeField] protected Coroutine _shotCoroutine = null;

        [Header("Sounds")]
        [SerializeField] protected AudioSource _deathAudioSource = null;
        [SerializeField] protected AudioSource _damageAudioSource = null;
        
        protected void Start()
        {
            _rigidBody2D    = GetComponent<Rigidbody2D>();
            _boxCollider2D  = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _deathParticle  = GetComponentInChildren<ParticleSystem>();

            weapon          = GetComponent<Weapon>();
            weapon.onShot.Add(OnShot);
            
            _health = _maxHealth;
            ignoreHits = true;

            EnemyStart();
        }

        public override void OnReachedPointEvent(EnemyScriptedBehavior enemyScriptedBehavior)
        {
            switch (enemyScriptedBehavior)
            {
                case EnemyScriptedBehavior.StartShot:
                    StartShot();
                    break;
                case EnemyScriptedBehavior.StopShot:
                    StopShot();
                    break;
            }
        }

        public override void OnReachedLastPoint()
        {
            if (onLastPointWillDestroy)
            {
                Kill(false, true);
                return;
            }
        }

        public void StartShot()
        {
            if (!weakType)
            {
                OnStartShot();
                weapon.Shot();
            }
        }

        public void StopShot()
        {
            if (!weakType)
            {
                OnStopShot();
                weapon.StopShooting();
            }
        }

        public override void Unspawn()
        {
            Kill(false, true);
        }

        public void Kill(bool givePlayerScore, bool unspawn = false)
        {
            weapon.StopShooting();

            _lifes = 0;
            _health = 0;
            _isDead = true;

            if (!unspawn)
                OnDieCompletely();

            if (givePlayerScore)
                if (_playerController)
                    _playerController.Score += 1000;

            _deathAudioSource?.Play();
            if (_deathParticle && !unspawn)
            {
                _pathSystem?.DetachObject(this);
                _spriteRenderer.enabled = false;
                _boxCollider2D.enabled = false;
                ParticleSystemRenderer pr = _deathParticle.GetComponent<ParticleSystemRenderer>();
                pr.material.color = _spriteRenderer.color;
                _deathParticle.Play();
                Destroy(gameObject, _deathParticle.main.duration);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CreateDamageStatusText(string text)
        {
            GameObject textGO = new GameObject();
            textGO.name = $"ReduceHealthTextMesh{textGO.GetInstanceID()}";
            textGO.transform.position = transform.position;
            textGO.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            DamageStatusText dst = textGO.AddComponent<DamageStatusText>();
            dst.text = text;
        }

        public void Damage()
        {
            if (!ignoreHits)
            {
                _damageAudioSource?.Play();
                OnDamage();
                if (_health <= 0.0f)
                {
                    _health = _maxHealth;
                    _lifes--;

                    OnDie();

                    if (_lifes == 0)
                        Kill(true);

                    if (_showDamageStatusText)
                        CreateDamageStatusText("Dead");
                }
                else
                {
                    if (_showDamageStatusText)
                        CreateDamageStatusText($"-{_reduceHealth} HP");

                    _health -= _reduceHealth;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Component[] components = collision.gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                switch (component)
                {
                    case PlayerController p:
                        if (weakType)
                            Kill(false);
                        break;
                    case Bullet b:
                        Destroy(b);
                        Damage();
                        break;
                }
            }
        }

        protected virtual void EnemyStart()
        {

        }

        public virtual void OnShot()
        {

        }

        public virtual void OnStopShot()
        {

        }

        public virtual void OnStartShot()
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
            //TODO: Drop point in random position by range
            if (_bonusWeaponBuff)
            {
                for (int i = 0; i <= Random.Range(0, MAX_RANDOM_DROP_WEAPON_BONUS); i++)
                {
                    Instantiate(_bonusWeaponBuff);
                    _bonusWeaponBuff.transform.position = gameObject.transform.position;
                }
            }

            if (_bonusScoreBuff)
            {
                for (int i = 0; i <= Random.Range(0, MAX_RANDOM_DROP_SCORE_BONUS); i++)
                {
                    Instantiate(_bonusScoreBuff);
                    _bonusScoreBuff.transform.position = gameObject.transform.position;
                }
            }
        }
    }
}