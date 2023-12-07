using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using System.Linq;

public class PlayerLobbyUI : MonoBehaviour
{
    public PlayerNetwork PlayerNetwork { get; private set; }
    public Friend SteamUser;
    public int Skin;

    public TMP_Text ReadyStatus;
    public GameObject SkinSelection;

    public void Awake() => GetComponent<Canvas>().worldCamera = Camera.main;

    public async void SetPlayerInfo(GameObject player)
    {
        PlayerNetwork = player.GetComponent<PlayerNetwork>();
        //Debug.Log($"Owner id: {player.GetComponent<NetworkObject>().OwnerClientId}");
        //Debug.Log($"Member count: {SteamworksManager.Instance.lobby.Value.Members.ToArray().Length}"); //добавить steamOwner
        SteamUser = SteamworksManager.Instance.Lobby.Members.Single(x => x.Id.Value == player.GetComponent<PlayerNetwork>().SteamId.Value); //index is outside of bounds
        GetComponentInChildren<TMP_Text>().text = SteamUser.Name;
        if (!SteamUser.IsMe) Destroy(SkinSelection);
        PlayerNetwork.IsReady.OnValueChanged += GetReady;
        GetReady(PlayerNetwork.IsReady.Value, PlayerNetwork.IsReady.Value);
        GetComponentInChildren<RawImage>().texture = (await SteamUser.GetLargeAvatarAsync())?.Convert(); //result
    }

    public void SetReady()
    {
        if (PlayerNetwork.IsLocalPlayer)
        {
            PlayerNetwork.IsReady.Value = !PlayerNetwork.IsReady.Value;
            ReadyStatus.text = PlayerNetwork.IsReady.Value ? "Ready" : "---";
        }
    }

    public void GetReady(bool previous, bool current) => ReadyStatus.text = current ? "Ready" : "---";

    public void NextSkin()
    {
        Skin = Skin == 3 ? 0 : Skin + 1;
        PlayerNetwork.Skin.Value = Skin;
    }

    public void PrevSkin()
    {
        Skin = Skin == 0 ? 3 : Skin - 1;
        PlayerNetwork.Skin.Value = Skin;
    }
}
