using Steamworks;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class PlayerNetwork : NetworkBehaviour
{
    #region Data
    public Friend Owner;
    [ReadOnly] public NetworkVariable<ulong> SteamId = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> Skin = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> IsFlashlightOn = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> IsReady = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion

    public override void OnNetworkSpawn()
    {
        StartCoroutine(Spawn());
        Skin.OnValueChanged += OnSkinChanged;
        UpdateSkin();
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("OnNetworkDespawnPre");
        GameManager.Instance.Players.Remove(gameObject);

        if (!GameManager.Instance.GameStarted)
        {
            var PLUI = FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).SingleOrDefault(x => x.PlayerNetwork.gameObject == gameObject);
            if (PLUI) Destroy(PLUI.gameObject);
        }
        else if (NetworkObject.IsOwner) GameManager.Instance.MakePlayerSpectator();
        Debug.Log("OnNetworkDespawnAfter");
        base.OnNetworkDespawn();
        Debug.Log("OnNetworkDespawnAfterBase");
    }

    private IEnumerator Spawn()
    {
        if (IsOwner)
        {
            GameManager.Instance.Owner = gameObject;
            SteamId.Value = SteamClient.SteamId.Value;
        }
        else
        {
            SteamId.OnValueChanged += OnSteamIdChanged;
            UpdateSteamId();
        }
        Debug.Log($"SteamId: {SteamId.Value}");
        yield return new WaitForSeconds(2);
        UIManager.Lobby.GetComponent<LobbyUI>().CreatePlayerInfo(gameObject);
    }

    private void OnSteamIdChanged(ulong prev, ulong curr)
    {
        SteamId.Value = curr;
    }

    private void UpdateSteamId() => OnSteamIdChanged(SteamId.Value, SteamId.Value);

    private void OnSkinChanged(int prev, int curr)
    {
        Player.Appearance.Characters.ForEach(x => x.SetActive(false));
        Player.Appearance.Characters[curr].SetActive(true);
    }

    private void UpdateSkin() => OnSkinChanged(Skin.Value, Skin.Value);

}
