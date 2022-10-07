using DMSH.Misc;
using DMSH.Misc.Animated;

using Scripts.Utils;

using System;

using UnityEngine;
using UnityEngine.UI;

namespace DMSH.UI
{
    public class UI_Root : Instance<UI_Root>
    {
        [Header("UI")]
        [SerializeField]
        private Text _uiScoreText = null;
        
        [SerializeField]
        private Text _uiBoostGainText = null;
        public Text UI_BoostGainText => _uiBoostGainText;

        [SerializeField]
        private Text _uiBoostText = null;
        
        [SerializeField]
        private Text _uiLifeText = null;
        
        [SerializeField]
        private Text _uiFpsCounterText = null;

        [SerializeField]
        private Text _uiChapterName = null;

        [SerializeField]
        private GUIStyle _cheatGUIStyle = null;
        public GUIStyle CheatGUIStyle => _cheatGUIStyle; 

        [Header("UI Screens")]
        [SerializeField]
        private GameObject _uiPauseScreen = null;
        public bool IsPauseMenuOpened => _uiPauseScreen.activeSelf;
        
        [SerializeField]
        private GameObject _uiDeathScreen = null;
        
        [Tooltip("Only for death screen")]
        [SerializeField]
        private Text _uiCurrentScoreOnDeathText = null;
        
        [SerializeField]
        private Text _uiMaxScoreText = null;

        private Coroutine _showChapterNameCoroutine = null;

        // unity
        
        protected void Start()
        {
            _uiBoostGainText.text = "100%";

            // Set style for cheat gui
            _cheatGUIStyle.fontSize = 13;
            _cheatGUIStyle.normal.textColor = new Color(255, 0, 0);
        }
        
        protected void Update()
        {
            if (_uiFpsCounterText != null)
            {
                _uiFpsCounterText.text = $"FPS:{(int)(1f / Time.unscaledDeltaTime)}";
            }
        }

        // pubic APIs

        public void ShowChapterName(string chapterName)
        {
            _showChapterNameCoroutine = StartCoroutine(BasicAnimationsPack.SmoothAwakeText(_uiChapterName, 255, 15));
            _uiChapterName.text = chapterName;
        }
        
        public void HideChapterName()
        {
            if (_showChapterNameCoroutine != null)
            {
                StopCoroutine(_showChapterNameCoroutine);
            }

            _showChapterNameCoroutine = StartCoroutine(BasicAnimationsPack.SmoothFadeText(_uiChapterName, 15));
        }

        public void TogglePauseScreen()
        {
            _uiPauseScreen.SetActive(!IsPauseMenuOpened);
            Cursor.visible = IsPauseMenuOpened;
            GlobalSettings.SetGameActive(!IsPauseMenuOpened);
        }

        public void SetDeathScreen(bool shouldBeVisible, string score = null)
        {
            _uiDeathScreen.SetActive(shouldBeVisible);
            if (!string.IsNullOrEmpty(score))
            {
                _uiCurrentScoreOnDeathText.text = score;
            }
        }

        public void UpdateGameHud(int life, string numberWithZeros, int boostValue)
        {
            _uiLifeText.text = life.ToString();
            _uiScoreText.text = numberWithZeros;
            _uiBoostText.text = boostValue.ToString();
        }
    }
}