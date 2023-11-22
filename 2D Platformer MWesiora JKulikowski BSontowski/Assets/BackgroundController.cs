using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public Transform target;
    private Rigidbody2D rigidBody;
    private Vector2 startingPosition;
    private Vector2 playerStartingPosition;


    // Use this for initialization
    private void Start()
    {
        startingPosition = transform.position;
        playerStartingPosition = target.position;
        rigidBody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 position = new Vector2(target.position.x - playerStartingPosition.x, target.position.y - playerStartingPosition.y);
        transform.position = startingPosition + position * new Vector2(0.2f, 0.1f);
    }
}
