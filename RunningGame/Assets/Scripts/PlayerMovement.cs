using NUnit.Framework;
using Unity.Mathematics;
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
    private Vector3 slideDirection;
    private bool isSliding = false;
    public float slideDeceleration = 10f;
    private float slideSpeed = 0f;
    public float slopeAcceleration = 15f;
    public float maxSlideSpeed = 15f;

    public Camera mainCamera;
    private Vector2 cameraInput;
    public float cameraSensitivity = 0.5f;
    private float xRotation = 0f;
    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (context.started)
        {
            if (isSprinting && moveInput.y > 0f)
            {
                isSliding = true;
                slideDirection = transform.forward;
                slideSpeed = speed * sprintMultiplier;
            }
            else isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
            isSliding = false;
        }
    }
    
    private void Update()
    {
        isGrounded = controller.isGrounded;

        // --- Movement ---
        Vector3 move = Vector3.zero;
        if (!isSliding) move = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));

        float currentSpeed;
        if (isSliding)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                Vector3 groundNormal = hit.normal;
                Vector3 camForward = mainCamera.transform.forward;
                Vector3 slideDir = Vector3.ProjectOnPlane(camForward, groundNormal).normalized;

                slideDirection = Vector3.Lerp(slideDirection, slideDir, Time.deltaTime * 10f);

                float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
                if (slopeAngle > 5f)
                {
                    float slopeFactor = Mathf.Sign(Vector3.Dot(slideDir, Vector3.down)) * slopeAngle / 45f;
                    slideSpeed += slopeFactor * slopeAcceleration * Time.deltaTime;
                }

                slideSpeed = Mathf.MoveTowards(slideSpeed, 0f, slideDeceleration * Time.deltaTime);
                slideSpeed = Mathf.Clamp(slideSpeed, 0f, maxSlideSpeed);
            }

            move = slideDirection;
            currentSpeed = slideSpeed;

            if (slideSpeed <= speed * crouchMultiplier)
            {
                isSliding = false;
                isCrouching = true;
                slideSpeed = 0f;
            }
        }
        else if (isCrouching) currentSpeed = speed * crouchMultiplier;
        else if (isSprinting) currentSpeed = speed * sprintMultiplier;
        else currentSpeed = speed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Jump ---
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (isGrounded && jumpHeld && jumpQueued)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }
        if (!isGrounded && !jumpQueued) jumpQueued = true;

        // --- Gravitace ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Crouch ---
        float targetScaleY = (isCrouching || isSliding) ? 0.5f : 1f;
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Lerp(scale.y, targetScaleY, 30 * Time.deltaTime);
        transform.localScale = scale;

        // --- Camera ---
        float mouseX = cameraInput.x * cameraSensitivity;
        float mouseY = cameraInput.y * cameraSensitivity;

        // Vertikální rotace
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontální rotace
        transform.Rotate(Vector3.up * mouseX);
    }
}