using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private int lives = 3;
    private Vector2 startPosition;
    private int keysFound = 0;
    private int keysNumber = 3;
    public TMP_Text endGameText;

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
            Debug.Log("Score " + score);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("End-of-level"))
        {
            if (keysNumber == keysFound)
            {
                Debug.Log("Wygrales");
                endGameText.gameObject.SetActive(true);
                endGameText.text = "Gratulacje, zdoby³eœ " + score.ToString() + " punktów!";
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
                score += 2;
                Debug.Log("Killed an enemy");
            }
            else
            {
                Death();
            }
        }
        else if (other.CompareTag("Key"))
        {
            keysFound++;
            other.gameObject.SetActive(false);
            Debug.Log("Zebrano klucz");
        }
        else if (other.CompareTag("Heart"))
        {
            lives++;
            other.gameObject.SetActive(false);
            Debug.Log("Zebrano HAPE");
        }
        else if (other.CompareTag("FallLevel"))
        {
            Death();
        }
    }

    private void Death()
    {
        lives--;
        rigidBody.velocity = new Vector2(0.0f, 0.0f);
        transform.position = startPosition;
        if (lives <= 0)
        {
            Debug.Log("Umarl smiercia tera-giczna");
        }
    }
}
