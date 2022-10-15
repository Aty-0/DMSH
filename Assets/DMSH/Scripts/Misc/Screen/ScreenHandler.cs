using System;
using System.Collections.Generic;
using UnityEngine;

namespace DMSH.Misc.Screen
{
    public class ScreenHandler : MonoBehaviour
    {
        public List<Action> onScreenResolutionChange = new List<Action>();
        public int Width => UnityEngine.Screen.width;
        public int Height => UnityEngine.Screen.height;

        [Header("Resoulution")]
        [SerializeField] 
        private int _lastScreenWidth = 0;
        [SerializeField] 
        private int _lastScreenHeight = 0;

        protected void Update()
        {
            if (_lastScreenWidth != Width || _lastScreenHeight != Height)
            {
                // Rewrite last screen width and height
                _lastScreenWidth = UnityEngine.Screen.width;
                _lastScreenHeight = UnityEngine.Screen.height;

                // Invoke events 
                foreach (Action action in onScreenResolutionChange)
                    action?.Invoke();
            }
        }
    }
}
