using UnityEngine;
using TMPro;

public class ClickToPlaceWithPreview : MonoBehaviour
{
    [Header("Основные настройки")]
    public GameObject previewObject; // Простой объект для превью (например, полупрозрачный куб)
    public GameObject objectToPlace; // Объект для спавна по клику
    public LayerMask placementMask; // Слои, по которым можно размещать объекты
    public float yOffset = 0.5f; // Смещение по высоте
    public TMP_Text inventoryText;

    private Camera mainCamera;
    private bool canPlace = false;

    [Header("Настройки уровня")]
    public int Rabbits = 5; //Кроликов в инвентаре

    void Start()
    {
        inventoryText.text = "Inventory Rabbit " + Rabbits;
        mainCamera = Camera.main;

        // Создаем превью-объект и делаем его полупрозрачным
        if (previewObject != null)
        {
            previewObject = Instantiate(previewObject);
            SetMaterialTransparent(previewObject);
        }
    }

    void Update()
    {
        if (!GameController.isPaused)
        {
            UpdatePreviewPosition();

            // Размещаем объект по ЛКМ
            if (Input.GetMouseButtonDown(0) && canPlace && Rabbits > 0)
            {
                Instantiate(objectToPlace, previewObject.transform.position, transform.rotation);
                Rabbits--;
                inventoryText.text = "Inventory Rabbit " + Rabbits;
            }   
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
            color.a = 0.5f; // Фиксированная прозрачность
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