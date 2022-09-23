using System;
using UnityEngine;

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
                    }
                }
            }
        }

    }
}