using UnityEngine;

namespace DMSH.Misc
{
    public class CubeRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 _currentEulerAngles;
        [SerializeField] private float _speed = 10.0f;

        protected void Update()
        {
            _currentEulerAngles += new Vector3(Time.deltaTime * _speed, Time.deltaTime * _speed, Time.deltaTime * _speed);
            gameObject.transform.eulerAngles = _currentEulerAngles;
        }
    }
}