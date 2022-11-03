using System;

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