using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypoint = 0;
    [SerializeField] private float speed = 1.0f;
    private Rigidbody2D rigidBody;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(this.transform.position, waypoints[currentWaypoint].transform.position);

        if (distance < 0.1f)
        { 
            currentWaypoint = currentWaypoint + 1;
            if (currentWaypoint >= waypoints.Length)
            { 
                currentWaypoint = 0;
            }
        }

        this.transform.position = Vector2.MoveTowards(this.transform.position, 
                                    waypoints[currentWaypoint].transform.position, speed * Time.fixedDeltaTime);

    }
}
