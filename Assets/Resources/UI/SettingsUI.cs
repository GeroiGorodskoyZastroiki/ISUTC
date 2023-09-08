using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider gameVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider voiceChatVolumeSlider;
    [SerializeField] Slider mouseSensitivitySlider;

    void Start()
    {
        SetSliders();
    }

    public void SetSliders()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100);
        gameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 100);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);
        voiceChatVolumeSlider.value = PlayerPrefs.GetFloat("VoiceChatVolume", 100);
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1);
    }

    public void SetVolume(string name)
    {
        Slider slider = gameObject.GetComponentsInChildren<Slider>().Single(x => x.name == $"{name}Slider");
        audioMixer.SetFloat(name, ConvertToDB(slider.value));
        PlayerPrefs.SetFloat(name, slider.value);
    }

    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
    }

    public float ConvertToDB(float value) => (value >= 100 ? value * 0.2f - 20 : value * 0.8f - 80);

    public void Back()
    {
        //будет проблема с выключением PauseMenu
        if (GameManager.Instance.gameStarted) UIManager.Open(UIManager.pauseMenu);
        else UIManager.Open(UIManager.mainMenu);
    }
}
