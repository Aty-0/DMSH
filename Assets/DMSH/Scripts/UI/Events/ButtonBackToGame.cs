using UnityEngine;
using DMSH.Characters;

namespace DMSH.UI.Events
{
    public class ButtonBackToGame : MonoBehaviour
    {
        [SerializeField] 
        private PlayerController _controller;
        
        public void ButtonBackToGameEvent()
        {
            if(_controller != null)
                _controller.ShowPauseScreen();
        }
    }
}