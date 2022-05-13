using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEvents : MonoBehaviour
{
    public static void GlobalEventAllEnemyStartShot()
    {
        foreach (Enemy e in FindObjectsOfType<Enemy>())
        {
            e.StartShot();
        }
    }

    public static void GlobalEventAllEnemyStopShot()
    {
        foreach (Enemy e in FindObjectsOfType<Enemy>())
        {
            e.StopShot();
        }
    }
}


