using UnityEngine;

public class Bigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Скорость увеличения размера")]
    public float growthSpeed = 0.1f;

    [Tooltip("Максимальный размер")]
    public float maxSize = 2.0f;

    [Tooltip("Начать увеличение при старте")]
    public bool growOnStart = true;

    private bool isGrowing = false;

    void Start()
    {
        if (growOnStart)
        {
            StartGrowing();
        }
    }

    void Update()
    {
        if (isGrowing)
        {
            // Увеличиваем размер объекта
            transform.localScale += Vector3.one * growthSpeed * Time.deltaTime;

            // Проверяем, достигли ли максимального размера
            if (transform.localScale.x >= maxSize)
            {
                transform.localScale = Vector3.one * maxSize;
                isGrowing = false;
            }
        }
    }

    /// <summary>
    /// Начать увеличение размера
    /// </summary>
    public void StartGrowing()
    {
        isGrowing = true;
    }

    /// <summary>
    /// Остановить увеличение размера
    /// </summary>
    public void StopGrowing()
    {
        isGrowing = false;
    }

    /// <summary>
    /// Сбросить размер к исходному
    /// </summary>
    public void ResetSize()
    {
        transform.localScale = Vector3.one;
    }
}
