using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace _188898
{
    enum DIRECTION { UP, DOWN }

    public class GeneratedPlatforms : MonoBehaviour
    {
        public GameObject platformPrefab;
        private const int PLATFORMS_NUM = 4;
        private GameObject[] platforms;
        private Vector3[] positions;
        private Vector3[] dstPositions;
        private DIRECTION[] upDown;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float moveRange = 10;
        private float speedValue;
        private float paddingX = 5;

        private void Awake()
        {
            platforms = new GameObject[PLATFORMS_NUM];
            positions = new Vector3[PLATFORMS_NUM];
            dstPositions = new Vector3[PLATFORMS_NUM];
            upDown = new DIRECTION[PLATFORMS_NUM];
            speedValue = speed;
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                positions[i].x = transform.position.x + i * paddingX;
                positions[i].y = transform.position.y + System.MathF.Sin(i * 2 * System.MathF.PI / PLATFORMS_NUM) * moveRange;
                positions[i].z = 0;
                platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);

                dstPositions[i].x = positions[i].x;
                if (i % 2 == 0)
                {
                    dstPositions[i].y = transform.position.y + moveRange;
                    upDown[i] = DIRECTION.UP;
                }
                else
                {
                    dstPositions[i].y = transform.position.y - moveRange;
                    upDown[i] = DIRECTION.DOWN;
                }
                dstPositions[i].z = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < PLATFORMS_NUM; i++)
            {
                platforms[i].transform.position = Vector3.MoveTowards(platforms[i].transform.position, dstPositions[i], speed * Time.deltaTime);

                if (upDown[i] == DIRECTION.UP && platforms[i].transform.position.y >= dstPositions[i].y)
                {
                    dstPositions[i].y = transform.position.y - moveRange;
                    upDown[i] = DIRECTION.DOWN;
                }
                else if (upDown[i] == DIRECTION.DOWN && platforms[i].transform.position.y <= dstPositions[i].y)
                {
                    dstPositions[i].y = transform.position.y + moveRange;
                    upDown[i] = DIRECTION.UP;
                }
            }
        }

        public void ChangeActive()
        {
            if (speed != 0)
            {
                speed = 0;
            }
            else
            {
                speed = speedValue;
            }
        }
    }
}
