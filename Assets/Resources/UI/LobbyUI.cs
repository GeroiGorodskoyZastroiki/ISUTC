using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Toggle joinableToogle;

    [SerializeField] GameObject playerInfoUIPrefab;
    [HideInInspector] public PlayerLobbyUI ownerPLUI;

    void OnEnable()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(true);
            joinableToogle.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
            joinableToogle.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (gameObject.activeSelf == true && NetworkManager.Singleton.IsHost) UpdateStartGameButton();
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        if (FindObjectsByType<Player>(FindObjectsSortMode.None).Where(x => x.ready.Value == false).Count() > 0) return;
        SteamworksManager.Instance.StartGame();
    }

    void UpdateStartGameButton()
    {
        if (FindObjectsByType<Player>(FindObjectsSortMode.None).Where(x => x.ready.Value == false).Count() > 0)
            startGameButton.interactable = false;
        else startGameButton.interactable = true;
    }

    public void SetReady()
    {
        var PLUI = FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).SingleOrDefault(x => x.player == GameManager.Instance.owner);
        if (PLUI) PLUI.SetReady();
    }

    public void SetJoinable() =>
        SteamworksManager.Instance.lobby.SetJoinable(joinableToogle.isOn);

    public void LeaveLobby()
    {
        SteamworksManager.Instance.LeaveLobby();
        UIManager.Open(UIManager.mainMenu);
    }

    public void CreatePlayerInfo(GameObject player)
    {
        Vector3 PLUIpos = Vector3.Lerp(player.transform.position, Camera.main.transform.position, 0.1f);
        PLUIpos.y = 0;
        GameObject PLUIObject = Instantiate(playerInfoUIPrefab, PLUIpos, player.transform.rotation);
        Billboard.RotateObjectTowards(PLUIObject.transform, Camera.main.transform);
        PLUIObject.GetComponent<PlayerLobbyUI>().SetPlayerInfo(player);
        if (player.GetComponent<NetworkObject>().IsOwner) ownerPLUI = PLUIObject.GetComponent<PlayerLobbyUI>();
    }
}
