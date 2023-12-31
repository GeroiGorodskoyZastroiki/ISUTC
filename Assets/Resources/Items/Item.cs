using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [MinValue(1.01f)][SerializeField] float _outlineThickness;

    void Start()
    {
        GetComponent<Renderer>().materials[1].SetFloat("_Thickness", 0);
    }

    public void DrawOutline(bool draw)
    {
        if (!draw) GetComponent<Renderer>().materials[1].SetFloat("_Thickness", 0);
        else GetComponent<Renderer>().materials[1].SetFloat("_Thickness", _outlineThickness);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ItemDespawnServerRpc(ServerRpcParams serverRpcParams = default) => this.NetworkObject.Despawn();
}

