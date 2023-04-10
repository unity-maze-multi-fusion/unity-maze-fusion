using System.Collections;
using System.Collections.Generic;
using FusionScripts;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    void Start()
    {
        
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (gameObject.name == "Key(Clone)")
    //     {
    //         GameManager.keyCount += 1;
    //         Destroy(gameObject);
    //     }
    // }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.name.Contains("Door") && GameManager.doorCount > NetworkGame.keys.Count)
        {
            GameManager.doorCount -= 1;
            gameObject.SetActive(false);
        }
        if (gameObject.name == "Boat")
        {
            GameManager.isGameActive = false;
            GameManager.isGameover = true;

        }
    }
}
