using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject Enemy;
    [HideInInspector] public GameObject Owner;
    [HideInInspector] public List<GameObject> Players = new(); //по€вл€ютс€ после старта игры
    public int ItemsCount;
    public GameObject[] Items;
    [HideInInspector] public bool GameStarted;
    public InputActionAsset InputActionAsset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Players.Count != 0 || !GameStarted) return;
        UIManager.PauseMenu.GetComponent<PauseMenuUI>().Disconnect();
        GameStarted = false;
    }

    private async void Start()
    {
        LoadSettings();
        await SceneManager.LoadSceneAsync("Banka");
        UIManager.Open(UIManager.MainMenu);
    }

    #region OnLobbyGameCreated
    public void SpawnPlayers()
    {
        Players = FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Select(x => x.gameObject).ToList();
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        for (int i = 0; i < Players.Count; i++)
        {
            GameObject player = Players[i];
            player.transform.SetPositionAndRotation(spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            if (player.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                Camera.main!.gameObject.SetActive(false);
                player.GetComponent<Player>().EnableComponents();
            }
        }

        FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).ToList().ForEach(x => Destroy(x.gameObject));
    }

    public void SpawnEnemy()
    {
        List<GameObject> enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint").ToList();
        GameObject randomSpawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
        GameObject enemy = Instantiate(Enemy, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation);
        enemy.GetComponent<Enemy>().enabled = true;
        enemy.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnItems()
    {
        List<GameObject> itemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
        for (int i = 0; i < ItemsCount; i++)
        {
            GameObject selectedSpawnPoint = itemSpawnPoints[Random.Range(0, itemSpawnPoints.Count)];
            GameObject item = Instantiate(Items[Random.Range(0, Items.Length)], selectedSpawnPoint.transform.position + new Vector3(0, 0.016f, 0), selectedSpawnPoint.transform.rotation);
            item.GetComponent<NetworkObject>().Spawn(true);
            itemSpawnPoints.Remove(selectedSpawnPoint);
        }
    }
    #endregion

    public GameObject GetPlayer(GameObject player, bool next)
    {
        Debug.Log($"Players count: {Players.Count}");
        if (Players.Count == 1) return player;
        int index = Players.FindIndex(x => x == player);
        Debug.Log($"Next player is {Players[index + 1].name}");
        return (next ? Players[index + 1] : Players[index - 1]);
    }

    public void MakePlayerSpectator()
    {
        Debug.Log(Players.Count);
        if (Players.Count == 0) return;
        Players[0].GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        UIManager.Open(UIManager.Spectator);
    }

    private void LoadSettings()
    {
        foreach (var map in InputActionAsset.actionMaps)
            foreach (var action in map)
                for (int i = 0; i < action.bindings.Count; i++)
                    if (PlayerPrefs.HasKey($"{map.name}{action.name}{i}Bind"))
                        action.LoadBindingOverridesFromJson(PlayerPrefs.GetString($"{map.name}{action.name}{i}Bind"));
    }
}