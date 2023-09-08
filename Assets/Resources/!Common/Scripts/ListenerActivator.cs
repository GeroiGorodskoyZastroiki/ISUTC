using UnityEngine;

public class ListenerActivator : MonoBehaviour
{
    void OnEnable()
    {
        SteamAudio.SteamAudioManager.Singleton.ChangeListener(transform);
        tag = "MainCamera";
    }

    private void OnDisable()
    {
        tag = "Untagged";
    }
}
