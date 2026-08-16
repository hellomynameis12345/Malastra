using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    private Vector2 lookInput;
    public float lookSensitivity = 0.1f;
    private float xRotation = 0f;
    public Transform cameraHolder;
    
    
    private float verticalVelocity;
    private CharacterController controller;
    private UnityEngine.InputSystem.PlayerInput playerInput;
    
   void Start()
{
    controller = GetComponent<CharacterController>();
    playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    void Update()
{
    // Read input
    Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
    lookInput = playerInput.actions["Look"].ReadValue<Vector2>();

    // Rotate player left/right
    transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

    // Rotate camera up/down
    xRotation -= lookInput.y * lookSensitivity;
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    // Calculate movement based on where the player is facing
    Vector3 movement = transform.right * input.x + transform.forward * input.y;

    // Prevent diagonal movement from being faster
    movement = Vector3.ClampMagnitude(movement, 1f);

    // Apply movement speed
    movement *= moveSpeed;

    // Apply gravity
    verticalVelocity += gravity * Time.deltaTime;
    movement.y = verticalVelocity;

    // Move the player
    controller.Move(movement * Time.deltaTime);
}
}
