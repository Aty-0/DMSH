using System;
using UnityEngine;

// TODO: 1. Make functions to stop non DMSH Game elements 
//       2. Read settings from game.json or something like that
//       3. Save the current settings [Save volume, graphics preset, max scores]

namespace DMSH.Misc
{    
    [Serializable]
    public static class GlobalSettings
    {
        // TODO:
        // Works only in unity editor, because we use gizmos
        public static bool debugDrawWeaponPoints        = false;
        public static bool debugDrawPSObjectInfo        = false;
        public static bool debugDrawPSAllPoints         = false;
        public static bool debugDrawPSCurrentMovement   = false;
        public static bool debugDrawPlayerDGUI = false;

        public static bool cheatGod = false;
        public static bool cheatInfiniteBoost = false;

        public static bool musicPlay = true;
        public static bool mainMenuAwakeAnimation = true;

        // It's non serilized cuz we are constatly save the false state on pause
        // And on game load it's just stuck
        [NonSerialized] private static bool _gameActive = true;

        public static int  gameActiveAsInt { get => Convert.ToInt32(_gameActive); }
        public static bool gameActiveAsBool { get => _gameActive; }


        public static void SetGameActive(bool gameActive)
        {
            _gameActive = gameActive;

            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                Component[] components = go.GetComponents<Component>();

                foreach (Component component in components)
                {
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
}
