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
                Debug.Log("ItemsCount=0");
                if (player.NetworkObject.IsOwner) GameManager.Instance.MakePlayerSpectator();
                Debug.Log("AfterSpectator");
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("CollisionOnHost");
                    player.GetComponent<NetworkObject>().Despawn();
                }
            }
        }
    }
}
