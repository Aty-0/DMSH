using System;

// TODO: 1. Make functions to stop non DMSH Game elements 
//       2. Read settings from game.json or something like that
//       3. Save the current settings [Save volume, graphics preset, max scores]

namespace DMSH.Misc
{
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

        public static bool IsPaused { get; private set; }
        public static int GameActiveAsInt => IsPaused ? 0 : 1;

        public static void SetGameActive(bool gameActive)
        {
            var wantedToSetPauseState = !gameActive;
            
            if (IsPaused == wantedToSetPauseState)
                return;

            IsPaused = wantedToSetPauseState;
        }
    }
}