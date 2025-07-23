using UnityEngine;

public class Bigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("�������� ���������� �������")]
    public float growthSpeed = 0.1f;

    [Tooltip("������������ ������")]
    public float maxSize = 2.0f;

    [Tooltip("������ ���������� ��� ������")]
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
            // ����������� ������ �������
            transform.localScale += Vector3.one * growthSpeed * Time.deltaTime;

            // ���������, �������� �� ������������� �������
            if (transform.localScale.x >= maxSize)
            {
                transform.localScale = Vector3.one * maxSize;
                isGrowing = false;
            }
        }
    }

    /// <summary>
    /// ������ ���������� �������
    /// </summary>
    public void StartGrowing()
    {
        isGrowing = true;
    }

    /// <summary>
    /// ���������� ���������� �������
    /// </summary>
    public void StopGrowing()
    {
        isGrowing = false;
    }

    /// <summary>
    /// �������� ������ � ���������
    /// </summary>
    public void ResetSize()
    {
        transform.localScale = Vector3.one;
    }
}
