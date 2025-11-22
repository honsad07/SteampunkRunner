using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GameManager gameManager;
    public bool isPaused => isPaused;

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
    
    private int wallJumpCount = 0;
    public int maxWallJumps = 1;
    private Vector3 wallJumpPush = Vector3.zero;
    public float wallJumpPushStrength = 5f;
    public float wallJumpPushDuration = 0.2f;
    private float wallJumpPushTimer = 0f;
    private float airControlMultiplier = 0f;

    private bool isCrouching = false;
    private bool crouchHeld = false;
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
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
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

    public void OnWallJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TryWallJump();
        }
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
            crouchHeld = true;
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
            crouchHeld = false;
        }
    }

    private bool CanStand()
    {
        float checkHeight = 1f;
        Vector3[] offsets = {
            Vector3.zero,
            new Vector3(0, 0, 0.4f),
            new Vector3(0, 0, -0.4f),
            new Vector3(0.4f, 0, 0),
            new Vector3(-0.4f, 0, 0),
            new Vector3(0.2f, 0, 0.2f),
            new Vector3(-0.2f, 0, 0.2f),
            new Vector3(0.2f, 0, -0.2f),
            new Vector3(-0.2f, 0, -0.2f)
        };

        foreach (var offset in offsets)
        {
            if (Physics.Raycast(transform.position + offset, Vector3.up, checkHeight)) return false;
        }
        return true;
    }

    private bool IsTouchingWall()
    {
        float distance = 0.6f;
    
        Vector3[] directions = {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (-transform.forward + transform.right).normalized,
            (-transform.forward - transform.right).normalized
        };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, distance)) return true;
        }

        return false;
    }

    private void TryWallJump()
    {
        if (!isGrounded && IsTouchingWall() && wallJumpCount < maxWallJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            wallJumpCount++;
    
            Vector3 wallNormal;
            if (GetWallNormal(out wallNormal))
            {
                wallJumpPush = wallNormal * wallJumpPushStrength;
                wallJumpPushTimer = wallJumpPushDuration;
            }
        }
    }

    private bool GetWallNormal(out Vector3 wallNormal)
    {
        float distance = 0.6f;
        Vector3[] directions = {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (-transform.forward + transform.right).normalized,
            (-transform.forward - transform.right).normalized
        };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance))
            {
                wallNormal = hit.normal;
                return true;
            }
        }

        wallNormal = Vector3.zero;
        return false;
    }

    private void Update()
    {
        if (gameManager != null && gameManager.isPaused) return;
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            wallJumpCount = 0;
            airControlMultiplier = 1f;
        }
        else airControlMultiplier = 0.8f;

        if (!crouchHeld && (isCrouching || isSliding) && CanStand())
        {
            isCrouching = false;
            isSliding = false;
        }

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
            currentSpeed = slideSpeed * airControlMultiplier;

            if (slideSpeed <= speed * crouchMultiplier)
            {
                isSliding = false;
                isCrouching = true;
                slideSpeed = 0f;
            }
        }
        else if (isCrouching) currentSpeed = speed * crouchMultiplier * airControlMultiplier;
        else if (isSprinting) currentSpeed = speed * sprintMultiplier * airControlMultiplier;
        else currentSpeed = speed * airControlMultiplier;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- Wall jump push ---
        if (wallJumpPushTimer > 0f)
        {
            controller.Move(wallJumpPush * Time.deltaTime);
            wallJumpPushTimer -= Time.deltaTime;

            wallJumpPush = Vector3.Lerp(wallJumpPush, Vector3.zero, Time.deltaTime * 5f);
        }

        // --- Jump ---
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (isGrounded && jumpHeld && jumpQueued)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }
        if (!isGrounded && !jumpQueued) jumpQueued = true;

        // --- Gravity ---
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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}