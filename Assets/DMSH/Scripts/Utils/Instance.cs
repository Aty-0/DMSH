using UnityEngine;

namespace Scripts.Utils
{
    [DefaultExecutionOrder(-5)]
    public class Instance<T> : MonoBehaviour where T : Component
    {
        public static T Get { get; private set; }

        protected void OnEnable()
        {
            if (Get == null)
            {
                Get = this as T;
                OnInstanceStateChanged(true);
            }
        }

        protected void OnDisable()
        {
            if (Get == this)
            {
                Get = null;
                OnInstanceStateChanged(false);
            }
        }

        protected virtual void OnInstanceStateChanged(bool isInstanced)
        {
            
        }
    }
}