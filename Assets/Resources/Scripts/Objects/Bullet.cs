using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
//Rework freeMovement 
//Need use impulse for pushing bullet 
//We need to choise impulse direction 
//Need to create lifeTime logic 

public class Bullet : MovableObject
{
    [Header("Bullet")]
    public bool       isEnemyBullet = false;
    public bool       collisionDestoryBullet = true;
    public float      lifeTime = 2.0f;
    public bool       freeMovement = false;
    public Timer      timer;
    [SerializeField] private Vector2    _bullet_dir = new Vector2(0, 1);
    [SerializeField] private float      _rotation_speed = 300.0f;
    
    protected void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void Kill()
    {
        Destroy(gameObject);
    }

    protected void Awake()
    {
        if (freeMovement == true)
        {
            //Destroy(this.gameObject, lifeTime);

            timer = gameObject.AddComponent(typeof(Timer)) as Timer;
            timer.EndEvent += Kill;
            timer.time = lifeTime;
            timer.StartTimer();
        }
    }

    protected void Update()
    {
        //Basic effect of rotation
        rigidBody2D.MoveRotation(rigidBody2D.rotation + (_rotation_speed * Time.fixedDeltaTime * GlobalSettings.gameActive));

        if (freeMovement == true)
            rigidBody2D.MovePosition(rigidBody2D.position + ((_bullet_dir * speed) * Time.fixedDeltaTime * GlobalSettings.gameActive));
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionDestoryBullet)
            Kill();
    }

}
