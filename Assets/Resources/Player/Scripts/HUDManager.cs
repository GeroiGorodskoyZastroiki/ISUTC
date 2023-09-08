using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject itemsFound;

    public bool showHUD = false;
    
    public float outlineWidth;

    Vector2 _mousePosition;
    private Color transparent = new Color(0, 0, 0, 0);
    private Item outlinedItem;
    Camera cam;
    Player owner;

    void Start()
    {
        cam = FindFirstObjectByType<Camera>();
        owner = GetComponentInParent<Player>();

        UIManager.HideCursor(true);
    }

    void Update()
    {
        DrawPickUp();
        ShowHUD();
    }

    void DrawPickUp()
    {
        bool pickUp = false;
        Ray ray = cam.ScreenPointToRay(_mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Item item;
            hit.transform.gameObject.TryGetComponent<Item>(out item);
            if (item != null)
            {
                outlinedItem = item;
                if (hit.distance < owner.PickUpDistance) pickUp = true;
            }
        }

        if (pickUp)
        {
            //crosshair.color = Color.white;
            crosshair.SetActive(true);
            outlinedItem.DrawOutline(true);
        }
        else
        {
            crosshair.SetActive(false);
            //crosshair.color = transparent;
            if (outlinedItem)
            {
                outlinedItem.DrawOutline(false);
                outlinedItem = null;
            }
        }
    }

    public void ShowHUD()
    {
        if (showHUD) itemsFound.SetActive(true);
        else itemsFound.SetActive(false);
        itemsFound.GetComponent<TMP_Text>().text = $"{GameManager.Instance.itemsCount - FindObjectsByType<Item>(FindObjectsSortMode.None).Length} / {GameManager.Instance.itemsCount}";
    }

    public void OnMousePosition(InputAction.CallbackContext context) =>
        _mousePosition = context.ReadValue<Vector2>();
}
