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
        public Text UI_ScoreText => _uiScoreText; 
        
        [SerializeField]
        private Text _uiBoostGainText = null;
        public Text UI_BoostGainText => _uiBoostGainText;

        [SerializeField]
        private Text _uiBoostText = null;
        public Text UI_BoostText => _uiBoostText; 
        
        [SerializeField]
        private Text _uiLifeText = null;
        public Text UI_LifeText => _uiLifeText;
        
        [SerializeField]
        private Text _uiFpsCounterText = null;

        [SerializeField]
        private Text _uiChapterName = null;
        public Text UI_ChapterName => _uiChapterName;

        [SerializeField]
        private GUIStyle _cheatGUIStyle = null;
        public GUIStyle CheatGUIStyle => _cheatGUIStyle; 

        [Header("UI Screens")]
        [SerializeField]
        private GameObject _uiPauseScreen = null;
        public GameObject UI_PauseScreen => _uiPauseScreen;
        
        [SerializeField]
        private GameObject _uiDeathScreen = null;
        public GameObject UI_DeathScreen => _uiDeathScreen;
        
        [SerializeField]
        private Text _uiCurrentScoreOnDeathText = null; // Only for death screen
        public Text UI_CurrentScoreOnDeathText => _uiCurrentScoreOnDeathText; 
        
        [SerializeField]
        private Text _uiMaxScoreText = null;

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

        // data

        // public struct 
    }
}