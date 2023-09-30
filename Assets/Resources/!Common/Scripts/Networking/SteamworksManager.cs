using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamworksManager : MonoBehaviour
{
    public static SteamworksManager Instance { get; private set; }
    private FacepunchTransport transport;

    public Lobby lobby;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SteamClient.RestartAppIfNecessary(480);
        transport = GetComponent<FacepunchTransport>();

        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        //SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberLeave;
        //SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        //SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    void OnApplicationQuit() => LeaveLobby();

    #region NetworkFlow
    public async Task StartHost()
    {
        lobby = (await SteamMatchmaking.CreateLobbyAsync(4)).Value;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient(SteamId steamId)
    {
        transport.targetSteamId = steamId.Value;
        NetworkManager.Singleton.StartClient();
    }

    public void StartGame() =>lobby.SetGameServer(lobby.Owner.Id);

    public void ClientDisconnected(ulong id)
    {
        if (id == 0) LeaveLobby();
    }

    public void LeaveLobby()
    {
        lobby.Leave();
        if (NetworkManager.Singleton) NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //UIManager.Open(UIManager.mainMenu);
        GameManager.Instance.gameStarted = false;
    }
    #endregion

    #region SteamworksCallbacks
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK) return;
        lobby.SetPublic();
        lobby.SetJoinable(true);

        SteamFriends.OpenGameInviteOverlay(lobby.Id);
    }

    private void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        
    }

    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        await lobby.Join();
        this.lobby = lobby;
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost) return;
        StartClient(this.lobby.Owner.Id);
        UIManager.Open(UIManager.lobby);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log(friend.Id);
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Id} is diconnected");
    }

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        UIManager.lobby.SetActive(false);
        GameManager.Instance.SpawnPlayers();
        if (NetworkManager.Singleton.IsHost)
        {
            GameManager.Instance.SpawnItems();
            GameManager.Instance.SpawnEnemy();
        }
        GameManager.Instance.gameStarted = true;
        lobby.SetJoinable(false);
    }
    #endregion
}
