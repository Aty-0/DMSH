using DMSH.Characters.Animation;

using System.Collections;
using UnityEngine;

using DMSH.Path;
using DMSH.Misc;
using DMSH.Misc.Screen;
using DMSH.Misc.Animated;
using DMSH.Objects;
using DMSH.LevelSpecifics.Stage;
using DMSH.Gameplay;
using DMSH.Scripts.Controls;
using DMSH.UI;

using Scripts.Utils;
using Scripts.Utils.Pools;

namespace DMSH.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Weapon))]
    public class PlayerController : MovableObject
    {
        public static PlayerController Player { get; private set; }
        
        #region Public Values
        public int Life
        {
            get => _life;
            set
            {
                if (value >= PLAYER_MIN_LIFE)
                    _life = value;

                UpdateHUD();
            }
        }
        public int Boost
        {
            get => _boost;
            set
            {
                if (value >= PLAYER_MIN_LIFE)
                    _boost = value;

                UpdateHUD();
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;

                UpdateHUD();
            }
        }
        #endregion

        #region Constants
        public const int PLAYER_MAX_LIFE = 3;
        public const int PLAYER_MAX_BOOST = 5;
        public const int PLAYER_MIN_LIFE = -1;
        #endregion

        [Header("Global")]
        public ResizableGameElements resizableGameElements;

        [Header("Graphics")]
        [SerializeField]
        private FrameAnimator m_animator;
        public FrameAnimator Animator => m_animator;
        
        [SerializeField] 
        private Vector2 _moveDirection = Vector2.zero;
        public override Vector2 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        [HideInInspector] 
        [SerializeField] 
        private bool _isDead;

        [HideInInspector] 
        [SerializeField] 
        private CameraDeathAnimation _cameraDeathAnimation;

        [HideInInspector] 
        [SerializeField] 
        private StageSystem _stageSystem;

        [Header("Current statistics")]
        [SerializeField] 
        private int _score;
        [SerializeField] 
        private int _life = PLAYER_MAX_LIFE;
        [SerializeField] 
        private int _boost = PLAYER_MAX_BOOST;

        [Header("Boost")]
        [SerializeField] 
        private float _boost_speed = 0.05f;
        [SerializeField] 
        private float _saved_time_scale;

        public Weapon                   Weapon { get; private set; }

        [Header("Sounds")]
        [SerializeField] 
        #pragma warning disable CS0414
        // ReSharper disable once NotAccessedField.Local
        private AudioSource audioSourceWeapon = null;
        #pragma warning restore CS0414
        [SerializeField] 
        private AudioSource audioSourceDeath = null;
        [SerializeField] 
        private AudioSource audioSourceMusic = null;

        [Header("Particles")]
        [SerializeField] 
        protected ParticleSystem _deathParticle = null;

        private Coroutine _slowMotionCoroutine = null;
        private Coroutine _deathAwakeCoroutine = null;

        protected void OnEnable()
        {
            if (Player == null)
            {
                Player = this;
            }
        }

        protected void OnDisable()
        {
            if (Player == this)
            {
                Player = null;
            }
        }

        protected void Start()
        {            
            // First initialize
            PrepareComponents();
            UpdateHUD();
            UpdateSettings();

            // Set timer callback for stage system
            _stageSystem.onTimerStart.Add(() => UI_Root.Get.ShowChapterName($"Chapter {_stageSystem.CurrentStageIndex + 1} {_stageSystem.currentStage.name}"));
            _stageSystem.onTimerEnd.Add(() => UI_Root.Get.HideChapterName());
        }

        private void PrepareComponents()
        {
            // Get all components
            RigidBody2D = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<Collider2D>();
            Weapon = GetComponent<Weapon>();

            _stageSystem = StageSystem.Get;
            if (_stageSystem == null)
                Debug.LogError("No stage system in scene");

            if (TryGetComponent(out resizableGameElements) && resizableGameElements.enabled)
            {
                resizableGameElements.gameCamera = GameCamera.Get.Camera;
                resizableGameElements.Initialize();
                
                // Set respawnPoint position
                gameObject.transform.position = resizableGameElements.respawnPoint.transform.position;
            }

            _cameraDeathAnimation = gameObject.AddComponent<CameraDeathAnimation>();
            _cameraDeathAnimation.animCamera = GameCamera.Get.Camera;
            _cameraDeathAnimation.target = gameObject;

            // Don't show cursor when we are create the player 
            Cursor.visible = false;
        }

        protected void FixedUpdate()
        {
            var velocity = _moveDirection * _speed * GlobalSettings.GameActiveAsInt;

#if PREFFER_A_T_RESIZABLE_GAME_ELEMENTS
            _rigidBody2D.velocity = velocity;
 #else
            _rigidBody2D.MoveRigidbodyInsideScreen(velocity, GameCamera.Get.Camera);
#endif
        }

        private IEnumerator DoSlowMotion(bool isBoost = true)
        {
            while (Time.timeScale < 1.0f)
            {
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                Time.timeScale += GlobalSettings.GameActiveAsInt * _boost_speed;

                foreach (var sound in FindObjectsOfType<AudioSource>())
                {
                    if (!sound.gameObject.CompareTag("NotGenericSound"))
                    {
                        sound.pitch = Time.timeScale;
                    }
                }

                if (isBoost)
                {
                    UI_Root.Get.UI_BoostGainText.text = $"{(int)(Time.timeScale * 100)}%";
                }

                yield return new WaitForSeconds(.1f);
            }

            UI_Root.Get.UI_BoostGainText.text = "100%";

            Time.timeScale = 1.0f;
        }

        public void UseBoost()
        {
            if ((Boost <= 0 && !GlobalSettings.cheatInfiniteBoost) || Time.timeScale < 1.0f)
            {
                return;
            }
            foreach (Bullet bullet in FindObjectsOfType<Bullet>())
            {
                if (bullet.IsEnemyBullet &&
                    bullet.IsCollisionDestroyBullet)
                {
                    bullet.SqueezeAndDestroy();
                }
            }
            Boost--;
            Time.timeScale = 0.05f;
            _slowMotionCoroutine = StartCoroutine(DoSlowMotion());
        }


        public void ShowDeathScreen()
        {
            Cursor.visible = true;

            // Disable all sounds in scene
            foreach (var sound in FindObjectsOfType<AudioSource>())
            {
                if (!sound.gameObject.CompareTag("NotGenericSound"))
                {
                    sound.Stop();
                }
            }

            // TODO: Change track 
            audioSourceMusic.Stop();
             
            // Show death screen
            UI_Root.Get.SetDeathScreen(true, StringUtils.GetNumberWithZeros(Score));

            // Stop game world
            Time.timeScale = 1.0f;
            GlobalSettings.SetGameActive(false);

            // Show some results
            //_uiMaxScoreText.text += GetNumberWithZeros(maxScore);
        }

        public void ShowPauseScreen()
        {
            // Save the last time scale state
            if (!UI_Root.Get.IsPauseMenuOpened)
            {
                _saved_time_scale = Time.timeScale;
            }

            // Enable or disable pause menu
            UI_Root.Get.TogglePauseScreen();
            Time.timeScale = UI_Root.Get.IsPauseMenuOpened
                ? 1f
                : _saved_time_scale;

            // TODO: Change track 
            if (!UI_Root.Get.IsPauseMenuOpened)
            {
                // Enable boost if we are exit from pause menu and if we hasnt enable boost
                // in game we are skip the loop because loop work if Time.timeScale < 1.0f
                _slowMotionCoroutine = StartCoroutine(DoSlowMotion());

                // Enable death animation
                if (Animator.SpriteRenderer.color.a < 0.9f)
                {
                    _deathAwakeCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeSprite(Animator.SpriteRenderer));
                }

                audioSourceMusic.Play();
            }
            else
            {
                Weapon.StopShooting();

                if (_deathAwakeCoroutine != null)
                {
                    StopCoroutine(_deathAwakeCoroutine);
                }

                // If we have enabled boost
                if (_slowMotionCoroutine != null)
                {
                    StopCoroutine(_slowMotionCoroutine);
                }

                audioSourceMusic.Pause();
            }

            var plInput = PlayerControl.Get.Input;
            plInput.currentActionMap.Disable();
            plInput.SwitchCurrentActionMap(UI_Root.Get.IsPauseMenuOpened
                ? "Pause"
                : "Player");
            plInput.currentActionMap.Enable();

            UpdateSettings();
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (GlobalSettings.cheatGod)
            {
                GUI.Label(new Rect(0, 60, 500, 500), "[God]", UI_Root.Get.CheatGUIStyle);
            }

            if (GlobalSettings.cheatInfiniteBoost)
            {
                GUI.Label(new Rect(0, 80, 500, 500), "[Infinity boost]", UI_Root.Get.CheatGUIStyle);
            }

            if (GlobalSettings.debugDrawPlayerDGUI)
            {
                GUI.Label(new Rect(100, 80, 500, 500),  $"DeltaTime: {Time.deltaTime}");
                GUI.Label(new Rect(100, 120, 500, 500), $"Position: {RigidBody2D.position}");
                GUI.Label(new Rect(100, 140, 500, 500), $"Velocity: {RigidBody2D.velocity}");
                GUI.Label(new Rect(100, 280, 500, 500), $"Time scale: {Time.timeScale}");
                GUI.Label(new Rect(100, 300, 500, 500), $"Saved time scale: {_saved_time_scale}");
                GUI.Label(new Rect(100, 320, 500, 500), $"gamePaused: {GlobalSettings.IsPaused}");
                GUI.Label(new Rect(100, 340, 500, 500), $"weaponUpgradeGain: {Weapon.weaponUpgradeGain}");
            }
        }
