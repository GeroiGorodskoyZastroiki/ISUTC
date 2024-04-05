using UnityEngine;

public class SpectatorUI : MonoBehaviour
{
    private void OnEnable()
    {
        UIManager.HideCursor(false);
    }

    private void Update()
    {
        //сделать условное включение или отключение в зависимости от кол-ва игроков
    }

    public void NextPlayer()
    {
        if (GameManager.Instance.Players.Count >= SteamworksManager.Instance.Lobby.MemberCount - 1) return;
        var nextPlayer = GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<PlayerNetwork>().gameObject, true);
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        nextPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    public void PrevPlayer()
    {
        if (GameManager.Instance.Players.Count >= SteamworksManager.Instance.Lobby.MemberCount - 1) return;
        var prevPlayer = GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<PlayerNetwork>().gameObject, false);
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        prevPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }
}
