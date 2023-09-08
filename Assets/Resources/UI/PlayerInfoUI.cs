using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using System.Linq;
using Unity.Netcode;

public class PlayerLobbyUI : MonoBehaviour
{
    public GameObject player;
    public Friend steamUser;
    public int skin = 0;

    public TMP_Text readyStatus;
    public GameObject skinSelection;

    public void Awake() => GetComponent<Canvas>().worldCamera = Camera.main;

    public async void SetPlayerInfo(GameObject player)
    {
        this.player = player;
        //Debug.Log($"Owner id: {player.GetComponent<NetworkObject>().OwnerClientId}");
        //Debug.Log($"Member count: {SteamworksManager.Instance.lobby.Value.Members.ToArray().Length}"); //добавить steamOwner
        steamUser = SteamworksManager.Instance.lobby.Members.Single(x => x.Id.Value == player.GetComponent<Player>().steamId.Value);//index is outside of bounds
        GetComponentInChildren<TMP_Text>().text = steamUser.Name;
        if (!steamUser.IsMe) Destroy(skinSelection);
        var networkPlayer = player.GetComponent<Player>();
        networkPlayer.ready.OnValueChanged += GetReady;
        GetReady(networkPlayer.ready.Value, networkPlayer.ready.Value);
        GetComponentInChildren<RawImage>().texture = (await steamUser.GetLargeAvatarAsync()).Value.Convert(); //result
    }

    public void SetReady()
    {
        if (player.GetComponent<Player>().IsLocalPlayer)
        {
            player.GetComponent<Player>().ready.Value = !player.GetComponent<Player>().ready.Value;
            readyStatus.text = player.GetComponent<Player>().ready.Value ? "Ready" : "---";
        }
    }

    public void GetReady(bool previous, bool current) => readyStatus.text = current ? "Ready" : "---";

    public void NextSkin()
    {
        skin = skin == 3 ? 0 : skin+1;
        player.GetComponent<Player>().skin.Value = skin;
    }

    public void PrevSkin()
    {
        skin = skin == 0 ? 3 : skin-1;
        player.GetComponent<Player>().skin.Value = skin;
    }
}
