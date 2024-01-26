using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    #region Data
    public GameObject Crosshair;
    public GameObject ItemsFound;

    public bool IsHUDShown;

    public float OutlineWidth;

    private Color _transparent = new(0, 0, 0, 0);
    private Item _outlinedItem;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion

    private void OnEnable()
    {
        UIManager.HideCursor(true);
    }

    private void Update()
    {
        DrawPickUp();
        ShowHUD();
    }

    private void DrawPickUp()
    {
        bool pickUp = false;
        Ray ray = Camera.main!.ScreenPointToRay(Player.Input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Player.Interaction.PickUpDistance))
        {
            hit.transform.gameObject.TryGetComponent(out Item item);
            if (item != null)
            {
                _outlinedItem = item;
                pickUp = true;
            }
        }

        if (pickUp)
        {
            //crosshair.color = Color.white;
            Crosshair.SetActive(true);
            _outlinedItem.DrawOutline(true);
        }
        else
        {
            Crosshair.SetActive(false);
            //crosshair.color = transparent;
            if (_outlinedItem)
            {
                _outlinedItem.DrawOutline(false);
                _outlinedItem = null;
            }
        }
    }

    public void ShowHUD()
    {
        if (IsHUDShown) ItemsFound.SetActive(true);
        else ItemsFound.SetActive(false);
        ItemsFound.GetComponent<TMP_Text>().text = $"{GameManager.Instance.ItemsCount - FindObjectsByType<Item>(FindObjectsSortMode.None).Length} / {GameManager.Instance.ItemsCount}";
    }
}
