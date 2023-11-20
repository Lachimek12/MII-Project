using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; //moving speed of the player
    [Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 6.0f;
    private Rigidbody2D rigidBody;
    public LayerMask groundLayer;
    const float rayLength = 1.5f;
    private bool canDoubleJump = true;
    private Animator animator;
    private bool isWalking = false;
    private bool isFacingRight = true;
    private int moveInputX = 0;
    private bool jumpInput = false;
    private int lives = 3;
    private Vector2 startPosition;
    private int keysFound = 0;
    private int keysNumber = 3;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                jumpInput = true;
            }
        }
        else
        {
            moveInputX = 0;
            jumpInput = false;
        }
    }
    void FixedUpdate()
    {
        isWalking = false;

        Vector2 moveVectorX = new Vector2(1.0f, 0.0f);
        rigidBody.velocity = rigidBody.velocity * new Vector2(0.0f, 1.0f) + moveVectorX * moveSpeed * moveInputX;
        if (moveInputX != 0)
        {
            isWalking = true;
        }

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

        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isWalking", isWalking);

        //Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1.0f, false);
    }

    bool isGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer.value);
    }

    void Jump()
    {
        if (isGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canDoubleJump = true;
            //Debug.Log("jumpung");
        }
        else if (canDoubleJump == true)
        {
            rigidBody.velocity = rigidBody.velocity * new Vector2(1.0f, 0.0f);
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canDoubleJump = false;
        }
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
            GameManager.instance.AddPoints(1);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("End-of-level"))
        {
            if (keysNumber == keysFound)
            {
                Debug.Log("Wygrales");
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
            GameManager.instance.AddKeys();
            keysFound++;
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Heart"))
        {
            GameManager.instance.AddLive();
            lives++;
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("FallLevel"))
        {
            Death();
        }
        else if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    private void Death()
    {
        GameManager.instance.RemoveLive();
        lives--;
        rigidBody.velocity = new Vector2(0.0f, 0.0f);
        transform.position = startPosition;
        if (lives <= 0)
        {
            Debug.Log("Umarl smiercia tera-giczna");
        }
    }
}
