using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonNewGame : MonoBehaviour
{
    public ScenePicker scene;

    public void ButtonNewGameEvent()
    {
        SceneManager.LoadScene(scene.scenePath);
    }
}


