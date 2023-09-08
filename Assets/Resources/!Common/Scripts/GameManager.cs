using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject enemy;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public List<GameObject> players = new List<GameObject>(); //по€вл€ютс€ после старта игры
    public int itemsCount;
    public GameObject[] items;
    [HideInInspector] public bool gameStarted;

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

    async void Start()
    {
        UIManager.Close(UIManager.settings); //???
        UIManager.Open(UIManager.mainMenu);
        await SceneManager.LoadSceneAsync("Banka", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("Init");
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
        var randomSpawnPoint = enemySpawnPoints[UnityEngine.Random.Range(0, enemySpawnPoints.Count)];
        GameObject enemy = Instantiate(this.enemy, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation);
        if (NetworkManager.Singleton.IsHost)
        {
            enemy.GetComponent<Enemy>().enabled = true;
            enemy.GetComponentInChildren<EnemyVision>().enabled = true;
        }
        enemy.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnItems()
    {
        var itemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
        if (itemsCount > itemSpawnPoints.Count) throw new ArgumentException("Parameter is invalid", nameof(itemsCount));
        for (int i = 0; i < itemsCount; i++)
        {
            var selectedSpawnPoint = itemSpawnPoints[UnityEngine.Random.Range(0, itemSpawnPoints.Count)];
            GameObject item = Instantiate(items[UnityEngine.Random.Range(0, itemSpawnPoints.Count)], selectedSpawnPoint.transform);
            item.GetComponent<NetworkObject>().Spawn(true);
            itemSpawnPoints.Remove(selectedSpawnPoint);
        }
    }
    #endregion

    public GameObject GetPlayer(GameObject player, bool next)
    {
        if (players.Count == 1) return player;
        var index = players.FindIndex(x => x == player);
        return (next ? players[index + 1] : players[index - 1]);
    }

    public void MakePlayerSpectator()
    {
        if (players.Count == 0) return;
        players[0].GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        UIManager.Open(UIManager.spectator);
        Destroy(owner);
    }
}
