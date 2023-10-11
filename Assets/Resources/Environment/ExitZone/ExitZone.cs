using Unity.Netcode;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void Update()
    {
        int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
        if (itemsCount == 0) gameObject.GetComponent<MeshRenderer>().enabled = true;
        else gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerStay(Collider collider)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        collider.TryGetComponent<PlayerTag>(out var player);
        if (player)
        {
            int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
            if (itemsCount == 0)
            {
                Debug.Log("ExitZonePreDespawn");
                Destroy(player);
                //player.GetComponent<NetworkObject>().Despawn();
                Debug.Log("ExitZoneAfterDespawn");
            }
        }
    }
}
