using UnityEngine;

namespace DMSH.UI.Events
{
    public class MainMenuOpenTabEvent : MonoBehaviour
    {
        public GameObject tab;

        public void OpenTab()
        {
            tab.gameObject.SetActive(true);
            transform.parent.gameObject.SetActive(false);
        }
    }
}