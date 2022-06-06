using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Vector3 _currentEulerAngles;
    [SerializeField] private float _speed = 10.0f;

    protected void Update()
    {
        _currentEulerAngles += new Vector3(Time.deltaTime * _speed, Time.deltaTime * _speed, Time.deltaTime * _speed);
        _gameObject.transform.eulerAngles = _currentEulerAngles;
    }
}
