using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider effectsSlider;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource effectsSource; // UI seslerini çalan kaynak


    void Start()
    {
        // Kayıtlı değerleri yükle
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);

        // Hemen uygula
        if (musicSource) musicSource.volume = musicSlider.value;
        if (effectsSource) effectsSource.volume = effectsSlider.value;

        // Dinleyicileri bağla
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        effectsSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicSource)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void OnEffectsVolumeChanged(float value)
    {
        if (effectsSource)
            effectsSource.volume = value;

        PlayerPrefs.SetFloat("EffectsVolume", value);
        PlayerPrefs.Save();
    }
}
