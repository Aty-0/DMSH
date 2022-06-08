using System;

//TODO: Make functions to stop non DMSH Game elements 
public static class GlobalSettings
{
    public static bool  musicPlay = true;
    public static bool  mainMenuAwakeAnimation = true;
    public static int   gameActive = 1;
    public static bool  gameActiveBool => Convert.ToBoolean(gameActive);
}

