using System;
using UnityEngine;

namespace Scripts.Utils
{
    public class MonoBehaviourWithUniqueID : MonoBehaviour
    {
        [Header("UniqueID")]
        [SerializeField] 
        private string _ID = Guid.NewGuid().ToString();
        public string ID => _ID;

        [ContextMenu("Generate new ID")]
        private void RegenerateGUID() => _ID = Guid.NewGuid().ToString();
    }
}