using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonRetry : MonoBehaviour
{
    public void ButtonRetryEvent()
    {
        Time.timeScale = 1.0f;
        GlobalSettings.gameActive = 1;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
