using UnityEngine;

using DMSH.Characters;
using DMSH.Gameplay;
using DMSH.Misc;

using Scripts.Utils.Pools;

using UnityEditor;

namespace DMSH.Objects.Bonuses
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
    public class Bonus : MonoBehaviour
    {
        private static readonly Vector2 INITIAL_FALL_DIRECTION = new(0, 4);
        private static readonly Vector2 DEFAULT_FALL_DIRECTION = new(0, -2);

        public IBonusBehaviour BonusBehaviour;

        [SerializeField]
        private float m_initialSpeedModifier = 3f;

        [SerializeField]
        private float m_moveSpeedModifier = 9f;

        [SerializeField]
        private float m_magnetRadius = 2.4f;

        [SerializeField]
        private AudioSource m_audioSource;
        public AudioSource AudioSource => m_audioSource;

        public Rigidbody2D Rigidbody { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public Collider2D Collider { get; private set; }

        private Timer _destroyTimer;
        private float _directionChangeLerp;

        // unity

        protected void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Renderer = GetComponent<SpriteRenderer>();
            Collider = GetComponent<Collider2D>();

            if (m_audioSource != null)
            {
                m_audioSource = GetComponent<AudioSource>();
            }
        }

        protected void Start()
        {
            _destroyTimer = gameObject.AddComponent<Timer>();

            const float SECONDS_TO_DESTROY = 7f;
            _destroyTimer.time = SECONDS_TO_DESTROY;
            _destroyTimer.EndEvent += Kill;
            _destroyTimer.StartTimer();
        }

        protected void OnEnable()
        {
            if (_destroyTimer != null)
            {
                _destroyTimer.RestartTimer();
            }
        }

        protected void OnDisable()
        {
            if (_destroyTimer != null)
            {
                _destroyTimer.StopTimer();
            }
        }

        protected void Update()
        {
            if (_directionChangeLerp < 1 && !GlobalSettings.IsPaused)
            {
                _directionChangeLerp += Time.deltaTime * m_initialSpeedModifier;
            }
        }

        protected void FixedUpdate()
        {
            if (GlobalSettings.IsPaused)
            {
                Rigidbody.velocity = Vector2.zero;
                return;
            }

            if (PlayerController.Player != null)
            {
                var playerPosition = PlayerController.Player.transform.position;
                var currentPosition = transform.position;

                if (Rigidbody.gravityScale > 0)
                {
                    Rigidbody.gravityScale = 0;
                }

                var distanceToPlayer = Vector3.Distance(currentPosition, playerPosition);

                if (distanceToPlayer < m_magnetRadius)
                {
                    Rigidbody.velocity = (playerPosition - transform.position).normalized * m_moveSpeedModifier;

                    const float TRIGGER_DISTANCE = 0.6f;
                    if (distanceToPlayer <= TRIGGER_DISTANCE)
                    {
                        if (BonusBehaviour != null)
                        {
                            BonusBehaviour.Use(this);
                        }
                        else
                        {
                            Debug.LogWarning($"No {nameof(BonusBehaviour)} on {nameof(Bonus)}!", this);
                            Kill();
                        }
                    }
                }
                else
                {
                    Rigidbody.velocity = Vector2.Lerp(INITIAL_FALL_DIRECTION, DEFAULT_FALL_DIRECTION, _directionChangeLerp);
                }
            }
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.forward, m_magnetRadius);
        }
#endif

        // APIs

        public void SpawnAt(Vector3 position, IBonusBehaviour bonusBehaviour)
        {
            transform.position = position;
            BonusBehaviour = bonusBehaviour;
            if (BonusBehaviour != null)
            {
                BonusBehaviour.Apply(this);
            }
        }

        public void Kill()
        {
            if (!BonusPool.TryRelease(this))
            {
                Destroy(gameObject, AudioSource.clip.length);
                Debug.LogError($"Spawned bullet without pool! Will call direct destroy", this);
                return;
            }

            BonusBehaviour = null;
        }
    }
}