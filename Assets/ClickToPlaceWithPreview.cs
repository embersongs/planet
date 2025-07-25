using UnityEngine;

public class ClickToPlaceWithPreview : MonoBehaviour
{
    [Header("�������� ���������")]
    public GameObject previewObject; // ������� ������ ��� ������ (��������, �������������� ���)
    public GameObject objectToPlace; // ������ ��� ������ �� �����
    public LayerMask placementMask; // ����, �� ������� ����� ��������� �������
    public float yOffset = 0.5f; // �������� �� ������

    private Camera mainCamera;
    private bool canPlace = false;

    void Start()
    {
        mainCamera = Camera.main;

        // ������� ������-������ � ������ ��� ��������������
        if (previewObject != null)
        {
            previewObject = Instantiate(previewObject);
            SetMaterialTransparent(previewObject);
        }
    }

    void Update()
    {
        UpdatePreviewPosition();

        // ��������� ������ �� ���
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            Instantiate(objectToPlace, previewObject.transform.position, transform.rotation);
        }
    }

    void UpdatePreviewPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementMask))
        {
            canPlace = true;
            previewObject.transform.position = hit.point + Vector3.up * yOffset;
            SetPreviewColor(Color.green);
        }
        else
        {
            canPlace = false;
            SetPreviewColor(Color.red);
        }
    }

    void SetMaterialTransparent(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }
    }

    void SetPreviewColor(Color color)
    {
        if (previewObject == null) return;

        Renderer renderer = previewObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            color.a = 0.5f; // ������������� ������������
            renderer.material.color = color;
        }
    }

    void OnDestroy()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
}