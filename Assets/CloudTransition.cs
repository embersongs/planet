using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CloudTransition : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 movementDirection = Vector3.right; // Default direction
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float speedVariation = 0.5f; // Random variation in speed
    [SerializeField] private float directionVariation = 15f; // Degrees of random direction variation

    [Header("Visual Settings")]
    [SerializeField] private float fadeDistance = 20f; // Distance at which cloud fades
    [SerializeField] private float spawnDistance = 25f; // Distance to respawn when out of view
    [SerializeField] private float scaleVariation = 0.3f; // Random scale variation

    private Vector3 originalScale;
    private float currentSpeed;
    private Vector3 currentDirection;
    private Transform mainCamera;
    private Renderer cloudRenderer;
    private Material cloudMaterial;
    private Color originalColor;

    private void Awake()
    {
        cloudRenderer = GetComponent<Renderer>();
        cloudMaterial = cloudRenderer.material;
        originalColor = cloudMaterial.color;
        originalScale = transform.localScale;
        mainCamera = Camera.main.transform;

        InitializeCloud();
    }

    private void InitializeCloud()
    {
        // Apply random scale variation
        float scaleFactor = 1f + Random.Range(-scaleVariation, scaleVariation);
        transform.localScale = originalScale * scaleFactor;

        // Set random speed within variation
        currentSpeed = movementSpeed * (1f + Random.Range(-speedVariation, speedVariation));

        // Set slightly randomized direction
        currentDirection = Quaternion.Euler(
            Random.Range(-directionVariation, directionVariation),
            Random.Range(-directionVariation, directionVariation),
            Random.Range(-directionVariation, directionVariation)
        ) * movementDirection.normalized;

        // Randomize initial alpha slightly
        Color startColor = originalColor;
        startColor.a *= Random.Range(0.7f, 1f);
        cloudMaterial.color = startColor;
    }

    private void Update()
    {
        // Move the cloud
        transform.position += currentDirection * currentSpeed * Time.deltaTime;
    }

    private void RepositionCloud()
    {
        // Position cloud opposite to movement direction
        Vector3 repositionDirection = -currentDirection.normalized;

        // Reinitialize with new random variations
        InitializeCloud();
    }

    private void OnDestroy()
    {
        // Clean up material instance if we created one
        if (cloudMaterial != null && Application.isPlaying)
        {
            Destroy(cloudMaterial);
        }
    }
}