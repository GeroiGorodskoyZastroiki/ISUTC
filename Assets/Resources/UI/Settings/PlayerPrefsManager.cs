using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    private void Start()
    {
        SettingsUI settings = UIManager.Settings.GetComponent<SettingsUI>();
        settings.AudioMixer.SetFloat("MasterVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("MasterVolume", 100)));
        settings.AudioMixer.SetFloat("GameVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("GameVolume", 100)));
        settings.AudioMixer.SetFloat("MusicVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("MusicVolume", 100)));
        settings.AudioMixer.SetFloat("VoiceChatVolume", settings.ConvertToDB(PlayerPrefs.GetFloat("VoiceChatVolume", 100)));
    }
}
