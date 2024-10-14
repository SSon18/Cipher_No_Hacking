using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        // Load the saved volume for the slider on start
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float volume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(volume);
        }
    }
}
