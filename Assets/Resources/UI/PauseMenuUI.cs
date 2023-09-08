using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public bool pause;

    void OnEnable() => SwitchPause(true);

    void OnDisable() => SwitchPause(false);

    void SwitchPause(bool True) //это чё за высер?
    {
        UIManager.HideCursor(!True);
        pause = True;
        var owner = GameManager.Instance.owner;
        owner.GetComponent<Player>().enabled = !True;
        owner.GetComponentInChildren<AimTarget>().enabled = !True;
        owner.GetComponent<Animator>().SetFloat("VelocityX", 0);
        owner.GetComponent<Animator>().SetFloat("VelocityZ", 0);
    }

    public void Continue() => gameObject.SetActive(false);

    public void Settings() => UICommonMethods.Settings();

    public void Disconnect()
    {
        SteamworksManager.Instance.LeaveLobby();
        UIManager.Open(UIManager.mainMenu);
    }

    public void ExitGame() => UICommonMethods.ExitGame();
}
