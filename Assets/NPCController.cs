using UnityEngine;
using UnityEngine.UIElements;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 3f;          // �������� �������� ������
    public float rotationSpeed = 90f;     // �������� �������� (������� � �������)
    public float minIdleTime = 1f;        // ����������� ����� �������
    public float maxIdleTime = 3f;        // ������������ ����� �������
    public float minMoveTime = 2f;        // ����������� ����� ��������
    public float maxMoveTime = 5f;        // ������������ ����� ��������
    public int saturation = 0;
    public float lifeTime = 15;

    private Animator animator;
    private CharacterController characterController;
    private float timer;
    private MovementState currentState;
    private float currentStateDuration;
    private Vector3 moveDirection;

    public GameObject rabbit;

    private enum MovementState
    {
        Idle,
        MovingForward,
        TurningLeft,
        TurningRight
    }

    private void Death()
    {
        animator.SetBool("death", true);
    }

    void Start()
    {
        InvokeRepeating("Death", lifeTime - 1, 1);
        Destroy(gameObject, lifeTime);
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }

        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found!");
        }

        SetNewState(MovementState.Idle);
    }

    void Update()
    {
        if (animator == null || characterController == null) return;

        timer += Time.deltaTime;

        if (timer >= currentStateDuration)
        {
            // �������� ����� ��������� ���������
            MovementState newState = (MovementState)Random.Range(0, System.Enum.GetValues(typeof(MovementState)).Length);
            SetNewState(newState);
        }

        // ��������� �������� ���������
        switch (currentState)
        {
            case MovementState.Idle:
                moveDirection = Vector3.zero;
                break;

            case MovementState.MovingForward:
                moveDirection = transform.forward * moveSpeed;
                break;

            case MovementState.TurningLeft:
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                moveDirection = transform.forward * moveSpeed;
                break;

            case MovementState.TurningRight:
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                moveDirection = transform.forward * moveSpeed;
                break;
        }

        // ��������� �������� ����� CharacterController
        if (characterController.enabled)
        {
            characterController.SimpleMove(moveDirection);
        }

        // ���������� ��������� ����� �������� Speed
        float animationSpeed = (currentState == MovementState.Idle) ? 0f : 1f;
        animator.SetFloat("speed", animationSpeed);
    }

    private void SetNewState(MovementState newState)
    {
        currentState = newState;
        timer = 0f;

        switch (newState)
        {
            case MovementState.Idle:
                currentStateDuration = Random.Range(minIdleTime, maxIdleTime);
                break;

            case MovementState.MovingForward:
            case MovementState.TurningLeft:
            case MovementState.TurningRight:
                currentStateDuration = Random.Range(minMoveTime, maxMoveTime);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("grass"))
        {
            Destroy(other.gameObject);
            saturation--;
            if (saturation <= 0)
            {
                saturation = 5;
                GameObject newRabbit = Instantiate(rabbit, transform.position, Quaternion.identity) as GameObject;
     
            }
        }

    }
}