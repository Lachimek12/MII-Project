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
    private int score = 0;
    public TMP_Text endGameText;

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

        if (gameObject.transform.position.y < -15) //check if player fell off the map
        {
            gameObject.transform.position = new Vector3(-22.73f, -1.98f, 0);
        }

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
            Debug.Log("Score " + score);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("End-of-level"))
        {
            endGameText.gameObject.SetActive(true);
            endGameText.text = "Gratulacje, zdoby³eœ " + score.ToString() + " punktów!";
        }
    }
}
