using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class MCController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;         // Movement speed
    public float jumpHeight = 1.5f;  // Jump strength
    public float gravity = -9.81f;   // Gravity value

    [Header("Knockback")]
    private Vector3 knockbackVelocity;
    [SerializeField] private float knockbackDecay = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;   // drag your Camera into this slot in Inspector
    private float xRotation = 0f;

    [Header("Fall Damage Settings")]
    [SerializeField] float fallDamageThreshold = -15f; // Start taking damage below this speed
    [SerializeField] float fallDamageMultiplier = 2f;  // Scales damage based on fall speed
    private float fallStartY;
    private bool isFalling;

    [Header("interaction setting")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] float sphereRadius = 0.5f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Camera playerCam;
    [SerializeField] GameObject prompt;
    [SerializeField] GameObject promptIMG;

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
        if (OxygenPlayer.Instance.IsDepleted())
            Dead();
    }

    void HandleMovement()
    {
        // Ground check
        bool wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            // Just landed!
            if (isFalling)
            {
                ApplyFallDamage();
                isFalling = false;
            }

            velocity.y = -2f;
        }
        else if (!isGrounded && !wasGrounded)
        {
            if (!isFalling && velocity.y < -2f)
            {
                fallStartY = transform.position.y;
                isFalling = true;
            }
        }

        // WASD movement
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

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Apply knockback movement
        if (knockbackVelocity.magnitude > 0.1f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
        }
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
        //Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(ray, out hit, interactRange, interactLayer)) 
        { 
            currentTarget = hit.collider.GetComponent<IInteractable>(); 
            if (currentTarget != null) 
            { 
                prompt.SetActive(true); 
                promptIMG.SetActive(true); 
                TMP_Text text = prompt.GetComponent<TMP_Text>(); 
                text.text = currentTarget.GetPrompt(); 
                // Optional: Show prompt on screen
                // Debug.Log(currentTarget.GetPrompt());
            } 
        } 
        else 
        { 
            prompt.SetActive(false); 
            promptIMG.SetActive(false); 
            currentTarget = null; 
        } 
    }

    void HandleInteraction()
    {
        if (currentTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentTarget.OnInteractStart();
            }

            if (Input.GetKey(KeyCode.E))
            {
                currentTarget.OnInteractHold();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                currentTarget.OnInteractEnd();
            }
        }
    }

    public void SetVelocity(Vector3 motion)
    {
        platformMotion = motion;
    }

    void OnDrawGizmosSelected()
    {
        if (playerCam == null) return;

        // Visualize the forward ray direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(playerCam.transform.position, playerCam.transform.forward * interactRange);

        // Visualize the "sweep" of the SphereCast (start and end spheres)
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // semi-transparent green

        // Start sphere (where cast begins)
        Gizmos.DrawWireSphere(playerCam.transform.position, sphereRadius);

        // End sphere (where cast ends)
        Gizmos.DrawWireSphere(playerCam.transform.position + playerCam.transform.forward * interactRange, sphereRadius);
    }

    void Dead(){
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void ApplyFallDamage()
    {
        float fallDistance = fallStartY - transform.position.y;
        Debug.Log($"fall distance {fallDistance}");
        if (fallDistance > Mathf.Abs(fallDamageThreshold))
        {
            float damage = (fallDistance - Mathf.Abs(fallDamageThreshold)) * fallDamageMultiplier;
            OxygenPlayer.Instance.Drain(damage);
            Debug.Log($"Took {damage} fall damage! Oxygen left: {OxygenPlayer.Instance.currentOxygen}");
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction * force;
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
