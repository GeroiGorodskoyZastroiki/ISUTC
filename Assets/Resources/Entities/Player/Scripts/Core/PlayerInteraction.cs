using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    #region Data
    public float PickUpDistance => _pickUpDistance;
    [SerializeField] private float _pickUpDistance = 3f;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion

    public void PickUp()
    {
        var ray = Camera.main!.ScreenPointToRay(Player.Input.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _pickUpDistance))
        {
            hit.transform.gameObject.TryGetComponent(out Item item);
            if (item && hit.distance < PickUpDistance) item.ItemDespawnServerRpc();
        }
    }
}
