using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 3f;          // Скорость движения вперед
    public float rotationSpeed = 90f;     // Скорость поворота (градусы в секунду)
    public float minIdleTime = 1f;        // Минимальное время стояния
    public float maxIdleTime = 3f;        // Максимальное время стояния
    public float minMoveTime = 2f;        // Минимальное время движения
    public float maxMoveTime = 5f;        // Максимальное время движения
    public int grassToNew = 5;            // Сколько надо съесть чтобы размножиться
    public float golod = 5;               // Сколько времени до режима голод
    public float detectionRadius = 10f;    // Радиус поиска травы
    public float lifeTime = 15;

    public GameObject targetGrass;        // Целевая трава
    private float maxGolod;
    private int maxGrassToNew;         // Сколько нужно съесть травы чтобы появился новый кролик
    private Animator animator;
    private CharacterController characterController;
    private float timer;
    public MovementState currentState;
    private float currentStateDuration;
    private Vector3 moveDirection;

    public GameObject rabbit;

    public enum MovementState
    {
        Idle,
        MovingForward,
        TurningLeft,
        TurningRight,
        Golod
    }

    private void Death()
    {
        animator.SetBool("death", true);
    }

    void Start()
    {
        //Сохраняем начальные значения уровня голода и уровня количества травы для размножения
        maxGolod = golod;
        maxGrassToNew = grassToNew;

        //Регистрируем действие смерть по истечению времени жизни
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
        golod -= Time.deltaTime; //голод постоянно растет, 0 голоден


        if (timer >= currentStateDuration)
        {
            // Выбираем новое случайное состояние 
            if (golod >= 0)
            {
                MovementState newState = (MovementState)Random.Range(0, System.Enum.GetValues(typeof(MovementState)).Length - 1); //-1 чтобы не выбрать голод
                SetNewState(newState);
            } else
            {
                //проголодался
                SetNewState(MovementState.Golod);
            }

            
        }

        // Обработка текущего состояния
        switch (currentState)
        {
            case MovementState.Golod:
                if (targetGrass == null) FindClosestGrass();
                if (targetGrass != null) MoveToTarget();
                SetNewState(MovementState.MovingForward);
                //Действия по голоду
                break;

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

        // Применяем движение через CharacterController
        if (characterController.enabled)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // Управление анимацией через параметр Speed
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
        if (other.CompareTag("grass") && golod <= 0)
        {
            SetNewState(MovementState.Idle);
            golod = maxGolod; // Сбрасываем голод
            Debug.Log("Достиг травы. Голод утолен!");

            Destroy(other.gameObject);
            grassToNew--;
            if (grassToNew <= 0)
            {
                grassToNew = maxGrassToNew;
                GameObject newRabbit = Instantiate(rabbit, transform.position, Quaternion.identity) as GameObject;
     
            }
        }



    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("grass") && golod <= 0)
        {
            SetNewState(MovementState.MovingForward);
        }
    }

    void FindClosestGrass()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;
        GameObject closestGrass = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("grass"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGrass = hitCollider.gameObject;
                }
            }
        }

        if (closestGrass != null)
        {
            targetGrass = closestGrass;
            Debug.Log($"Найдена трава! Иду к цели: {targetGrass.name}");
        }
        else
        {
            SetNewState(MovementState.MovingForward); //Если трава не найдена бежать вперед 
            Debug.Log("В радиусе нет травы.");
        }
    }

    void MoveToTarget()
    {
        // Мгновенный поворот к цели (без плавности)
        Vector3 direction = targetGrass.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        }
      

    // Визуализация радиуса поиска в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}