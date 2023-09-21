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
        collider.TryGetComponent<Player>(out Player player);
        if (player)
        {
            int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
            if (itemsCount == 0)
            {
                if (player.NetworkObject.IsOwner) GameManager.Instance.MakePlayerSpectator();
                if (NetworkManager.Singleton.IsHost) player.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
