using UnityEngine;
using UnityEngine.UI;

public class SpectatorUI : MonoBehaviour
{
    private void OnEnable()
    {
        UIManager.HideCursor(false);
    }

    private void Update()
    {
        if (GameManager.Instance.Players.Count < 3)
            UIManager.Close(UIManager.Spectator);
    }

    public void NextPlayer()
    {
        var nextPlayer = GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<PlayerNetwork>().gameObject, true);
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        nextPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    public void PrevPlayer()
    {
        var prevPlayer = GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<PlayerNetwork>().gameObject, false);
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        prevPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }
}
