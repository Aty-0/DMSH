using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using DMSH.Path;
using DMSH.Misc;
using DMSH.Misc.Screen;
using DMSH.Misc.Animated;
using DMSH.Misc.Log;
using DMSH.Objects;
using DMSH.LevelSpecifics.Stage;
using DMSH.Gameplay;
using DMSH.UI;

namespace DMSH.Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(LogHandler))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Weapon))]

    public class PlayerController : MovableObject
    {
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
        public const int UI_ZEROS_SCORE_TEXT = 8;
        #endregion

        [Header("Global")]
        public SpriteRenderer   spriteRenderer = null;
        public Camera           gameCamera = null;
        public LogHandler       logHandler = null;
        public PlayerInput      playerInput = null;
        public ResizableGameElements resizableGameElements = null;

        [SerializeField] 
        private Vector2 _moveDirection = Vector2.zero;

        [HideInInspector] 
        [SerializeField] 
        private bool _isDead = false;

        [HideInInspector] 
        [SerializeField] 
        private CameraDeathAnimation _cameraDeathAnimation = null;

        [HideInInspector] 
        [SerializeField] 
        private StageSystem _stageSystem = null;

        [Header("Current statistics")]
        [SerializeField] 
        private int _score = 0;
        [SerializeField] 
        private int _life = PLAYER_MAX_LIFE;
        [SerializeField] 
        private int _boost = PLAYER_MAX_BOOST;

        [Header("Boost")]
        [SerializeField] 
        private float _boost_speed = 0.05f;
        [SerializeField] 
        private float _saved_time_scale = 0.0f;

        [Header("Weapon")]
        public Weapon                   weapon;

        [Header("Sounds")]
        [SerializeField] 
        private AudioSource audioSourceWeapon = null;
        [SerializeField] 
        private AudioSource audioSourceDeath = null;
        [SerializeField] 
        private AudioSource audioSourceMusic = null;

        [Header("Particles")]
        [SerializeField] 
        protected ParticleSystem _deathParticle = null;

        private Coroutine _showChapterNameCoroutine = null;
        private Coroutine _slowMotionCoroutine = null;
        private Coroutine _deathAwakeCoroutine = null;

        protected void Start()
        {
            // First initialize
            PrepareComponents();
            UpdateHUD();
            UpdateSettings();

            // Set timer callback for stage system
            _stageSystem.onTimerStart.Add(ShowChapterName);
            _stageSystem.onTimerEnd.Add(RemoveChapterName);
        }

        private void PrepareComponents()
        {
            // Get all components
            gameCamera = GetComponentInParent(typeof(Camera)) as Camera;
            spriteRenderer = GetComponent<SpriteRenderer>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            playerInput = GetComponent<PlayerInput>();
            logHandler = GetComponent<LogHandler>();
            weapon = GetComponent<Weapon>();
            _stageSystem = FindObjectOfType<StageSystem>();

            resizableGameElements = GetComponent<ResizableGameElements>();
            resizableGameElements.gameCamera = gameCamera;
            resizableGameElements.Initialize();

            _cameraDeathAnimation = gameObject.AddComponent<CameraDeathAnimation>();
            _cameraDeathAnimation.animCamera = gameCamera;
            _cameraDeathAnimation.target = gameObject;

            // Don't show cursor when we are create the player 
            Cursor.visible = false;

            // Set respawnPoint position
            gameObject.transform.position = resizableGameElements.respawnPoint.transform.position;

        }
        public void ShowChapterName()
        {
            _showChapterNameCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeText(UI_Root.Get.UI_ChapterName, 255, 15));
            UI_Root.Get.UI_ChapterName.text = $"Chapter {_stageSystem.CurrentStageIndex + 1} {_stageSystem.currentStage.name}";
        }

        public void RemoveChapterName()
        {
            if (_showChapterNameCoroutine != null)
            {
                StopCoroutine(_showChapterNameCoroutine);
            }

            _showChapterNameCoroutine = StartCoroutine(BasicAnimationsPack.SmoothFadeText(UI_Root.Get.UI_ChapterName, 15));
        }

        protected void FixedUpdate()
        {
            _rigidBody2D.MovePosition(_rigidBody2D.position + ((_moveDirection * _speed) * Time.fixedDeltaTime * GlobalSettings.gameActiveAsInt));
        }

        private IEnumerator DoSlowMotion(bool isBoost = true)
        {
            while (Time.timeScale < 1.0f)
            {
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                Time.timeScale += GlobalSettings.gameActiveAsInt * _boost_speed;

                foreach (var sound in FindObjectsOfType<AudioSource>())
                {
                    if (sound.gameObject.tag != "NotGenericSound")
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
                if (bullet.isEnemyBullet &&
                    bullet.collisionDestroyBullet &&
                    bullet.pathSystem == null)
                {
                    bullet.SqueezeAndDestroy();
                }
            }
            Boost--;
            Time.timeScale = 0.05f;
            _slowMotionCoroutine = StartCoroutine(DoSlowMotion());
        }

        private void OnUseBoost(InputValue input)
        {
            if (GlobalSettings.gameActiveAsBool)
            {
                UseBoost();
            }
        }

        private void OnShot(InputValue input)
        {
            if (GlobalSettings.gameActiveAsBool && input.isPressed)
            {
                weapon.Shot();
            }
            else
            {
                weapon.StopShooting();
            }
        }

        private void OnMoveH(InputValue input)
        {
            _moveDirection.x = input.Get<Vector2>().x;
        }

        private void OnMoveV(InputValue input)
        {
            _moveDirection.y = input.Get<Vector2>().y;
        }

        private void OnPause(InputValue input)
        {
            ShowPauseScreen();
        }

        public void ShowDeathScreen()
        {
            Cursor.visible = true;

            // Disable all sounds in scene
            foreach (var sound in FindObjectsOfType<AudioSource>())
            {
                if (sound.gameObject.tag != "NotGenericSound")
                {
                    sound.Stop();
                }
            }

            // TODO: Change track 
            audioSourceMusic.Stop();
             
            // Show death screen
            UI_Root.Get.UI_DeathScreen.SetActive(!UI_Root.Get.UI_DeathScreen.activeSelf);

            // Stop game world
            Time.timeScale = 1.0f;
            GlobalSettings.SetGameActive(false);

            // Show some results
            UI_Root.Get.UI_CurrentScoreOnDeathText.text += GetNumberWithZeros(Score);
            //_uiMaxScoreText.text += GetNumberWithZeros(maxScore);
        }

        public void ShowPauseScreen()
        {
            // Save the last time scale state
            if (UI_Root.Get.UI_PauseScreen.activeSelf == false)
            {
                _saved_time_scale = Time.timeScale;
            }

            // Enable or disable pause menu
            UI_Root.Get.UI_PauseScreen.SetActive(!UI_Root.Get.UI_PauseScreen.activeSelf);
            Cursor.visible = UI_Root.Get.UI_PauseScreen.activeSelf;
            GlobalSettings.SetGameActive(!UI_Root.Get.UI_PauseScreen.activeSelf);

            Time.timeScale = UI_Root.Get.UI_PauseScreen.activeSelf == false ? _saved_time_scale : 1.0f;

            // TODO: Change track 
            if (UI_Root.Get.UI_PauseScreen.activeSelf == false)
            {
                // Enable boost if we are exit from pause menu and if we hasnt enable boost
                // in game we are skip the loop because loop work if Time.timeScale < 1.0f
                _slowMotionCoroutine = StartCoroutine(DoSlowMotion());

                // Enable death animation
                if (spriteRenderer.color.a < 0.9f)
                {
                    _deathAwakeCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeSprite(spriteRenderer));
                }

                audioSourceMusic.Play();
            }
            else
            {
                weapon.StopShooting();

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

            playerInput.currentActionMap.Disable();
            playerInput.SwitchCurrentActionMap(UI_Root.Get.UI_PauseScreen.activeSelf == false ? "Player" : "Pause");
            playerInput.currentActionMap.Enable();

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
                GUI.Label(new Rect(100, 120, 500, 500), $"Position: {rigidBody2D.position}");
                GUI.Label(new Rect(100, 140, 500, 500), $"Velocity: {rigidBody2D.velocity}");
                GUI.Label(new Rect(100, 200, 500, 500), $"WeaponEnabled: {weapon.weaponEnabled}");
                GUI.Label(new Rect(100, 280, 500, 500), $"Time scale: {Time.timeScale}");
                GUI.Label(new Rect(100, 300, 500, 500), $"Saved time scale: {_saved_time_scale}");
                GUI.Label(new Rect(100, 320, 500, 500), $"gameActive: {GlobalSettings.gameActiveAsBool}");
                GUI.Label(new Rect(100, 340, 500, 500), $"WeaponBoostGain: {weapon.weaponBoostGain}");
                GUI.Label(new Rect(100, 360, 500, 500), $"WeaponType: {weapon.weaponType}");
            }
        }
