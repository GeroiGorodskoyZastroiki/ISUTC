using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    void Start()
    {
        SettingsUI settings = UIManager.settings.GetComponent<SettingsUI>();
        settings.audioMixer.SetFloat("MasterVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("MasterVolume", 100)));
        settings.audioMixer.SetFloat("GameVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("GameVolume", 100)));
        settings.audioMixer.SetFloat("MusicVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("MusicVolume", 100)));
        settings.audioMixer.SetFloat("VoiceChatVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("VoiceChatVolume", 100)));
    }
}
