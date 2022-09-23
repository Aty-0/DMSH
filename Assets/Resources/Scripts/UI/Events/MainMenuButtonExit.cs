using UnityEngine;

namespace DMSH.UI.Events
{
    public class MainMenuButtonExit : MonoBehaviour
    {
        public void ButtonExitEvent()
        {
            Application.Quit();
        }
    }
}