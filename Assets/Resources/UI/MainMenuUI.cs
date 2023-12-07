using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void OnEnable() => UIManager.HideCursor(false);

    public async void CreateLobby()
    {
        await SteamworksManager.Instance.StartHost();
        UIManager.Open(UIManager.Lobby);
    }

    public void Settings() => UICommonMethods.Settings();

    public void ExitGame() => UICommonMethods.ExitGame();
}
