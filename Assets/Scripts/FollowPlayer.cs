using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // public GameObject player;
    public Canvas launchUI;
    private AudioSource bgm;
    // private float rotateSpeed = 3f;
    // private float cameraDistance = 6f;
    // private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        // offset = transform.position - player.transform.position;
        bgm = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (launchUI.gameObject.activeSelf)
        {
            if (bgm.isPlaying)
            {
                bgm.Stop();
            }
        } else
        {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
        }
    }

    void LateUpdate()
    {
        // if (GameManager.isGameActive)
        // {
        //     offset = offset.normalized * cameraDistance;
        //     transform.position = player.transform.position + offset;
        //
        //     transform.LookAt(player.transform.position + new Vector3(0f, 1f, 0f));
        //
        //     float inputX = Input.GetAxis("Mouse X");
        //
        //     transform.RotateAround(player.transform.position, Vector3.up, rotateSpeed * inputX);
        //
        //     offset = transform.position - player.transform.position;
        // }
    }
}
