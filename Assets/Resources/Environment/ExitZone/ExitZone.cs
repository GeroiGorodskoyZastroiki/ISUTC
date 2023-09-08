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
        Player player;
        collider.TryGetComponent<Player>(out player);
        if (player)
        {
            int itemsCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
            if (itemsCount == 0)
            {
                if (player.NetworkObject.IsOwner) GameManager.Instance.MakePlayerSpectator();
                else Destroy(player.gameObject);
            }
        }
    }
}
