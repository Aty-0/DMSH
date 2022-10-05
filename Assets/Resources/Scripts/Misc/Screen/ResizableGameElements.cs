using UnityEngine;
using UnityEngine.UI;

using DMSH.Path;

namespace DMSH.Misc.Screen
{
    public class ResizableGameElements : MonoBehaviour
    {
        public GameObject respawnPoint = null;

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
        private float resultPoint = 0.0f;

        [SerializeField]
        private Vector3 resolutionInWorldPoint = Vector3.zero;

        public static Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }

        public void Initialize()
        {
            screenHandler = gameObject.AddComponent<ScreenHandler>();
            screenHandler.onScreenResolutionChange.Add(OnResolutionScreenChange);
            GenerateInvisibleWalls();
            CheckComponentsOnExist();
        }
        private void CheckComponentsOnExist()
        {
            if (_background == null)
                Debug.LogError("ResizableGameElements: Background is null");

            if (_uiSomeImage == null)
                Debug.LogError("ResizableGameElements: SomeImage is null");

            if (respawnPoint == null)
                Debug.LogError("ResizableGameElements: Respawn point is null");
        }

        private void OnResolutionScreenChange()
        {
            // Get actual resolution in world points
            resolutionInWorldPoint = new Vector3(gameCamera.ViewportToWorldPoint(new Vector2(1, 0)).x,
                gameCamera.ViewportToWorldPoint(new Vector2(0, 1)).y, 0);

            // Try to translate rectTransform to world coords            
            float imageRectWInWp = GetWorldRect(_uiSomeImage.rectTransform).width;
            float imageRectHInWp = GetWorldRect(_uiSomeImage.rectTransform).height;

            resultPoint = (resolutionInWorldPoint.x - (imageRectWInWp * 0.01f) * 1.3f);

            // Calculate point beetwen screen width and uiSomeImage width 
            resolutionInWorldPoint = new Vector3(resolutionInWorldPoint.x, resolutionInWorldPoint.y, resultPoint);
            // Set new position 
            UpdateInvisibleWallsPosition(resolutionInWorldPoint);
            // Restretch for new resolution
            RestretchInvisibleWalls();

            // Set screen middle position for respawn point
            respawnPoint.transform.position = new Vector2(-resolutionInWorldPoint.z / 2, -resolutionInWorldPoint.y / 1.2f);           

            // Rescale pathSystem and change position for all point
            foreach (var system in FindObjectsOfType<PathSystem>())
            {
                //system.transform.localScale = new Vector3(aspectRatioWithImage, system.transform.localScale.y, 1);
                // TODO: Positions
            }
        }

        public void Update()
        {
            if (GlobalSettings.debugDrawInvWallSI)
            {
                Debug.DrawLine(new Vector3(resultPoint, -resolutionInWorldPoint.y, 0), new Vector3(resultPoint, resolutionInWorldPoint.y, 0));
            }

            UpdateBackgroundPosAndScale();
            CheckBounds();
        }

        private void UpdateBackgroundPosAndScale()
        {
            if (_background)
            {
                _background.transform.localScale = new Vector3((gameCamera.orthographicSize / 6) + Vector3.Distance(-new Vector3(resolutionInWorldPoint.x, 0, 0), new Vector3(resultPoint, 0, 0)) * 0.1f,
                    (gameCamera.orthographicSize / 6) + Vector3.Distance(-new Vector3(0, resolutionInWorldPoint.y, 0), new Vector3(0, resolutionInWorldPoint.y, 0)) * 0.1f, 1);

                _background.transform.position = new Vector3(gameCamera.transform.position.x, gameCamera.transform.position.y, 5);
            }
        }

        private void CheckBounds()
        {
            Vector3 posInScreen = gameCamera.WorldToScreenPoint(transform.position);
            Vector3 rightWall = gameCamera.WorldToScreenPoint(_wallsList[2].transform.position);

            if ((posInScreen.x > rightWall.x || posInScreen.x < 0) ||
                (posInScreen.y > screenHandler.Height || posInScreen.y < 0))
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, respawnPoint.transform.position,
                    Time.deltaTime * 2 * Vector3.Distance(gameObject.transform.position, respawnPoint.transform.position));
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