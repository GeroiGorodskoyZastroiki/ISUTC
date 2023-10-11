using UnityEngine;

public class SpectatorUI : MonoBehaviour
{
    void OnEnable()
    {
        UIManager.HideCursor(false);
    }

    public void NextPlayer() //null refernce exception
    {
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<Player>(true).gameObject, true)
            .GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    public void PrevPlayer()
    {
        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        GameManager.Instance.GetPlayer(GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<Player>(true).gameObject, false)
            .GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }
}
