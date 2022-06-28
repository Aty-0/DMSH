using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeathAnimation : MonoBehaviour
{
    public GameObject   target = null;
    public Camera       animCamera = null;
    public float        size    = 2.0f;
    public float        speed   = 3.0f;

    [SerializeField] private Coroutine  _coroutineZoom;
    [SerializeField] private Coroutine  _coroutineMoveToTarget;
    
    private IEnumerator Zoom()
    {
        while (animCamera.orthographicSize >= size)
        {
            animCamera.orthographicSize -= speed * Time.deltaTime;            
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator MoveToTarget()
    {
        Vector2 target_pos = new Vector2(target.transform.position.x, target.transform.position.y);
        Vector2 camera_pos = new Vector2(animCamera.transform.position.x, animCamera.transform.position.y);
        while (camera_pos != target_pos)
        {
            camera_pos = Vector2.Lerp(camera_pos, target_pos + Random.insideUnitCircle, speed * Time.deltaTime);
            animCamera.transform.position = new Vector3(camera_pos.x, camera_pos.y, animCamera.transform.position.z);

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Play()
    {
        _coroutineZoom = StartCoroutine(Zoom());
        _coroutineMoveToTarget = StartCoroutine(MoveToTarget());
    }
}
