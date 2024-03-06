using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private void Awake()
    {
        GetComponentInParent<PlayerNetwork>().IsFlashlightOn.OnValueChanged += OnFlashlightSwitch;
    }

    public void OnFlashlightSwitch(bool prev, bool curr)
    {
        if (UIManager.PauseMenu.activeSelf) return;
        GetComponentInChildren<Light>().enabled = curr;
        //GetComponentInChildren<LensFlareComponentSRP>().enabled = curr;
        if (curr) GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.white);
        else GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
