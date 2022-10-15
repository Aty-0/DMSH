using UnityEngine;
using UnityEngine.SceneManagement;
using DMSH.Misc;

namespace DMSH.UI.Events
{
    public class ButtonRetry : MonoBehaviour
    {
        public void ButtonRetryEvent()
        {
            Time.timeScale = 1.0f;
            GlobalSettings.SetGameActive(true);
            Cursor.visible = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}