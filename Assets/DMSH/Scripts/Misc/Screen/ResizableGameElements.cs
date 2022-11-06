using UnityEngine;
using UnityEngine.UI;

namespace DMSH.Misc.Screen
{
    public class ResizableGameElements : MonoBehaviour
    {
        public GameObject respawnPoint = null;

        // X - Width, Y - Height, Z - Width to right image
        public Vector3    resolutionInWorldPoint = Vector3.zero;

        [HideInInspector]
        public ScreenHandler screenHandler = null;

        [SerializeField]
        private GameObject _background = null;

        [SerializeField]
        private Image _uiSomeImage = null; // Image on the right screen corner

        [HideInInspector]
        public Camera gameCamera = null;

        [SerializeField]
        private GameObject[] _wallsList = new GameObject[4];      

        [SerializeField]
        private float screenDistanceWidth; // It's width to someImage

        [SerializeField]
        private float screenDistanceHeight;

        protected void Update()
        {
            OnDrawDebug();

            UpdateBackgroundPosAndScale();
            CheckBounds();
        }

        private void OnDrawDebug()
        {
            if (GlobalSettings.debugDrawInvWallSI)
            {
                Debug.DrawLine(new Vector3(resolutionInWorldPoint.z, -resolutionInWorldPoint.y, 0), new Vector3(resolutionInWorldPoint.z, resolutionInWorldPoint.y, 0));
            }
        }

        public void Initialize()
        {
            screenHandler = gameObject.AddComponent<ScreenHandler>();
            screenHandler.onScreenResolutionChange.Add(OnResolutionScreenChange);
            GenerateInvisibleWalls();
            if (!CheckComponentsOnExist())
                return;

            // first 
            UpdateResolutionInWorldPoint();
            // Do last
            OnResolutionScreenChange();
        }

        private bool CheckComponentsOnExist()
        {
            var isFine = true;

            if (_background == null)
            {
                Debug.LogWarning("ResizableGameElements: Background is null", this);
                isFine = false;
            }

            if (_uiSomeImage == null)
            {
                Debug.LogWarning("ResizableGameElements: SomeImage is null", this);
                isFine = false;
            }

            if (respawnPoint == null)
            {
                Debug.LogWarning("ResizableGameElements: Respawn point is null", this);
                isFine = false;
            }

            enabled = isFine;
            if (!isFine)
            {
                Debug.LogWarning("Disable component! It's not ready to be used.", this);
            }
            return isFine;
        }

        private void UpdateResolutionInWorldPoint()
        {
            resolutionInWorldPoint = new Vector3(gameCamera.ViewportToWorldPoint(new Vector2(1, 0)).x,
                    gameCamera.ViewportToWorldPoint(new Vector2(0, 1)).y, 0);
        }

        private void OnResolutionScreenChange()
        {
            if (!enabled)
                return;
            
            // Get actual resolution in world points
            UpdateResolutionInWorldPoint();

            // Try to translate rectTransform to world coords            
            var offset = _uiSomeImage.rectTransform.rect.width;
            var bottomLeft = gameCamera.ScreenToWorldPoint(Vector3.zero);
            var topRight = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelWidth - offset, gameCamera.pixelHeight));
            var cameraRect = new Rect(bottomLeft.x, bottomLeft.y,
                topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

            resolutionInWorldPoint = new Vector3(resolutionInWorldPoint.x, resolutionInWorldPoint.y, cameraRect.xMax);
            // Set new position 
            UpdateInvisibleWallsPosition(resolutionInWorldPoint);
            // Restretch for new resolution
            RestretchInvisibleWalls();

            // Set screen middle position for respawn point
            respawnPoint.transform.position = new Vector2(-resolutionInWorldPoint.z / 2, -resolutionInWorldPoint.y / 1.2f);
            screenDistanceWidth = Vector3.Distance(-new Vector3(resolutionInWorldPoint.x, 0, 0), new Vector3(cameraRect.xMax, 0, 0)) * 0.1f;
            screenDistanceHeight = Vector3.Distance(-new Vector3(0, resolutionInWorldPoint.y, 0), new Vector3(0, resolutionInWorldPoint.y, 0)) * 0.1f;
        }

        private void UpdateBackgroundPosAndScale()
        {
            if (_background)
            {
                _background.transform.localScale = new Vector3((gameCamera.orthographicSize / 6) + screenDistanceWidth, (gameCamera.orthographicSize / 6) + screenDistanceHeight, 1);
                _background.transform.position = new Vector3(gameCamera.transform.position.x, gameCamera.transform.position.y, 5);
            }
        }

        private void CheckBounds()
        {
            // Check player is still in screen coords
            var playerPosition = transform.position;
            
            if ((playerPosition.x > resolutionInWorldPoint.z || playerPosition.x < -resolutionInWorldPoint.x) ||
                (playerPosition.y > resolutionInWorldPoint.y || playerPosition.y < -resolutionInWorldPoint.y))
            {
                var point = new Vector3(resolutionInWorldPoint.z / 2, resolutionInWorldPoint.y / 2, 0);
                var speed = Time.deltaTime * 2 * Vector3.Distance(playerPosition, point);
                gameObject.transform.position = Vector3.Lerp(playerPosition, point, speed);
            }
        }

        private void UpdateInvisibleWallsPosition(Vector3 resolutionInWorldPoint)
        {
            _wallsList[0].transform.position = new Vector3(0, resolutionInWorldPoint.y, 0);
            _wallsList[1].transform.position = new Vector3(0, -resolutionInWorldPoint.y, 0);
            _wallsList[2].transform.position = new Vector3(resolutionInWorldPoint.z, 0, 0);
            _wallsList[3].transform.position = new Vector3(-resolutionInWorldPoint.x, 0, 0);
        }

        private void RestretchInvisibleWalls()
        {
            for (int i = 0; i <= 3; i++)
            {
                var local_boxCollider2D = _wallsList[i].GetComponent<BoxCollider2D>();
                local_boxCollider2D.size = gameCamera.ViewportToWorldPoint(i <= 1 ? new Vector2(1, 0) : new Vector2(0, 1)) * 2;
                local_boxCollider2D.size += i <= 1 ? new Vector2(0.0f, 0.1f) : new Vector2(0.1f, 0.0f);
            }
        }

        private void GenerateInvisibleWalls()
        {
            for (int i = 0; i <= 3; i++)
            {
                _wallsList[i] = new GameObject($"InvisibleWall_{i}");
                var local_boxCollider2D = _wallsList[i].AddComponent<BoxCollider2D>();
                local_boxCollider2D.size = gameCamera.ViewportToWorldPoint(i <= 1 ? new Vector2(1, 0) : new Vector2(0, 1)) * 2;
                local_boxCollider2D.size += i <= 1 ? new Vector2(0.0f, 0.1f) : new Vector2(0.1f, 0.0f);
                _wallsList[i].layer = LayerMask.NameToLayer("InvisibleWall");
            }
        }
    }
}