using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _188898
{
    public class PlayerController : MonoBehaviour
    {

        public PlayerRunData Data;

        #region COMPONENTS
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip keySound;
        [SerializeField] private AudioClip heartSound;
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip endOfLevelSound;
        [SerializeField] private AudioClip killSound;
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip jumpSound;
        private AudioSource source;
        private Rigidbody2D rigidBody;
        private Animator animator;
        #endregion

        #region STATE PARAMETERS
        private bool isWalking = false;
        private bool isFacingRight = true;
        private bool isFalling = false;
        private bool isBounce = false;
        public float LastOnGroundTime;
        #endregion


        #region MOVEMENT PARAMETERS
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 0.1f; //moving speed of the player
        [SerializeField] private float acceleration = 0.5f;
        [SerializeField] private float decceleration = 0.5f;
        [SerializeField] private float velPower = 0.5f;
        [SerializeField] private float frictionAmount = 0.5f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 6.0f;
        [SerializeField] private float cayoteTime = 0.1f;

        private float lastJumpTime;
        private bool jumpInputReleased;
        private bool isJumping = false;

        private bool jumpInput = false;
        private bool canDoubleJump = true;
        const float rayLength = 1.5f;
        private int moveInputX = 0;

        [Header("Gravity")]
        [SerializeField] private float gravityScale = 6.0f;
        [SerializeField] private float fallGravityMultiplier = 6.0f;
        #endregion


        #region CHECK PARAMETERS
        [Header("Checks")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.7783947f, 0.03f);
        #endregion

        #region LAYERS & TAGS
        [Header("layers & tags")]
        [SerializeField] private LayerMask groundLayer;
        #endregion

        #region GAME STATE
        private Vector2 startPosition;
        private int lives = 3;
        private int keysFound = 0;
        private int keysNumber = 3;
        #endregion



        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            source = GetComponent<AudioSource>();
            startPosition = transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

            LastOnGroundTime -= Time.deltaTime;
            lastJumpTime -= Time.deltaTime;

            if (GameManager.instance.currentGameState == GameState.GS_GAME)
            {
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    moveInputX = 1;
                }
                else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    moveInputX = -1;
                }
                else
                {
                    moveInputX = 0;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpInput = true;
                }
            }
            else
            {
                moveInputX = 0;
                jumpInput = false;
            }

            if (isGrounded())
            {
                isJumping = false;
                LastOnGroundTime = cayoteTime;
            }

            if (rigidBody.velocity.y < 0)
            {
                rigidBody.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else
            {
                rigidBody.gravityScale = gravityScale;
            }

            if (isJumping && Mathf.Abs(rigidBody.velocity.y) >= 0.4f)
            {
                rigidBody.gravityScale = gravityScale * 0.75f;
            }


        }
        void FixedUpdate()
        {
            if (isBounce == false)
            {
                Run();
            }
            if (isGrounded())
            {
                isBounce = false;
            }


            //Vector2 moveVectorX = new Vector2(1.0f, 0.0f);
            // rigidBody.velocity = rigidBody.velocity * new Vector2(0.0f, 1.0f) + moveVectorX * moveSpeed * moveInputX;
            isWalking = Mathf.Abs(rigidBody.velocity.x) >= 0.01;


            if (!isFacingRight && moveInputX > 0)
            {
                Flip();
            }

            if (isFacingRight && moveInputX < 0)
            {
                Flip();
            }

            if (jumpInput)
            {
                Jump();
                jumpInput = false;
            }

            isFalling = rigidBody.velocity.y < -0.1f;


            animator.SetBool("isFalling", isFalling);
            animator.SetBool("isGrounded", isGrounded());
            animator.SetBool("isWalking", isWalking);

            //Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1.0f, false);

        }

        void Run()
        {
            float targetSpeed = moveInputX * moveSpeed;

            float speedDif = targetSpeed - rigidBody.velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            rigidBody.AddForce(movement * Vector2.right);


            #region Friction
            if (LastOnGroundTime > 0 && moveInputX != 0)
            {
                float amount = Mathf.Min(Mathf.Abs(rigidBody.velocity.x), Mathf.Abs(frictionAmount));

                amount *= Mathf.Sign(rigidBody.velocity.x);

                rigidBody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }
            #endregion

            /*
            float targetSpeed = moveInputX * runMaxSpeed;

            float accelRate;

            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            if (LastOnGroundTime > 0)
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
            }
            else
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
            }


            if (doConserveMomentum && Mathf.Abs(rigidBody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rigidBody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }

            float speedDif = targetSpeed - rigidBody.velocity.x;

            float movement = speedDif * accelRate;

            rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);

            */

        }


        bool isGrounded()
        {
            return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
        }

        void Jump()
        {
            // Debug.Log(LastOnGroundTime);
            if ((isGrounded() && !isJumping) || (LastOnGroundTime > 0 && !isJumping))
            {
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                LastOnGroundTime = 0;
                lastJumpTime = 0;
                isJumping = true;
                jumpInputReleased = true;
                canDoubleJump = true;

            }
            else if (canDoubleJump)
            {
                float velocityY = rigidBody.velocity.y;
                rigidBody.velocity = Vector2.right * rigidBody.velocity;

                if (velocityY > 0.1)
                {
                    Debug.Log(100);
                    rigidBody.velocity = rigidBody.velocity + new Vector2(0, 0.1f);
                }
                Debug.Log(rigidBody.velocity.y);
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false;
            }
            /*if (isGrounded())
            {
                source.PlayOneShot(jumpSound, AudioListener.volume);
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = true;
                //Debug.Log("jumpung");
            }
            else if (canDoubleJump == true)
            {
                source.PlayOneShot(jumpSound, AudioListener.volume);
                rigidBody.velocity = rigidBody.velocity * new Vector2(1.0f, 0.0f);
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false;
            }*/
        }

        void Flip()
        {
            Vector3 theScale = transform.localScale;

            isFacingRight = !isFacingRight;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bonus"))
            {
                source.PlayOneShot(coinSound, AudioListener.volume);
                GameManager.instance.AddPoints(1);
                other.gameObject.SetActive(false);
            }
            else if (other.CompareTag("End-of-level"))
            {
                if (keysNumber == keysFound)
                {
                    source.PlayOneShot(endOfLevelSound, AudioListener.volume);
                    GameManager.instance.AddPoints(100 * lives);
                    GameManager.instance.LevelCompleted();
                }
                else
                {
                    Debug.Log("za malo kluczy");
                }
            }
            else if (other.CompareTag("Enemy"))
            {
                if (transform.position.y > other.gameObject.transform.position.y)
                {
                    source.PlayOneShot(killSound, AudioListener.volume);
                    GameManager.instance.AddPoints(2);
                    GameManager.instance.AddKill();
                }
                else
                {
                    Death();
                }
            }
            else if (other.CompareTag("Key"))
            {
                source.PlayOneShot(keySound, AudioListener.volume);
                GameManager.instance.AddKeys(other.GetComponent<SpriteRenderer>().color);
                keysFound++;
                other.gameObject.SetActive(false);
            }
            else if (other.CompareTag("Heart"))
            {
                source.PlayOneShot(heartSound, AudioListener.volume);
                GameManager.instance.AddLive();
                lives++;
                other.gameObject.SetActive(false);
            }
            else if (other.CompareTag("BouncePad"))
            {
                Debug.Log(2.0f * jumpForce * Mathf.Cos(other.gameObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
                rigidBody.velocity = new Vector2(2.0f * jumpForce * Mathf.Sin(-other.gameObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 2.0f * jumpForce * Mathf.Cos(-other.gameObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
                if (other.gameObject.transform.rotation.eulerAngles.z != 0) isBounce = true;
            }
            else if (other.CompareTag("FallLevel"))
            {
                Death();
            }
            else if (other.CompareTag("MovingPlatform"))
            {
                rigidBody.interpolation = RigidbodyInterpolation2D.None;
                transform.SetParent(other.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("MovingPlatform"))
            {
                rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
                transform.SetParent(null);
            }
        }

        private void Death()
        {
            source.PlayOneShot(hurtSound, AudioListener.volume);
            GameManager.instance.RemoveLive();
            lives--;
            rigidBody.velocity = new Vector2(0.0f, 0.0f);
            if (transform.position.x <= 88) {
                transform.position = startPosition;
            }
            else
            {
                transform.position = new Vector3(88.0f, 7.0f, 0.0f);
            }
            if (lives <= 0)
            {
                source.PlayOneShot(gameOverSound, AudioListener.volume);
                GameManager.instance.GameOver();
            }
        }
    }
}
