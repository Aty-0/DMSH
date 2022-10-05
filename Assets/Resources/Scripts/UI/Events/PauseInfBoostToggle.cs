using UnityEngine;
using UnityEngine.UI;
using DMSH.Misc;

namespace DMSH.UI.Events
{
    public class PauseInfBoostToggle : MonoBehaviour
    {
        [SerializeField]
        private Toggle _toggle;

        private void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = GlobalSettings.cheatInfiniteBoost;
            _toggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(_toggle);
            });
        }

        public void ToggleValueChanged(Toggle change)
        {
            GlobalSettings.cheatInfiniteBoost = _toggle.isOn;
        }
    }
}