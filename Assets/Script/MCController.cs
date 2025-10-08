using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MCController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;         // Movement speed
    public float jumpHeight = 1.5f;  // Jump strength
    public float gravity = -9.81f;   // Gravity value

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;   // drag your Camera into this slot in Inspector
    private float xRotation = 0f;

    [Header("interaction setting")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Camera playerCam;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 platformMotion = Vector3.zero;
    private bool isGrounded;

    private IInteractable currentTarget;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // lock to center of screen
        Cursor.visible = false;                   // hide it
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        DetectInteractable();
        HandleInteraction();
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move((move * speed + platformMotion) * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            platformMotion = Vector3.zero;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // stop flipping

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void DetectInteractable()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            currentTarget = hit.collider.GetComponent<IInteractable>();
            if (currentTarget != null)
            {
                // Optional: Show prompt on screen
                Debug.Log(currentTarget.GetPrompt());
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    void HandleInteraction()
    {
        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact();
        }
    }

    public void SetVelocity(Vector3 motion)
    {
        platformMotion = motion;
    }

}
