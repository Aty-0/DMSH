using System.Collections;
using UnityEngine;
using DMSH.Misc;
using DMSH.Path;

namespace DMSH.Objects
{
    public class Bullet : MovableObject
    {
        [Header("Bullet")]
        public bool isEnemyBullet = false;
        public bool collisionDestroyBullet = true;
        public float lifeTime = 2.0f;
        public bool freeMovement = false;
        public Timer timer = null;

        [SerializeField] private Vector2 _bullet_dir = new Vector2(0, 1);
        [SerializeField] private float _rotation_speed = 300.0f;
        [SerializeField] private TrailRenderer _trailRenderer = null;

        protected void Start()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();

            // If we are attach pathSystem on start 
            freeMovement = freeMovement && pathSystem == null;

            if (freeMovement == true)
            {
                timer = gameObject.AddComponent(typeof(Timer)) as Timer;
                timer.EndEvent += Unspawn;
                timer.time = lifeTime;
                timer.StartTimer();
            }
        }

        public override void Unspawn()
        {
            Destroy(gameObject);
        }

        // Squeeze bullet 
        // It's just destroy effect for boost or player death

        private IEnumerator SqueezeAnimation()
        {
            while (transform.localScale != new Vector3(0.0f, 0.0f, transform.localScale.z))
            {
                // FIX ME: Smooth...
                float speed = 4.0f * Time.deltaTime * 10 * GlobalSettings.gameActiveAsInt;

                Vector3 vec = Vector3.Lerp(transform.localScale, Vector3.zero, speed);
                float startwidth = Mathf.Lerp(_trailRenderer.startWidth, 0, speed);
                float endwidth = Mathf.Lerp(_trailRenderer.endWidth, 0, speed);
                transform.localScale = new Vector3(vec.x, vec.y, transform.localScale.z);
                _trailRenderer.endWidth = startwidth;
                _trailRenderer.startWidth = endwidth;
                yield return new WaitForSeconds(0.01f);
            }

            Unspawn();
        }

        public void SqueezeAndDestroy()
        {
            boxCollider2D.enabled = false;
            StartCoroutine(SqueezeAnimation());
        }

        protected void Update()
        {
            // Basic effect of rotation
            rigidBody2D.MoveRotation(rigidBody2D.rotation + (_rotation_speed * Time.fixedDeltaTime * GlobalSettings.gameActiveAsInt));

            if (freeMovement == true)
                rigidBody2D.MovePosition(rigidBody2D.position + ((_bullet_dir * speed) * Time.fixedDeltaTime * GlobalSettings.gameActiveAsInt));
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collisionDestroyBullet)
                Unspawn();
        }
    }
}
