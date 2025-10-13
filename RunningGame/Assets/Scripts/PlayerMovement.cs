using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Camera mainCamera;
    private Vector2 moveInput;
    private Vector2 cameraInput;
    public float speed = 5f;
    public float cameraSensitivity = 1f;
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
        //Debug.Log(moveInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
        Debug.Log("Camera input: " + cameraInput);
    }

    private void Update()

    {
        // --- Movement ---
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * speed * Time.deltaTime);

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