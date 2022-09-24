using System;
using System.Collections.Generic;
using UnityEngine;

namespace DMSH.Misc
{
    public class ScreenHandler : MonoBehaviour
    {
        public List<Action> onScreenResolutionChange = new List<Action>();

        [Header("Resoulution")]
        [SerializeField] private int _lastScreenWidth = 0;
        [SerializeField] private int _lastScreenHeight = 0;

        protected void Update()
        {
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                // Rewrite last screen width and height
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;

                // Invoke events 
                foreach (Action action in onScreenResolutionChange)
                    action?.Invoke();
            }
        }
    }
}
