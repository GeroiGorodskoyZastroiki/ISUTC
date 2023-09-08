using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public static GameObject mainMenu;
    public static GameObject pauseMenu;
    public static GameObject lobby;
    public static GameObject settings;
    public static GameObject spectator;
    static List<GameObject> allUI = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        SetRefs();
    }

    void Start() =>
        gameObject.GetComponentsInChildren<Transform>(true).Where(x => x.tag == "UIScreen").ToList().ForEach(x => allUI.Add(x.gameObject));

    void SetRefs()
    {
        mainMenu = GetComponentInChildren<MainMenuUI>(true).gameObject;
        pauseMenu = GetComponentInChildren<PauseMenuUI>(true).gameObject;
        lobby = GetComponentInChildren<LobbyUI>(true).gameObject;
        settings = GetComponentInChildren<SettingsUI>(true).gameObject;
        spectator = GetComponentInChildren<SpectatorUI>(true).gameObject;
    }

    void OnApplicationFocus(bool focus)
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
    public static void Open(GameObject UIToOpen)
    {
        allUI.ForEach(x => Close(x));
        UIToOpen.SetActive(true);
    }

    public static void Close(GameObject UIToClose) => UIToClose.SetActive(false);
}
