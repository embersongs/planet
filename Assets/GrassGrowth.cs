using UnityEngine;
using System.Collections;

[SelectionBase]
public class GrassGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public float spawnRadius = 1f;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public float growthDuration = 2f;
    [Tooltip("Time between spawn attempts (seconds)")]
    public float spawnInterval = 5f;
    [Range(0, 1)] public float spawnChance = 0.3f;
    [Tooltip("Radius to check for existing grass")]
    public float checkRadius = 0.5f;
    public LayerMask groundLayer;
    public LayerMask grassLayer;

    [Header("Optimization")]
    public int maxSpawnAttempts = 3;
    public float raycastHeight = 10f;
    public float raycastDistance = 20f;

    private float nextSpawnTime;
    private static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    void Start()
    {
        nextSpawnTime = Time.time + Random.Range(0, spawnInterval * 0.5f);
        gameObject.name = "grass"; // Ensure name is correct
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval;
            TrySpawnNewGrass();
        }
    }

    void TrySpawnNewGrass()
    {
        if (Random.value > spawnChance) return;

        StartCoroutine(SpawnWithChecks());
    }

    IEnumerator SpawnWithChecks()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnDirection = new Vector3(randomDirection.x, 0, randomDirection.y);
            Vector3 potentialPosition = transform.position + spawnDirection * spawnRadius;

            if (FindGroundPosition(ref potentialPosition))
            {
                if (!Physics.CheckSphere(potentialPosition, checkRadius, grassLayer))
                {
                    SpawnNewGrass(potentialPosition);
                    yield break;
                }
            }
            yield return waitForEndOfFrame;
        }
    }

    bool FindGroundPosition(ref Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * raycastHeight, Vector3.down,
            out RaycastHit groundHit, raycastDistance, groundLayer))
        {
            position = groundHit.point;
            return true;
        }
        return false;
    }

    void SpawnNewGrass(Vector3 position)
    {
        GameObject newGrass = Instantiate(gameObject, position, Quaternion.identity, transform.parent);
        newGrass.name = "grass"; // Set consistent name
        newGrass.transform.Rotate(0, Random.Range(0, 360), 0);
        StartCoroutine(GrowGrass(newGrass.transform));
    }

    IEnumerator GrowGrass(Transform grassTransform)
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.one * minScale;
        Vector3 endScale = Vector3.one * maxScale;

        grassTransform.localScale = startScale;

        while (elapsedTime < growthDuration)
        {
            grassTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / growthDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        grassTransform.localScale = endScale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}