#endif

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_isDead)
            {
                if (ProjectilePool.TryGetByGo(collision.gameObject, out var collidedBullet)
                    && collidedBullet.IsEnemyBullet)
                {
                    Damage();
                }
                else if(collision.gameObject.TryGetComponent<IMovableObject>(out var movable) && movable is Enemy)
                {
                    // TODO get rid of GetComponent
                    Damage();
                }
            }
        }

        public void Kill()
        {
            if (_cameraDeathAnimation != null)
            {
                _cameraDeathAnimation.Play();
            }

            if (_slowMotionCoroutine != null)
            {
                StopCoroutine(_slowMotionCoroutine);
            }

            Weapon.StopShooting();

            Life = PLAYER_MIN_LIFE;

            _isDead = true;
            Animator.SpriteRenderer.enabled = false;
            Collider2D.enabled = false;
            RigidBody2D.isKinematic = true;
            PlayerControl.Get.Input.enabled = false;

            // Show death screen 
            ShowDeathScreen();
        }

        public void Damage()
        {
            // Play player death sound 
            audioSourceDeath.Play();

            if (_deathParticle)
            {
                var particleModule = _deathParticle.main;
                // TODO store it in variable
                particleModule.startColor =Color.red; 
                _deathParticle.transform.position = RigidBody2D.transform.position;
                _deathParticle.Play();
            }

            // Don't continue if we are dead or in god mode
            if (/*_debug_god || */_isDead == true)
                return;

            // Subtract one life point 
            if (!GlobalSettings.cheatGod)
            {
                Life -= 1;
            }


            // If life equal or less MIN_LIFE we are disable player components
            // Or we are teleport player to spawn point
            if (Life == PLAYER_MIN_LIFE)
            {
                Kill();
            }
            else
            {
                _deathAwakeCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeSprite(Animator.SpriteRenderer));

                // Make everything slow
                // Slow motion gameplay kekw
                Time.timeScale = 0.2f;
                _slowMotionCoroutine = StartCoroutine(DoSlowMotion(false));

                // Destroy all bullet cuz we are can teleport player into the bullet 
                foreach (var bullet in FindObjectsOfType<Bullet>())
                {
                    if (bullet.IsEnemyBullet &&
                        bullet.IsCollisionDestroyBullet)
                    {
                        bullet.SqueezeAndDestroy();
                    }
                }
                // Set spawn point position to player
                if (resizableGameElements != null)
                {
                    gameObject.transform.position = resizableGameElements.respawnPoint.transform.position;
                }
            }
        }

        private void UpdateHUD()
        {
            UI_Root.Get.UpdateGameHud(Life, StringUtils.GetNumberWithZeros(Score), Boost);
        }

        // That thing should update some player components
        private void UpdateSettings()
        {
            audioSourceMusic.enabled = GlobalSettings.musicPlay;
        }
    }
}

