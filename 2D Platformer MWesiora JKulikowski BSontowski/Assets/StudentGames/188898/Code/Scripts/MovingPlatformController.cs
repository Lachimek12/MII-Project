using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _188898
{
    public class MovingPlatformController : MonoBehaviour
    {
        [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; //moving speed of the platform
        private float startPositionX;
        public float moveRange = 1.0f;
        private bool isMovingRight = false;

        private void Awake()
        {
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

        void MoveRight()
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        void MoveLeft()
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }
    }
}
