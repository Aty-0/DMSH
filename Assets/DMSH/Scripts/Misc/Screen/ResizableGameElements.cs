using UnityEngine;

namespace DMSH.Misc.Screen
{
    public class ResizableGameElements : MonoBehaviour
    {
        public GameObject respawnPoint = null;

        public Rect cameraRect;

        [HideInInspector]
        public ScreenHandler screenHandler = null;

        [SerializeField]
        private GameObject _background = null;

        [HideInInspector]
        public Camera gameCamera = null;

        [SerializeField]
        private GameObject[] _wallsList = new GameObject[4];      

        [SerializeField]
        private float screenDistanceWidth; 

        [SerializeField]
        private float screenDistanceHeight;

        protected void Update()
        {
            UpdateBackgroundPosAndScale();
            CheckBounds();
        }
        
        public void Initialize()
        {
            screenHandler = gameObject.AddComponent<ScreenHandler>();
            screenHandler.onScreenResolutionChange.Add(OnResolutionScreenChange);
            GenerateInvisibleWalls();
            if (!CheckComponentsOnExist())
                return;

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

        private void OnResolutionScreenChange()
        {
            if (!enabled)
                return;
            
            // Try to translate rectTransform to world coords  
            var bottomLeft = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelRect.x, gameCamera.pixelRect.y, 0));
            var topRight = gameCamera.ScreenToWorldPoint(new Vector3(gameCamera.pixelRect.x + gameCamera.pixelRect.width, gameCamera.pixelRect.y + gameCamera.pixelRect.height));

            cameraRect = new Rect(bottomLeft.x, bottomLeft.y,
                topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

            
            // Set new position 
            UpdateInvisibleWallsPosition();
            // Restretch for new resolution
            RestretchInvisibleWalls();

            //respawnPoint.transform.position = new Vector2(cameraRect.xMin / 8, cameraRect.yMin / 1.5f);

            screenDistanceWidth = Vector3.Distance(new Vector3(cameraRect.xMin, 0, 0), new Vector3(cameraRect.xMax, 0, 0)) * 0.1f;
            screenDistanceHeight = Vector3.Distance(new Vector3(0, cameraRect.yMin, 0), new Vector3(0, cameraRect.yMax, 0)) * 0.1f;
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
            
            if ((playerPosition.x > cameraRect.xMax || playerPosition.x < cameraRect.xMin) ||
                (playerPosition.y > cameraRect.yMax || playerPosition.y < cameraRect.yMin))
            {
                var point = new Vector3(cameraRect.xMax / 2, cameraRect.yMax / 2, 0);
                var speed = Time.deltaTime * 2 * Vector3.Distance(playerPosition, point);
                gameObject.transform.position = Vector3.Lerp(playerPosition, point, speed);
            }
        }

        private void UpdateInvisibleWallsPosition()
        {
            _wallsList[0].transform.position = new Vector3(0, cameraRect.yMax, 0);
            _wallsList[1].transform.position = new Vector3(0, cameraRect.yMin, 0);
            _wallsList[2].transform.position = new Vector3(cameraRect.xMax, 0, 0);
            _wallsList[3].transform.position = new Vector3(cameraRect.xMin, 0, 0);
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