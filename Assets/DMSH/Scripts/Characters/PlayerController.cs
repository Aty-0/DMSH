using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//TODO 
//Add the IMovableObject 

public class PlayerController : MonoBehaviour
{
    [Header("Global")]
    public GameObject RespawnPoint;
    public int        MaxScore = 0;

    #region Public Values
    public int Life
    {
        get => _player_life;
        set
        {
            if (value >= PLAYER_MIN_LIFE)
                _player_life = value;

            UpdateHUD();
        }
    }
    public int Boost
    {
        get => _player_boost;
        set
        {
            if (value >= PLAYER_MIN_LIFE)
                _player_boost = value;

            UpdateHUD();
        }
    }

    public int Score
    {
        get => _player_score;
        set
        {
            _player_score = value;

            UpdateHUD();
        }
    }

    public bool CheatGod
    {
        get => _debug_god;
        set
        {
            _debug_god = value;
        }
    }

    public bool CheatInfBoost
    {
        get => _debug_inf_boost;
        set
        {
            _debug_inf_boost = value;
        }
    }
    #endregion

    [Header("Constants")]
    public const int PLAYER_MAX_LIFE = 3;
    public const int PLAYER_MAX_BOOST = 5;
    public const int PLAYER_MIN_LIFE = -1;
    public const int UI_ZEROS_SCORE_TEXT = 8;

    [Header("Statistics")]
    [SerializeField] private int _player_score = 0;
    [SerializeField] private int _player_life = PLAYER_MAX_LIFE;
    [SerializeField] private int _player_boost = PLAYER_MAX_BOOST;
    
    [Header("Boost")]
    [SerializeField] private float _boost_speed = 0.05f;
    [SerializeField] private float _saved_time_scale = 0.0f;

    [Header("Main")]
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _sprite_renderer;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BoxCollider2D _boxcollider2D;
    [SerializeField] private PlayerInput _playerInput;

    [Header("Cheats & Debug")]
    public bool debugLog = false;
    [SerializeField] private bool _debug_gui;
    [SerializeField] private bool _debug_god;
    [SerializeField] private bool _debug_inf_boost;
    [SerializeField] private GUIStyle _cheat_gui_style;

    [Header("UI")]
    [SerializeField] private Text _score_text;
    [SerializeField] private Text _boost_gain_text;
    [SerializeField] private Text _life_text;
    [SerializeField] private Text _boost_text;
    [SerializeField] private Text _fps_counter_text;
    [SerializeField] private Text _max_score;
    [SerializeField] private Text _current_score;


    [Header("Stage")]
    [SerializeField] private StageSystem _stageSystem;
  
    [SerializeField] private Text       _uiStagePass;
    [SerializeField] private Text       _uiStageClear;

    [Header("UI Screens")]
    [SerializeField] private GameObject _pause_screen;
    [SerializeField] private GameObject _death_screen;

    [Header("Movement")]
    [SerializeField] private Vector2 _move;
    [SerializeField] private float _speed;

    [Header("Weapon")]
    public bool WeaponEnabled;
    [SerializeField] private Bullet _bulletprefab;
    [SerializeField] private float  _shot_frequency = 0.1f;

    [Header("Walls")]
    [SerializeField] private GameObject[] _walls_list = new GameObject[4];
    [SerializeField] private Image        _ui_some_image;

    [Header("Sound")]
    [SerializeField] private AudioSource _weapon_audio_source;
    [SerializeField] private AudioSource _death_audio_source;
    [SerializeField] private AudioSource _music_audio_source;

    private bool isDead = false;
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    protected void Start()
    {
        _camera             = gameObject.GetComponentInParent(typeof(Camera)) as Camera;
        _sprite_renderer    = GetComponent<SpriteRenderer>();
        _rigidbody2D        = GetComponent<Rigidbody2D>();
        _boxcollider2D      = GetComponent<BoxCollider2D>();
        _playerInput        = GetComponent<PlayerInput>();
        _stageSystem        = FindObjectOfType<StageSystem>();

        _stageSystem.onTimerStart.Add(ShowStageStatus);
        _stageSystem.onTimerEnd.Add(CloseStageStatus);

        _cheat_gui_style.fontSize = 13;
        _cheat_gui_style.normal.textColor = new Color(255, 0, 0);

        //First initialize
        _boost_gain_text.text = "100%";

        UpdateHUD();
        GenerateInvisibleWalls();
        UpdateSettings();
    }

