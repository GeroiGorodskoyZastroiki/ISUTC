using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    [MinValue(1.01f)][SerializeField] private float _outlineThickness = 1.01f;
    [SerializeField] private AudioClip _pickUpSound;

    private void Start()
    {
        GetComponent<Renderer>().materials[1].SetFloat("_Thickness", 0);
    }

    public void DrawOutline(bool draw)
    {
        if (!draw) GetComponent<Renderer>().materials[1].SetFloat("_Thickness", 0);
        else GetComponent<Renderer>().materials[1].SetFloat("_Thickness", _outlineThickness);
    }

    [Rpc(SendTo.Everyone)]
    public void PickUpRpc()
    {
        AudioSource.PlayClipAtPoint(_pickUpSound, transform.position);
        NetworkObject.Despawn();
    }
}

