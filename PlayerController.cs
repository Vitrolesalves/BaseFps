using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [Header("Status Player")]
    public float velPlayer = 3f;
    public float velBase;
    public float jumpForce = 4f;
    public float sprintSpeed = 7f;


    public Vector3 jogadorEntradas;
    public CharacterController characterController;

    public Transform myCamera;


    private bool isGround;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform verificadorChao;
    [SerializeField] private float gravity = -9.81f;
    private float verticalSpeed;

    [SerializeField] private float radiusJump = 0.3f;

    // Camera

 




    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        myCamera = Camera.main.transform;

        velBase = velPlayer;

    }


    // Start is called before the first frame update
    void Start()
    {
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Mouse
        HandlerMouseState();

        // Player Controller
        HandlerMoviment();

        // Jump
        HandlerJump();



    }






    void HandlerMoviment()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, myCamera.eulerAngles.y, transform.eulerAngles.z);

        jogadorEntradas = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        jogadorEntradas = transform.TransformDirection(jogadorEntradas);

        isGround = Physics.CheckSphere(verificadorChao.position, radiusJump, layerMask);

        characterController.Move(jogadorEntradas * Time.deltaTime * velPlayer);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velPlayer = sprintSpeed;
        }
        else
        {
            velPlayer = velBase;
        }
    }

    void HandlerJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            verticalSpeed = MathF.Sqrt(jumpForce * -2 * gravity);
        }

        if (isGround && verticalSpeed < 0)
        {
            verticalSpeed = -1;
        }

        verticalSpeed += gravity * Time.deltaTime;

        characterController.Move(new Vector3(0, verticalSpeed, 0) * Time.deltaTime);
    }

    void HandlerMouseState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor();
            }

        }
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {

            LockCursor();
        }
    }


    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor Locked and Hidden");
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor Unlocked and Visible");
    }



}