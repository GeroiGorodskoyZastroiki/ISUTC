using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject enemy;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public List<GameObject> players = new List<GameObject>(); //по€вл€ютс€ после старта игры
    public int itemsCount;
    public GameObject[] items;
    [HideInInspector] public bool gameStarted;
    public InputActionAsset inputActionAsset;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (players.Count == 0 && gameStarted)
        {
            UIManager.pauseMenu.GetComponent<PauseMenuUI>().Disconnect();
            gameStarted = false;
        }
    }

    void Start()
    {
        LoadSettings();
        SceneManager.LoadSceneAsync("Banka");
    }

    #region OnLobbyGameCreated
    public void SpawnPlayers()
    {
        players = FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Select(x => x.gameObject).ToList();
        var spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            player.transform.position = spawnPoints[i].transform.position;
            player.transform.rotation = spawnPoints[i].transform.rotation;
            if (player.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                Camera.main.gameObject.SetActive(false);
                player.GetComponent<Player>().enabled = true;
            }
        }

        FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).ToList().ForEach(x => Destroy(x.gameObject));
    }

    public void SpawnEnemy()
    {
        var enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint").ToList();
        var randomSpawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
        GameObject enemy = Instantiate(this.enemy, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation);
        enemy.GetComponent<Enemy>().enabled = true;
        enemy.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnItems()
    {
        var itemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
        for (int i = 0; i < itemsCount; i++)
        {
            var selectedSpawnPoint = itemSpawnPoints[Random.Range(0, itemSpawnPoints.Count)];
            GameObject item = Instantiate(items[Random.Range(0, items.Length)], selectedSpawnPoint.transform.position + new Vector3(0, 0.016f, 0), selectedSpawnPoint.transform.rotation);
            item.GetComponent<NetworkObject>().Spawn(true);
            itemSpawnPoints.Remove(selectedSpawnPoint);
        }
    }
    #endregion

    public GameObject GetPlayer(GameObject player, bool next)
    {
        Debug.Log($"Players count: {players.Count}");
        if (players.Count == 1) return player;
        var index = players.FindIndex(x => x == player);
        Debug.Log($"Next player is {players[index + 1].name}");
        return (next ? players[index + 1] : players[index - 1]);
    }

    public void MakePlayerSpectator()
    {
        if (players.Count == 0) return;
        players[0].GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        UIManager.Open(UIManager.spectator);
    }

    void LoadSettings()
    {
        foreach (var map in inputActionAsset.actionMaps) 
            foreach (var action in map)
                for (int i = 0; i < action.bindings.Count; i++)
                    if (PlayerPrefs.HasKey($"{map.name}{action.name}{i}Bind"))
                        action.LoadBindingOverridesFromJson(PlayerPrefs.GetString($"{map.name}{action.name}{i}Bind"));
    }
}