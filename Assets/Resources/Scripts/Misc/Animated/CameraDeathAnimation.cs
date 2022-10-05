using System.Collections;
using UnityEngine;

namespace DMSH.Misc.Animated
{
    public class CameraDeathAnimation : MonoBehaviour
    {
        public GameObject target = null;
        public Camera animCamera = null;
        [SerializeField]
        private float _size = 2.0f;
        [SerializeField]
        private float _speed = 3.0f;

        private Coroutine _coroutineZoom;
        private Coroutine _coroutineMoveToTarget;

        private IEnumerator Zoom()
        {
            while (animCamera.orthographicSize >= _size)
            {
                animCamera.orthographicSize -= _speed * Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator MoveToTarget()
        {
            var targetPos = new Vector2(target.transform.position.x, target.transform.position.y);
            var cameraPos = new Vector2(animCamera.transform.position.x, animCamera.transform.position.y);
            while (cameraPos != targetPos)
            {
                cameraPos = Vector2.Lerp(cameraPos, targetPos + Random.insideUnitCircle, _speed * Time.deltaTime);
                animCamera.transform.position = new Vector3(cameraPos.x, cameraPos.y, animCamera.transform.position.z);

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void Play()
        {
            _coroutineZoom = StartCoroutine(Zoom());
            _coroutineMoveToTarget = StartCoroutine(MoveToTarget());
        }
    }
}