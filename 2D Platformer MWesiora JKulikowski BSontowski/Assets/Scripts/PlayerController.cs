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
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
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
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
        else if(canDoubleJump == true)
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canDoubleJump = false;
        }
    }
}
