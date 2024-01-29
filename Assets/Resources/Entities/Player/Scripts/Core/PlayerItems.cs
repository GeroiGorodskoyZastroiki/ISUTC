using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    #region Data
    public GameObject FlashlightPrefab;
    [HideInInspector] public Transform Flashlight;
    #endregion Data

    #region References
    [HideInInspector] public Player Player;
    #endregion

    private void Start()
    {
        Flashlight = Instantiate(FlashlightPrefab, transform).transform;
    }
}
