using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 moveInput;
    public float speed = 6f;
    private bool isSprinting = false;
    public float sprintMultiplier = 2f;

    private Vector3 velocity;
    private bool jumpHeld = false;
    private bool jumpQueued = false;
    private bool isGrounded = false;
    public float jumpHeight = 1.5f;
    public float gravity = -30f;

    private bool isCrouching = false;
    public float crouchMultiplier = 0.5f;

    public Camera mainCamera;
    private Vector2 cameraInput;
    public float cameraSensitivity = 0.5f;
    private float xRotation = 0f;   // Vertikální rotace
    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();
        // Pokud neexistuje komponent CharacterController, přidá se v Inspektoru

        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;   // Zamkne kurzor uprostřed obrazovky
        Cursor.visible = false;     // Aby nebyl kursor vidět
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        //Debug.Log("Movement input: " + moveInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
        //Debug.Log("Camera input: " + cameraInput);
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpHeld = true;
            jumpQueued = true;
        }
        else if (context.canceled) jumpHeld = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) isSprinting = true;
        else if (context.canceled) isSprinting = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started) isCrouching = true;
        else if (context.canceled) isCrouching = false;
    }
    
    private void Update()
    {
        isGrounded = controller.isGrounded;

        // --- Movement ---
        Vector3 move = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
        float currentSpeed;
        if (isCrouching) currentSpeed = speed * crouchMultiplier;
        else if (isSprinting) currentSpeed = speed * sprintMultiplier;
        else currentSpeed = speed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Jump ---
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (isGrounded && jumpHeld && jumpQueued)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;     // Aby neskočilo vícekrát, než se dostaneš ze země
        }
        if (!isGrounded && !jumpQueued) jumpQueued = true;

        // --- Gravitace ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Crouch ---
        float targetScaleY = isCrouching ? 0.5f : 1f;
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Lerp(scale.y, targetScaleY, 30 * Time.deltaTime);
        transform.localScale = scale;

        // --- Camera ---
        float mouseX = cameraInput.x * cameraSensitivity;
        float mouseY = cameraInput.y * cameraSensitivity;

        // Vertikální rotace
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Aby se nepřetočila kamera moc nahorů nebo dolů, zastavuje se na 90°
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontální rotace
        transform.Rotate(Vector3.up * mouseX);
    }
}