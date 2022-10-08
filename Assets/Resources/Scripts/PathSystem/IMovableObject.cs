using UnityEngine;
using Scripts.Utils;

namespace DMSH.Path
{
    public interface IMovableObject
    {
        int currentPoint { get; set; }
        int currentCurvePoint { get; set; }
        float speed { get; set; }
        Rigidbody2D rigidBody2D { get; set; }
        BoxCollider2D boxCollider2D { get; set; }
        Vector3 moveOffset { get; set; }
        PathSystem pathSystem { get; set; }
        float reduceSpeed { get; set; }
        float augmentSpeed { get; set; }
        float finalSpeed { get; set; }
    }


    public abstract class MovableObject : MonoBehaviourWithUniqueID, IMovableObject
    {
        [Header("Movable Object")]
        [SerializeField] 
        protected int _currentPoint = 0;
        [SerializeField] 
        protected int _currentCurvePoint = 0;
        [SerializeField] 
        protected float _speed = 10.0f;
        [SerializeField] 
        protected Rigidbody2D _rigidBody2D = null;
        [SerializeField] 
        protected BoxCollider2D _boxCollider2D = null;
        [SerializeField] 
        protected Vector3 _moveOffset = Vector3.zero;
        [SerializeField] 
        protected PathSystem _pathSystem = null;
        [SerializeField] 
        [HideInInspector] 
        protected float _reduceSpeed  = 1.0f;
        [SerializeField] 
        [HideInInspector]
        protected float _augmentSpeed = 1.0f;
        [SerializeField] 
        [HideInInspector]
        protected float _finalSpeed = 0.0f;

        public int currentPoint { get { return _currentPoint; } set { _currentPoint = value; } }
        public int currentCurvePoint { get { return _currentCurvePoint; } set { _currentCurvePoint = value; } }
        public float speed { get { return _speed; } set { _speed = value; } }
        public Rigidbody2D rigidBody2D { get { return _rigidBody2D; } set { _rigidBody2D = value; } }
        public BoxCollider2D boxCollider2D { get { return _boxCollider2D; } set { _boxCollider2D = value; } }
        public Vector3 moveOffset { get { return _moveOffset; } set { _moveOffset = value; } }
        public PathSystem pathSystem { get { return _pathSystem; } set { _pathSystem = value; } }
        public float reduceSpeed { get { return _reduceSpeed; } set { _reduceSpeed = value; } }
        public float augmentSpeed { get { return _augmentSpeed; } set { _augmentSpeed = value; } }
        public float finalSpeed { get { return _finalSpeed; } set { _finalSpeed = value; } }

        public virtual void OnReachedPointEvent(EnemyScriptedBehavior enemyScriptedBehavior) { }

        public virtual void OnReachedPoint() { }

        public virtual void OnReachedFirstPoint() { }

        public virtual void OnReachedLastPoint() { }

        public virtual void Unspawn() { }
    }
}