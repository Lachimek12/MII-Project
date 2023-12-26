using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _188898
{
    public class EnemyController : MonoBehaviour
    {
        [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; //moving speed of the enemy
        [SerializeField] private bool isFacingRight = false;
        [SerializeField] private bool isMovingRight = false;
        private Animator animator;
        private float startPositionX;
        public float moveRange = 1.0f;
        private BoxCollider2D enemyCollider;

        private void Awake()
        {
            //rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<BoxCollider2D>();
            startPositionX = this.transform.position.x;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (isMovingRight)
            {
                if (this.transform.position.x < startPositionX + moveRange)
                {
                    MoveRight();
                }
                else
                {
                    isMovingRight = false;
                    MoveLeft();
                }
            }
            else
            {
                if (this.transform.position.x > startPositionX - moveRange)
                {
                    MoveLeft();
                }
                else
                {
                    isMovingRight = true;
                    MoveRight();
                }
            }
        }

        void Flip()
        {
            Vector3 theScale = transform.localScale;

            isFacingRight = !isFacingRight;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        void MoveRight()
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            if (!isFacingRight)
            {
                Flip();
            }
        }

        void MoveLeft()
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            if (isFacingRight)
            {
                Flip();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (transform.position.y < other.gameObject.transform.position.y)
                {
                    animator.SetBool("isDead", true);
                    moveSpeed = 0;
                    enemyCollider.enabled = false; // disabling to prevent from damaging player during death time
                    StartCoroutine(KillOnAnimationEnd());
                }
            }
        }

        private IEnumerator KillOnAnimationEnd()
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }
    }
}