    private Coroutine _uiStagePassCoroutine = null;
    private Coroutine _uiStageClearCoroutine = null;

    public void ShowStageStatus()
    {
        _uiStagePassCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeText(_uiStagePass));
        if (Life == PLAYER_MAX_LIFE)
            _uiStageClearCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeText(_uiStageClear));       
    }

    public void CloseStageStatus()
    {
        StopCoroutine(_uiStagePassCoroutine);
        if(_uiStageClearCoroutine != null)
            StopCoroutine(_uiStageClearCoroutine);

        _uiStagePassCoroutine = StartCoroutine(BasicAnimationsPack.SmoothFadeText(_uiStagePass));
        if (Life == PLAYER_MAX_LIFE)
            _uiStageClearCoroutine = StartCoroutine(BasicAnimationsPack.SmoothFadeText(_uiStageClear));
    }

    private void GenerateInvisibleWalls()
    {
        for (int i = 0; i <= 3; i++)
        {
            _walls_list[i] = new GameObject($"GeneratedInvisibleWall{i}");
            var local_boxCollider2D = _walls_list[i].AddComponent<BoxCollider2D>();
            local_boxCollider2D.size = Camera.main.ViewportToWorldPoint(i <= 1 ? new Vector2(1, 0) : new Vector2(0, 1)) * 2;
            local_boxCollider2D.size += i <= 1 ? new Vector2(0.0f, 0.1f) : new Vector2(0.1f, 0.0f);
            _walls_list[i].layer = 8;
        }

        UpdateInvisibleWallsPosition();
    }

    protected void FixedUpdate()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + (_move * _speed) * Time.fixedDeltaTime);
    }

    protected void Update()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
            OnResolutionScreenChange();
        _fps_counter_text.text = $"FPS:{(int)(1f / Time.unscaledDeltaTime)}";
    }

    //TODO
    //Need to restretch background
    private void UpdateInvisibleWallsPosition()
    {
        Vector3 ViewportToWorldPointX = Camera.main.ViewportToWorldPoint(new Vector2(1, 0));
        Vector3 ViewportToWorldPointY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1));

        ViewportToWorldPointX.y = 0;
        ViewportToWorldPointY.x = 0;


        _walls_list[0].transform.position = ViewportToWorldPointY;
        _walls_list[1].transform.position = -ViewportToWorldPointY;
        _walls_list[2].transform.position = ViewportToWorldPointX - new Vector3(ViewportToWorldPointX.x * _ui_some_image.rectTransform.sizeDelta.x * 20.0f, 0, 0);
        _walls_list[3].transform.position = -ViewportToWorldPointX;

        //FIX ME 
        //Sometimes player can bypass invisible walls when this function is called
        //It's collision bug but we need to avoid this


        //it's very dumb but that fix problem
        if(this.gameObject.transform.position.x > _walls_list[2].transform.position.x 
            || this.gameObject.transform.position.x < _walls_list[3].transform.position.x ||
            this.gameObject.transform.position.y > _walls_list[0].transform.position.y
            || this.gameObject.transform.position.y < _walls_list[1].transform.position.y)
                this.gameObject.transform.position = RespawnPoint.transform.position;

        RespawnPoint.transform.position = new Vector2(-ViewportToWorldPointX.x / 1000, -ViewportToWorldPointY.y / 1.2f);
    }

    private void OnResolutionScreenChange()
    {
        UpdateInvisibleWallsPosition();
    }
    private IEnumerator Shot()
    {
        while (WeaponEnabled)
        {
            Vector2 final_pos = new Vector2(_rigidbody2D.position.x, _rigidbody2D.position.y + _boxcollider2D.size.y);
            Instantiate(_bulletprefab, final_pos, Quaternion.identity);

            _weapon_audio_source.Play();

            yield return new WaitForSeconds(_shot_frequency);
        }
    }

    private IEnumerator DoBoostSlowMotion()
    {
        while (Time.timeScale < 1.0f)
        {
            //FIX ME 
            //If we are use boost multiple times
            //Time.timeScale restore get faster 
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            Time.timeScale += _boost_speed;

            foreach (AudioSource s in FindObjectsOfType<AudioSource>())
                if(s.gameObject.tag != "NotGenericSound")
                    s.pitch = Time.timeScale;

            _boost_gain_text.text = $"{(int)(Time.timeScale * 100)}%";

            yield return new WaitForSeconds(.1f);
        }

        _boost_gain_text.text = "100%";

        Time.timeScale = 1.0f;
    }

    public void UseBoost()
    {
        if ((Boost <= 0 && !_debug_inf_boost) || Time.timeScale < 1.0f)
            return;

        Boost--;
        Time.timeScale = 0.05f;
        StartCoroutine(DoBoostSlowMotion());
    }

    private void OnUseBoost(InputValue input)
    {
        UseBoost();
    }

    private void OnShot(InputValue input)
    {
        WeaponEnabled = input.isPressed;
        StartCoroutine(Shot());
    }

    private void OnMoveH(InputValue input)
    {
        _move.x = input.Get<Vector2>().x;
    }

    private void OnMoveV(InputValue input)
    {
        _move.y = input.Get<Vector2>().y;
    }

    private void OnPause(InputValue input)
    {
        ShowPauseScreen();
    }

    private void OnConsoleCall(InputValue input)
    {
        LogHandler.drawConsole = !LogHandler.drawConsole;
    }

    public void ShowDeathScreen()
    {
        //Disable all sounds in scene
        foreach (AudioSource s in FindObjectsOfType<AudioSource>())
            s.Stop();

        //Show death screen
        _death_screen.SetActive(!_death_screen.activeSelf);

        //Stop game world
        Time.timeScale = _death_screen.activeSelf == false ? 1.0f : 0.0f;

        //Show some results
        _current_score.text += GetNumberWithZeros(Score);
        _max_score.text += GetNumberWithZeros(MaxScore);

    }

    public void ShowPauseScreen()
    {
        WeaponEnabled = false;

        //Disable or play all sounds in scene
        //foreach (AudioSource s in FindObjectsOfType<AudioSource>())
        //    if(_pause_screen.activeSelf == false)
        //        s.Stop();
 

        //Save the last time scale state
        if (_pause_screen.activeSelf == false)
            _saved_time_scale = Time.timeScale;

        //If we have enabled boost
        StopCoroutine(DoBoostSlowMotion());

        //Enable pause menu
        _pause_screen.SetActive(!_pause_screen.activeSelf);
        Time.timeScale = _pause_screen.activeSelf == false ? _saved_time_scale : 0.0f;

        //Enable boost if we are exit from pause menu and
        //if we hasnt enable boost in game we are skip the loop because loop work if Time.timeScale < 1.0f
        if (_pause_screen.activeSelf == false)
            StartCoroutine(DoBoostSlowMotion());

        _playerInput.currentActionMap.Disable();
        _playerInput.SwitchCurrentActionMap(_pause_screen.activeSelf == false ? "Player" : "Pause");
        _playerInput.currentActionMap.Enable();

        UpdateSettings();
    }

    //Basic debug ui
    private void OnGUI()
    {
        //If someone wants to cheat :D
        if (_debug_god)            
            GUI.Label(new Rect(0, 60, 500, 500), "[God]", _cheat_gui_style);
        
        if (_debug_inf_boost)
            GUI.Label(new Rect(0, 80, 500, 500), "[Infinity boost]", _cheat_gui_style);
        
        if (_debug_gui)
        {
            GUIStyle fps_style_label = new GUIStyle();
            fps_style_label.fontSize = 20;
            fps_style_label.normal.textColor = new Color(255, 0, 0);

            ////////////////////////////////////////////////////////////////////////////////////////
            GUI.Label(new Rect(100, 40, 500, 500), "FPS: " + (int)(1f / Time.unscaledDeltaTime), fps_style_label);
            GUI.Label(new Rect(100, 80, 500, 500), "DeltaTime: " + Time.deltaTime);
            ////////////////////////////////////////////////////////////////////////////////////////
            GUI.Label(new Rect(100, 120, 500, 500), $"Position: {_rigidbody2D.position}");
            GUI.Label(new Rect(100, 140, 500, 500), $"Velocity: {_rigidbody2D.velocity}");
            GUI.Label(new Rect(100, 200, 500, 500), $"WeaponEnabled: {WeaponEnabled}");
            //GUI.Label(new Rect(100, 220, 500, 500), $"Life: {Life}");
            //GUI.Label(new Rect(100, 240, 500, 500), $"Score: {Score}");
            //GUI.Label(new Rect(100, 260, 500, 500), $"Boost: {_player_boost}");
            GUI.Label(new Rect(100, 280, 500, 500), $"Time scale: {Time.timeScale}");
            GUI.Label(new Rect(100, 300, 500, 500), $"Saved time scale: {_saved_time_scale}");



            GUI.Label(new Rect(200, 120, 500, 500), $"MusicPlay: {GlobalSettings.MusicPlay}");
        }      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //FIXME
        //_death_audio_source.isPlaying it's dumb idea to fix double damage when player is got collision with two bullets
        if (collision.gameObject.tag != "Bullet" || _death_audio_source.isPlaying)
            return;

        Debug.Log($"Player got damage from { collision.gameObject.name } at position { collision.gameObject.transform.position } with tag { collision.gameObject.tag }");
        Damage();
    }

    public void Damage()
    {
        //Play player death sound 
        _death_audio_source.Play();
       
        //Don't continue if we are dead or in god mode
        if (/*_debug_god || */isDead == true)
            return;

        //Subtract one life point 
        if(!_debug_god)
            Life -= 1;

        //TODO 
        //Need to play death animation

        //If life equal MIN_LIFE we are disable player components for move and body 
        //If we are still alive we are teleport player to spawn point
        if (Life == PLAYER_MIN_LIFE)
        {
            isDead = true;
            _sprite_renderer.enabled = false;
            _boxcollider2D.enabled = false;
            _rigidbody2D.isKinematic = true;
            WeaponEnabled = false;
            //TODO
            //Maybe we should change actions list
            _playerInput.enabled = false;
            //Show death screen 
            ShowDeathScreen();
        }
        else
        {
            //TODO
            //Maybe it's good idea to destroy bullets in spawn point radius

            //Destroy all bullet cuz we are can teleport player into bullet 
            foreach (Bullet bullet in FindObjectsOfType<Bullet>())
                if(bullet.collisionDestoryBullet)
                    Destroy(bullet.gameObject);

            //Set spawn point position to player 
            this.gameObject.transform.position = RespawnPoint.transform.position;
        }
    }
    
    //Basic string tool to fill string with number also zeros 
    public string GetNumberWithZeros(int num)
    {
        //Initialize empty string
        string text = string.Empty;
        //Fill string by UI_ZEROS_SCORE_TEXT count subtract number length 
        for (int i = 0; i <= UI_ZEROS_SCORE_TEXT - num.ToString().Length; i++)
            text += "0";
        //Add number
        text += num.ToString();
        return text;
    }

    public void UpdateHUD()
    {
        _life_text.text  = Life.ToString();
        _score_text.text = GetNumberWithZeros(Score);
        _boost_text.text = Boost.ToString();
    }

    //Thats update all attached to player game things 
    //Like a volume of music, playing state, or other player sound sources
    //Or something related to gameplay
    public void UpdateSettings()
    {
        _music_audio_source.enabled = GlobalSettings.MusicPlay;
    }
}
