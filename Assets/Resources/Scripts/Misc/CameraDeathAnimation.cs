using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeathAnimation : MonoBehaviour
{
    public GameObject   target;
    public float        size    = 2.0f;
    public float        speed   = 3.0f;
    public Camera       animCamera;
    [SerializeField] private Coroutine  _coroutineZoom;
    [SerializeField] private Coroutine  _coroutineMoveToTarget;
    [SerializeField] private Coroutine  _coroutineCameraShake;
    private IEnumerator CameraShake()
    {
        while (true)
        {
            animCamera.transform.rotation = Quaternion.Euler(Vector3.Lerp(animCamera.transform.rotation.eulerAngles, animCamera.transform.rotation.eulerAngles +
                new Vector3(0, 0, Mathf.Sin(Random.insideUnitCircle.x) + 10), 
                (speed / 2) * Time.deltaTime));

            yield return new WaitForSeconds(0.01f);
        }
    }

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
        //_coroutineCameraShake = StartCoroutine(CameraShake());
    }

    //protected void OnGUI()
    //{
    //    if (GUI.Button(new Rect(20,200, 100, 100), "Play"))
    //    {
    //        if (_coroutineZoom != null)
    //            StopCoroutine(_coroutineZoom);
    //
    //        if (_coroutineCameraShake != null)
    //            StopCoroutine(_coroutineCameraShake);
    //
    //        if (_coroutineMoveToTarget != null)
    //            StopCoroutine(_coroutineMoveToTarget);
    //
    //        _camera.orthographicSize = 5.0f;
    //        _camera.transform.position = new Vector3(0, 0, _camera.transform.position.z);
    //        Play();
    //    }
    //}
}
