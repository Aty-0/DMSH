using System;

public static class GlobalSettings
{
    public static bool  musicPlay = false;
    public static bool  mainMenuAwakeAnimation = true;
    public static int   gameActive = 1;
    public static bool  gameActiveBool => Convert.ToBoolean(gameActive);
}

