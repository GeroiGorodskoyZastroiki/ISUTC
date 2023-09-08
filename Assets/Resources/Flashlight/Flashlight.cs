using UnityEngine;
using UnityEngine.Rendering;

public class Flashlight : MonoBehaviour
{
    private void Awake()
    {
        GetComponentInParent<Player>().flashlightIsOn.OnValueChanged += OnFlashlightSwitch;
    }

    public void OnFlashlightSwitch(bool prev, bool curr)
    {
        if (UIManager.pauseMenu.activeSelf) return;
        GetComponentInChildren<Light>().enabled = curr;
        GetComponentInChildren<LensFlareComponentSRP>().enabled = curr;
        if (curr) GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.white);
        else GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
