using UnityEngine;
using UnityEngine.SceneManagement;
using DMSH.Misc;

namespace DMSH.UI.Events
{
    public class MainMenuButtonNewGame : MonoBehaviour
    {
        public ScenePicker scene;

        public void ButtonNewGameEvent()
        {
            SceneManager.LoadScene(scene.scenePath);
        }
    }
}