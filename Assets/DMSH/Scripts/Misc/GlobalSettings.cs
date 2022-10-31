using DMSH.Characters;

using System;

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
        public static bool debugDrawInvWallSI = false;
        public static bool debugDrawWeaponPoints = false;
        public static bool debugDrawPSObjectInfo = false;
        public static bool debugDrawPSAllPoints = false;
        public static bool debugDrawPSCurrentMovement = false;
        public static bool debugDrawPlayerDGUI = false;

        public static bool cheatGod = false;
        public static bool cheatInfiniteBoost = false;

        public static bool musicPlay = true;
        public static bool mainMenuAwakeAnimation = true;

        // It's non serilized cuz we are constatly save the false state on pause
        // And on game load it's just stuck
        [NonSerialized]
        private static bool _gameActive = true;

        public static int gameActiveAsInt => _gameActive ? 1 : 0;
        public static bool gameActiveAsBool => _gameActive;


        public static void SetGameActive(bool gameActive)
        {
            if (_gameActive == gameActive)
                return;

            _gameActive = gameActive;

            if (_gameActive)
            {
                PlayerController.Player.Animator.StopPlayback();
            }
            else
            {
                PlayerController.Player.Animator.StartPlayback();
            }
        }
    }
}