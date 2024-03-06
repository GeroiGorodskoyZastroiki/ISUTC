using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Toggle _joinableToogle;

    [SerializeField] private GameObject _playerInfoUIPrefab;
    [HideInInspector] public PlayerLobbyUI OwnerPLUI;

    private void OnEnable()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _startGameButton.gameObject.SetActive(true);
            _joinableToogle.gameObject.SetActive(true);
        }
        else
        {
            _startGameButton.gameObject.SetActive(false);
            _joinableToogle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf && NetworkManager.Singleton.IsHost) UpdateStartGameButton();
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        if (FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None).Count(x => x.IsReady.Value == false) > 0) return;
        SteamworksManager.Instance.StartGame();
    }

    private void UpdateStartGameButton()
    {
        _startGameButton.interactable = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None).Count(x => x.IsReady.Value == false) <= 0;
    }

    public void SetReady()
    {
        var PLUI = FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).SingleOrDefault(x => x.PlayerNetwork.gameObject == GameManager.Instance.Owner);
        if (PLUI) PLUI.SetReady();
    }

    public void SetJoinable() =>
        SteamworksManager.Instance.Lobby.SetJoinable(_joinableToogle.isOn);

    public void LeaveLobby()
    {
        SteamworksManager.Instance.LeaveLobby();
        UIManager.Open(UIManager.MainMenu);
    }

    public void CreatePlayerInfo(GameObject player)
    {
        Vector3 PLUIpos = Vector3.Lerp(player.transform.position, Camera.main!.transform.position, 0.1f);
        PLUIpos.y = 0;
        GameObject PLUIObject = Instantiate(_playerInfoUIPrefab, PLUIpos, player.transform.rotation);
        Billboard.RotateObjectTowards(PLUIObject.transform, Camera.main.transform);
        PLUIObject.GetComponent<PlayerLobbyUI>().SetPlayerInfo(player);
        if (player.GetComponent<NetworkObject>().IsOwner) OwnerPLUI = PLUIObject.GetComponent<PlayerLobbyUI>();
    }
}
