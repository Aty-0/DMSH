using UnityEngine;
using UnityEngine.UI;

using DMSH.Misc;
using DMSH.Misc.Animated;

namespace DMSH.UI
{
    public class UIMainMenuStart : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 10.0f; //11

        protected void Start()
        {
            if (GlobalSettings.mainMenuAwakeAnimation)
            {
                foreach (Text text in FindObjectsOfType<Text>())
                {
                    StartCoroutine(BasicAnimationsPack.SmoothAwakeText(text, _speed));
                }
            }
        }
    }
}