using Scripts.Utils;

using UnityEngine;

namespace DMSH.Scripts.Controls
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : Instance<GameCamera>
    {
        public Camera Camera { get; private set; }

        protected override void OnInstanceStateChanged(bool isInstanced)
        {
            if (Camera == null && isInstanced)
            {
                Camera = GetComponent<Camera>();
            }
        }
    }
}