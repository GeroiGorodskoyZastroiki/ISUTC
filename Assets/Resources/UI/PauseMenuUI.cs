using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public bool Pause;

    private void OnEnable() => SwitchPause(true);

    private void OnDisable() => SwitchPause(false);

    private void SwitchPause(bool True) //это чё за высер?
    {
        UIManager.HideCursor(!True);
        Pause = True;
        var owner = GameManager.Instance.Owner;
        owner.GetComponent<PlayerMovement>().enabled = !True;
        owner.GetComponent<PlayerCamera>().enabled = !True;
        owner.GetComponent<PlayerAnimator>().enabled = !True;
        owner.GetComponentInChildren<AimTarget>().enabled = !True;
        owner.GetComponent<Animator>().SetFloat("VelocityX", 0);
        owner.GetComponent<Animator>().SetFloat("VelocityZ", 0);
    }

    public void Continue() => gameObject.SetActive(false);

    public void Settings() => UICommonMethods.Settings();

    public void Disconnect()
    {
        SteamworksManager.Instance.LeaveLobby();
        UIManager.Open(UIManager.MainMenu);
    }

    public void ExitGame() => UICommonMethods.ExitGame();
}
