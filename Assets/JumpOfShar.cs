using UnityEngine;

public class JumpOfShar : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f; // Сила прыжка
    [SerializeField] private KeyCode jumpKey = KeyCode.F; // Клавиша для прыжка

    [Header("Random Direction")]
    [SerializeField] private float maxRandomAngle = 45f; // Максимальный угол отклонения от вертикали
    [SerializeField] private bool allowHorizontalJump = true; // Разрешить горизонтальные прыжки

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required for JumpOfShar script!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, стоит ли объект на земле
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void Jump()
    {
        // Генерируем случайное направление
        Vector3 randomDirection = GetRandomDirection();

        // Применяем силу прыжка
        rb.AddForce(randomDirection * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private Vector3 GetRandomDirection()
    {
        // Базовое направление - вверх
        Vector3 direction = Vector3.up;

        // Добавляем случайное отклонение
        float randomAngleX = allowHorizontalJump ? Random.Range(-maxRandomAngle, maxRandomAngle) : 0f;
        float randomAngleZ = allowHorizontalJump ? Random.Range(-maxRandomAngle, maxRandomAngle) : 0f;

        // Создаем кватернион для поворота
        Quaternion randomRotation = Quaternion.Euler(randomAngleX, 0f, randomAngleZ);

        // Применяем поворот к базовому направлению
        return randomRotation * direction;
    }
}

