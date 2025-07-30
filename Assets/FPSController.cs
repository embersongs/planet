using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask; // Укажите слой, по которому определяется земля
    [SerializeField] private float groundCheckDistance = 0.2f; // Длина луча
    [SerializeField] private Transform groundCheckPoint; // Точка, откуда выпускается луч (обычно внизу персонажа)

    private CharacterController characterController;
    private Vector3 velocity;
    public bool isGrounded;
    private float xRotation = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleJump();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        // Проверка земли через Raycast
        isGrounded = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            groundCheckDistance,
            groundMask
        );

        // Альтернативный вариант: SphereCast для большей точности
        // isGrounded = Physics.SphereCast(
        //     groundCheckPoint.position,
        //     characterController.radius * 0.9f,
        //     Vector3.down,
        //     out RaycastHit hit,
        //     groundCheckDistance,
        //     groundMask
        // );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Фиксированная небольшая сила прижима к земле
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Визуализация луча в редакторе (для отладки)
    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed"))
        {
            // Увеличиваем скорость
            walkSpeed = 10f;

            // Уничтожаем объект Speed (опционально)
            Destroy(other.gameObject);

            // Запускаем корутину для возврата скорости к исходной через заданное время
        }
    }
}