using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f; //moving speed of the player
    [Range(0.01f, 20.0f)] [SerializeField] private float jumpForce = 6.0f;
    private Rigidbody2D rigidBody;
    public LayerMask groundLayer;
    const float rayLength = 1.5f;
    private bool canDoubleJump = true;
    private Animator animator;
    private bool isWalking = false;
    private bool isFacingRight = true;
    private int score = 0;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isWalking = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;
            if (!isFacingRight)
            {
                Flip();
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;
            if (isFacingRight)
            {
                Flip();
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
        else if(canDoubleJump == true)
        {
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
            score += 1;
            //Debug.Log("Score " + score);
            other.gameObject.SetActive(false);
        }
    }
}
