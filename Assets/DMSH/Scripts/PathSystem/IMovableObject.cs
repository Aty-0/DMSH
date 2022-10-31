using UnityEngine;

using Scripts.Utils;

namespace DMSH.Path
{
    public interface IMovableObject
    {
        int CurrentPoint { get; set; }
        int CurrentCurvePoint { get; set; }
        float speed { get; set; }
        Rigidbody2D RigidBody2D { get; set; }
        Collider2D Collider2D { get; set; }
        Vector3 MoveOffset { get; set; }
        PathSystem PathSystem { get; set; }
        float ReduceSpeed { get; set; }
        float AugmentSpeed { get; set; }
        float FinalSpeed { get; set; }
    }


    public abstract class MovableObject : MonoBehaviourWithUniqueID, IMovableObject
    {
        public virtual Vector2 MoveDirection { get; set; }

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
        protected Collider2D _Collider2D = null;
        [SerializeField]
        protected Vector3 _moveOffset = Vector3.zero;
        [SerializeField]
        protected PathSystem _pathSystem = null;
        [SerializeField, HideInInspector]
        protected float _reduceSpeed = 1.0f;
        [SerializeField, HideInInspector]
        protected float _augmentSpeed = 1.0f;
        [SerializeField, HideInInspector]
        protected float _finalSpeed = 0.0f;

        public int CurrentPoint
        {
            get => _currentPoint;
            set => _currentPoint = value;
        }

        public int CurrentCurvePoint
        {
            get => _currentCurvePoint;
            set => _currentCurvePoint = value;
        }

        public float speed
        {
            get => _speed;
            set => _speed = value;
        }

        public Rigidbody2D RigidBody2D
        {
            get => _rigidBody2D;
            set => _rigidBody2D = value;
        }

        public Collider2D Collider2D
        {
            get => _Collider2D;
            set => _Collider2D = value;
        }

        public Vector3 MoveOffset
        {
            get => _moveOffset;
            set => _moveOffset = value;
        }

        public PathSystem PathSystem
        {
            get => _pathSystem;
            set => _pathSystem = value;
        }

        public float ReduceSpeed
        {
            get => _reduceSpeed;
            set => _reduceSpeed = value;
        }

        public float AugmentSpeed
        {
            get => _augmentSpeed;
            set => _augmentSpeed = value;
        }

        public float FinalSpeed
        {
            get => _finalSpeed;
            set => _finalSpeed = value;
        }

        public virtual void OnReachedPointEvent(EnemyScriptedBehavior enemyScriptedBehavior)
        {
        }

        public virtual void OnReachedPoint()
        {
        }

        public virtual void OnReachedFirstPoint()
        {
        }

        public virtual void OnReachedLastPoint()
        {
        }

        public virtual void UnSpawn()
        {
            Debug.LogWarning($"({GetType().Name}) If it's can move, please pool it!", this);
        }
    }
}