using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //References
    private CharacterController controller;
    GunSystem gunSystem;
    private Camera cam;

    [Header("Player Stats")]
    public float baseSpeed;
    public float currentSpeed;
    public float dashSpeed;
    public float dashTime;

    //Random
    [HideInInspector] public Vector3 facingDir;
    private Vector3 pointToLook;
    private Vector3 move;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        gunSystem = GetComponent<GunSystem>();
        cam = Camera.main;

        currentSpeed = baseSpeed;
    }

    private void Update()
    {
        Movement();
        MouseLook();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("DashCoroutine");
        }
    }

    private void Movement()
    {
        facingDir = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);

        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        controller.Move(move * Time.deltaTime * currentSpeed);

        if (move != Vector3.zero)
        {
            transform.LookAt(facingDir);
        }
    }

    private void MouseLook()
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(mousePos, out rayLength))
        {
            pointToLook = mousePos.GetPoint(rayLength);

            transform.LookAt(facingDir);
            gunSystem.gunBarrel.transform.LookAt(facingDir);
        }
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            controller.Move(move * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }


}
