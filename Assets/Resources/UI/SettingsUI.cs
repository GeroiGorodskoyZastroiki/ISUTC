using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public AudioMixer AudioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _gameVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _voiceChatVolumeSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;

    private void Start()
    {
        SetSliders();
    }

    public void SetSliders()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100);
        _gameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 100);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);
        _voiceChatVolumeSlider.value = PlayerPrefs.GetFloat("VoiceChatVolume", 100);
        _mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1);
    }

    public void SetVolume(string name)
    {
        Slider slider = gameObject.GetComponentsInChildren<Slider>().Single(x => x.name == $"{name}Slider");
        AudioMixer.SetFloat(name, ConvertToDB(slider.value));
        PlayerPrefs.SetFloat(name, slider.value);
    }

    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", _mouseSensitivitySlider.value);
    }

    public float ConvertToDB(float value) => (value >= 100 ? value * 0.2f - 20 : value * 0.8f - 80);

    public void Back()
    {
        //будет проблема с выключением PauseMenu
        if (GameManager.Instance.GameStarted) UIManager.Open(UIManager.PauseMenu);
        else UIManager.Open(UIManager.MainMenu);
    }
}
