using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public static GameObject MainMenu;
    public static GameObject PauseMenu;
    public static GameObject Lobby;
    public static GameObject Settings;
    public static GameObject Spectator;
    private static readonly List<GameObject> _allUI = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        SetRefs();
    }

    private void Start() =>
        gameObject.GetComponentsInChildren<Transform>(true).Where(x => x.CompareTag("UIScreen")).ToList().ForEach(x => _allUI.Add(x.gameObject));

    private void SetRefs()
    {
        MainMenu = GetComponentInChildren<MainMenuUI>(true).gameObject;
        PauseMenu = GetComponentInChildren<PauseMenuUI>(true).gameObject;
        Lobby = GetComponentInChildren<LobbyUI>(true).gameObject;
        Settings = GetComponentInChildren<SettingsUI>(true).gameObject;
        Spectator = GetComponentInChildren<SpectatorUI>(true).gameObject;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child == transform) continue;
                if (child.gameObject.activeSelf)
                {
                    HideCursor(false);
                    return;
                }
            }
            HideCursor(true);
        }
    }

    public static void HideCursor(bool hasFocus)
    {
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hasFocus;
    }

    /// <summary>
    /// Closes all UI screens before opening new one
    /// </summary>
    public static void Open(GameObject UItoOpen)
    {
        _allUI.ForEach(Close);
        UItoOpen.SetActive(true);
    }

    public static void Close(GameObject UItoClose) => UItoClose.SetActive(false);
}
