using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEditor;

// TODO: 1. Make functions to stop non DMSH Game elements 
//       2. Read settings from game.json or something like that
//       3. Save the current settings

namespace DMSH.Misc
{
    [Serializable]
    public static class GlobalSettings
    {
        public static bool musicPlay = true;
        public static bool mainMenuAwakeAnimation = true;

        private static bool _gameActive = true;

        public static int gameActiveAsInt
        {
            get => Convert.ToInt32(_gameActive);
        }

        //  TODO: and etc 
        //        Sounds volume, graphics preset

        public static bool musicPlay = true;
        public static bool mainMenuAwakeAnimation = true;

        // In this list we are collect all components before activate pause
        // private static List<GameObject> _gameobjectBeforePause = new List<GameObject>();

        // It's non serilized cuz we are constatly save the false state on pause
        // And on game load it's just stuck
        [NonSerialized] private static bool _gameActive = true;

        public static bool gameActiveAsBool
        {
            get => _gameActive;
        }

        public static void SetGameActive(bool gameActive)
        {
            _gameActive = gameActive;

            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                Component[] components = go.GetComponents<Component>();

                foreach (Component component in components)
                {
                    switch (component)
                    {
                        case Rigidbody2D r:
                            r.isKinematic = _gameActive;
                            break;
                        case TrailRenderer tr:
                            // TODO: How we can save the data ??
                            // tr.time = 0; <- it's stop the trail
                            tr.time = gameActiveAsInt;
                            break;

                            // TODO: Add another components

                if (true /* !_gameActive */) // TODO: Need to back previous properties, if we change something custom like time in trailRenderer
                {
                    switch (component)
                    {
                      case Rigidbody2D rigidbody:
                            rigidbody.simulated = _gameActive;                            
                            break;
                        case TrailRenderer trail:
                            trail.time = 0;
                            break;
                        case Animator animator:
                            if (_gameActive)
                                animator.StopPlayback();
                            else
                                animator.StartPlayback();
                            break;
                        default:
                            continue;
                            // TODO: Add another components
                    }
                }
            }
        }
    }
}