using Scripts.Utils;

using UnityEngine;

namespace DMSH.Scripts.Controls
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : Instance<GameCamera>
    {
        public Camera Camera { get; private set; }

        [SerializeField]
        private Vector2Int m_worldSize = new(24, 28);

        private Rect _prevSafeRect;

        protected override void OnInstanceStateChanged(bool isInstanced)
        {
            if (Camera == null && isInstanced)
            {
                Camera = GetComponent<Camera>();
            }
        }

        protected void Update()
        {
            if (Camera == null || !Camera.enabled)
                return;

            const float FIXED_SIDEBAR_WIDTH = 240;
            const float SIDE_PADDINGS = 6;

            var safeRect = Screen.safeArea;
            if (_prevSafeRect == safeRect)
                return;

            _prevSafeRect = safeRect;

            var shouldBeAspect = (float)m_worldSize.x / m_worldSize.y;

            var maxAllowedWidth = safeRect.width - FIXED_SIDEBAR_WIDTH - SIDE_PADDINGS * 2;
            var maxAllowedHeight = safeRect.height - SIDE_PADDINGS * 2;

            float gameWidth;
            float gameHeight;

            if ((maxAllowedHeight * shouldBeAspect) <= maxAllowedWidth)
            {
                gameWidth = maxAllowedHeight * shouldBeAspect;
                gameHeight = maxAllowedHeight;
            }
            else
            {
                gameWidth = maxAllowedWidth;
                gameHeight = maxAllowedWidth / shouldBeAspect;
            }

            var xOffset = (safeRect.width - FIXED_SIDEBAR_WIDTH) / 2 - (gameWidth / 2);
            var yOffset = safeRect.height / 2 - (gameHeight / 2);

            Camera.pixelRect = new Rect(xOffset, yOffset, gameWidth, gameHeight);
        }
    }
}