#endif

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!audioSourceDeath.isPlaying)
            {
                Component[] components = collision.gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    switch (component)
                    {
                        case Enemy b:
                            Damage();
                            break;
                        case Bullet b:
                            if (b.isEnemyBullet)
                            {
                                Damage();
                            }
                            break;
                    }
                }
            }
        }

        public void Kill()
        {
            _cameraDeathAnimation.Play();

            if (_slowMotionCoroutine != null)
            {
                StopCoroutine(_slowMotionCoroutine);
            }

            weapon.StopShooting();

            Life = PLAYER_MIN_LIFE;

            _isDead = true;
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
            rigidBody2D.isKinematic = true;
            playerInput.enabled = false;

            // Show death screen 
            ShowDeathScreen();
        }

        public void Damage()
        {
            // Play player death sound 
            audioSourceDeath.Play();

            if (_deathParticle)
            {
                ParticleSystemRenderer pr = _deathParticle.GetComponent<ParticleSystemRenderer>();
                pr.material.color = Color.red;
                _deathParticle.transform.position = rigidBody2D.transform.position;
                _deathParticle.Play();
            }

            // Don't continue if we are dead or in god mode
            if (/*_debug_god || */_isDead == true)
            {
                return;
            }

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
                _deathAwakeCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeSprite(spriteRenderer));

                // Make everything slow
                // Slow motion gameplay kekw
                Time.timeScale = 0.2f;
                _slowMotionCoroutine = StartCoroutine(DoSlowMotion(false));

                // Destroy all bullet cuz we are can teleport player into the bullet 
                foreach (var bullet in FindObjectsOfType<Bullet>())
                {
                    if (bullet.isEnemyBullet &&
                        bullet.collisionDestroyBullet &&
                        bullet.pathSystem == null)
                    {
                        bullet.SqueezeAndDestroy();
                    }
                }
                // Set spawn point position to player 
                gameObject.transform.position = resizableGameElements.respawnPoint.transform.position;
            }
        }

        // Basic string tool to fill string with number also zeros 
        public string GetNumberWithZeros(int num)
        {
            // Initialize empty string
            string text = string.Empty;
            // Fill string by UI_ZEROS_SCORE_TEXT count subtract number length 
            for (int i = 0; i <= UI_ZEROS_SCORE_TEXT - num.ToString().Length; i++)
            {
                text += "0";
            }
            // Add number
            text += num.ToString();
            return text;
        }

        public void UpdateHUD()
        {
            var ui = UI_Root.Get;
            
            ui.UI_LifeText.text = Life.ToString();
            ui.UI_ScoreText.text = GetNumberWithZeros(Score);
            ui.UI_BoostText.text = Boost.ToString();
        }

        // That thing should update some player components
        public void UpdateSettings()
        {
            audioSourceMusic.enabled = GlobalSettings.musicPlay;
        }
    }
}

