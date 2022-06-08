using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMovableObject 
{
    int                 currentPoint { get; set; }
    int                 currentCurvePoint { get; set; }
    float               speed           { get; set; }
    Rigidbody2D         rigidBody2D { get; set; }
    BoxCollider2D       boxCollider2D   { get; set; }
    Vector3             moveOffset      { get; set; }
    PathSystem          pathSystem { get; set; }
}


public abstract class MovableObject : MonoBehaviour, IMovableObject
{
    [Header("Movable Object")]
    [SerializeField] protected int _currentPoint = 0;
    [SerializeField] protected int _currentCurvePoint = 0;
    [SerializeField] protected float _speed = 10.0f;
    [SerializeField] protected Rigidbody2D _rigidBody2D = null;
    [SerializeField] protected BoxCollider2D _boxCollider2D = null;
    [SerializeField] protected Vector3 _moveOffset = Vector3.zero;
    [SerializeField] protected PathSystem _pathSystem = null;

    public int currentPoint { get { return _currentPoint; } set { _currentPoint = value; } }
    public int currentCurvePoint { get { return _currentCurvePoint; } set { _currentCurvePoint = value; } }
    public float speed { get { return _speed; } set { _speed = value; } }
    public Rigidbody2D rigidBody2D { get { return _rigidBody2D; } set { _rigidBody2D = value; } }
    public BoxCollider2D boxCollider2D { get { return _boxCollider2D; } set { _boxCollider2D = value; } }
    public Vector3 moveOffset { get { return _moveOffset; } set { _moveOffset = value; } }
    public PathSystem pathSystem { get { return _pathSystem; } set { _pathSystem = value; } }

    public virtual void OnReachedPointEvent(EnemyScriptedBehavior enemyScriptedBehavior) { }    

    public virtual void OnReachedPoint() { }    

    public virtual void OnReachedFirstPoint() { }    

    public virtual void OnReachedLastPoint() { }    
}