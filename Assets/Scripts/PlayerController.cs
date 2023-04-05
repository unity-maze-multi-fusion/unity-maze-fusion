using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Animator anim;
    private Rigidbody rb;
    private Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    private float gravityModifier = 5f;
    private float speed;
    private float inputH;
    private float inputV;
    private Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Physics.gravity = gravityModifier * gravity;
        dir = new Vector3(0f, 90f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isGameActive)
        {
            inputH = Input.GetAxis("Horizontal");
            inputV = Input.GetAxis("Vertical");
            speed = Mathf.Max(Mathf.Abs(inputH), Mathf.Abs(inputV));
            anim.SetFloat("Speed", speed);
            rb.AddForce(Vector3.down, ForceMode.Impulse);

            if (inputV > 0 && inputH == 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y, 0f);
            } else if (inputV < 0 && inputH == 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y - 180f, 0f);
            } else if (inputV == 0 && inputH > 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y + 90f, 0f);
            } else if (inputV == 0 && inputH < 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y - 90f, 0f);
            } else if (inputV > 0 && inputH > 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y + 45f, 0f);
            } else if (inputV < 0 && inputH > 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y + 135f, 0f);
            } else if (inputV > 0 && inputH < 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y - 45f, 0f);
            } else if (inputV < 0 && inputH < 0) {
                dir = new Vector3(0f, cam.transform.localEulerAngles.y - 135f, 0f);
            } 

            transform.localEulerAngles = dir;

        } else {
            anim.SetFloat("Speed", 0f);
        }
        
    }

}
