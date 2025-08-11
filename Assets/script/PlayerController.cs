using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 1f;

    private PlayerInputActions inputActions;
    private CharacterController controller;

    private Vector2 inputMovement;
    private Vector2 inputLook;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;

    private float xRotation = 0f;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.player.move.performed += ctx => inputMovement = ctx.ReadValue<Vector2>();
        inputActions.player.move.canceled += _ => inputMovement = Vector2.zero;

        inputActions.player.look.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        inputActions.player.look.canceled += _ => inputLook = Vector2.zero;

        inputActions.player.jump.performed += _ => Jump();

        inputActions.player.sprint.performed += _ => isSprinting = true;
        inputActions.player.sprint.canceled += _ => isSprinting = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = inputLook.x * lookSensitivity;
        float mouseY = inputLook.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * inputMovement.x + transform.forward * inputMovement.y;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
