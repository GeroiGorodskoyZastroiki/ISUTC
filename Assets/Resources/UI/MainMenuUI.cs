using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    void OnEnable() => UIManager.HideCursor(false);

    public async void CreateLobby()
    {
        await SteamworksManager.Instance.StartHost();
        UIManager.Open(UIManager.lobby);
    }

    public void Settings() => UICommonMethods.Settings();

    public void ExitGame() => UICommonMethods.ExitGame();
}
