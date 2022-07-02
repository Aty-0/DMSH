using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBackToMainMenu : MonoBehaviour
{
    public ScenePicker scene;

    public void ButtonBackToMainMenuEvent()
    {
        Time.timeScale = 1.0f;
        GlobalSettings.SetGameActive(true);
        Cursor.visible = true;
        SceneManager.LoadScene(scene.scenePath);
    }
}
