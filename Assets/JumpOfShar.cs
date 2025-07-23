using UnityEngine;

public class JumpOfShar : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f; // ���� ������
    [SerializeField] private KeyCode jumpKey = KeyCode.F; // ������� ��� ������

    [Header("Random Direction")]
    [SerializeField] private float maxRandomAngle = 45f; // ������������ ���� ���������� �� ���������
    [SerializeField] private bool allowHorizontalJump = true; // ��������� �������������� ������

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
        // ���������, ����� �� ������ �� �����
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
        // ���������� ��������� �����������
        Vector3 randomDirection = GetRandomDirection();

        // ��������� ���� ������
        rb.AddForce(randomDirection * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private Vector3 GetRandomDirection()
    {
        // ������� ����������� - �����
        Vector3 direction = Vector3.up;

        // ��������� ��������� ����������
        float randomAngleX = allowHorizontalJump ? Random.Range(-maxRandomAngle, maxRandomAngle) : 0f;
        float randomAngleZ = allowHorizontalJump ? Random.Range(-maxRandomAngle, maxRandomAngle) : 0f;

        // ������� ���������� ��� ��������
        Quaternion randomRotation = Quaternion.Euler(randomAngleX, 0f, randomAngleZ);

        // ��������� ������� � �������� �����������
        return randomRotation * direction;
    }
}

