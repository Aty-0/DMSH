using UnityEngine;
using UnityEngine.UI;

using DMSH.Path;

using System;
using System.Collections.Generic;

namespace DMSH.Misc.Screen
{
    // TODO: Resize all game object 
    //       Why? Because on resizing path system we get is too long paths 
    //       On very high resolutions we get slow and hard gameplay,
    //       and objects will be is tiny

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
        private float resultPoint = 0.0f;

        [SerializeField]
        private float screenDistanceWidth; // It's width to someImage

        [SerializeField]
        private float screenDistanceHeight;

        [SerializeField]
        [HideInInspector]
        private List<Tuple<string,Vector3>> initialPosition = new List<Tuple<string, Vector3>>();

        [SerializeField]
        [HideInInspector]
        private List<Tuple<string,Vector3>> initialScale = new List<Tuple<string, Vector3>>();

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
                Debug.DrawLine(new Vector3(resultPoint, -resolutionInWorldPoint.y, 0), new Vector3(resultPoint, resolutionInWorldPoint.y, 0));
            }
        }

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
            if (!CheckComponentsOnExist())
                return;

            // first 
            UpdateResolutionInWorldPoint();
            OnResolutionScreenChange();

            // Save initial position of PathSystems
            foreach (var system in FindObjectsOfType<PathSystem>())
            {
                initialPosition.Add(Tuple.Create(system.ID, system.transform.position));
            }

            // Save initial scale of MovableObjects
            foreach (var mo in FindObjectsOfType<MovableObject>())
            {
                initialScale.Add(Tuple.Create(mo.ID, mo.transform.localScale));
            }
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
            float imageRectWInWp = GetWorldRect(_uiSomeImage.rectTransform).width;

            // TODO:
            //float imageRectHInWp = GetWorldRect(_uiSomeImage.rectTransform).height;

            resultPoint = (resolutionInWorldPoint.x - (imageRectWInWp * 0.01f) * 1.3f);

            // Calculate point beetwen screen width and uiSomeImage width 
            resolutionInWorldPoint = new Vector3(resolutionInWorldPoint.x, resolutionInWorldPoint.y, resultPoint);
            // Set new position 
            UpdateInvisibleWallsPosition(resolutionInWorldPoint);
            // Restretch for new resolution
            RestretchInvisibleWalls();

            // Set screen middle position for respawn point
            respawnPoint.transform.position = new Vector2(-resolutionInWorldPoint.z / 2, -resolutionInWorldPoint.y / 1.2f);
            screenDistanceWidth = Vector3.Distance(-new Vector3(resolutionInWorldPoint.x, 0, 0), new Vector3(resultPoint, 0, 0)) * 0.1f;
            screenDistanceHeight = Vector3.Distance(-new Vector3(0, resolutionInWorldPoint.y, 0), new Vector3(0, resolutionInWorldPoint.y, 0)) * 0.1f;

            // Rescale pathSystem and change position for all points
            foreach (var system in FindObjectsOfType<PathSystem>())
            {
                Debug.Log($"ResizableGameElements: Rescale [PathSystem] object {system.name} id {system.ID}");
                foreach (var pos in initialPosition)
                {
                    if (pos.Item1 == system.ID)
                    {
                        Debug.Log("ResizableGameElements: Passed");
                        system.transform.localPosition = new Vector3(pos.Item2.x - screenDistanceWidth, pos.Item2.y + screenDistanceHeight, 1);
                        break;
                    }
                }

                system.transform.localScale = new Vector3(screenDistanceWidth, screenDistanceHeight, 1);
            }

            // Rescale MovableObjects
            foreach (var mo in FindObjectsOfType<MovableObject>())
            {
                Debug.Log($"ResizableGameElements: Rescale [MovableObject] object {mo.name} id {mo.ID}");
                foreach (var scale in initialScale)
                {
                    if (scale.Item1 == mo.ID)
                    {
                        Debug.Log("ResizableGameElements: Passed");
                        mo.transform.localScale = new Vector3(scale.Item2.x + screenDistanceWidth, scale.Item2.y + screenDistanceHeight, 1);
                        break;
                    }
                }
            }
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