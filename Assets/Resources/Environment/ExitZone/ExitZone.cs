using Unity.Netcode;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void Update()
    {
        int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
        if (itemsCount == 0) gameObject.GetComponent<MeshRenderer>().enabled = true;
        else gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        collider.TryGetComponent<PlayerTag>(out var player);
        if (player)
        {
            int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
            if (itemsCount == 0)
            {
                if (NetworkManager.Singleton.IsHost) player.GetComponent<NetworkObject>().Despawn();
                else player.GetComponent<NetworkObject>().NetworkHide(player.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }
}
