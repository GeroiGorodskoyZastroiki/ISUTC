using UnityEngine;

public class SpectatorUI : MonoBehaviour
{
    void OnEnable()
    {
        UIManager.HideCursor(false);
    }

    public void NextPlayer()
    {
        Camera.main.gameObject.SetActive(false);
        GameManager.Instance.GetPlayer(Camera.main.GetComponentInParent<Player>(true).gameObject, true).GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    public void PrevPlayer()
    {
        Camera.main.gameObject.SetActive(false);
        GameManager.Instance.GetPlayer(Camera.main.GetComponentInParent<Player>(true).gameObject, false).GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }
}
