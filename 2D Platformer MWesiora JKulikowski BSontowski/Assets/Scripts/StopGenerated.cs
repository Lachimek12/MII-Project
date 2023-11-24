using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGenerated : MonoBehaviour
{
    public GeneratedPlatforms generetadPlatforms;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            generetadPlatforms.ChangeActive();
        }
    }
}
