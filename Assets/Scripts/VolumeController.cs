using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour
{
    public AudioClip buttonSound;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private List<Button> buttons = new List<Button>();
    private void Start()
    {
        if (musicSlider != null && sfxSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.AddListener(SetButtonSound);
            }
        }
    }
    private void SetButtonSound()
    {
        if(buttonSound != null) AudioManager.Instance.PlaySFX(buttonSound);
    }
    private void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
    }
    private void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }
    private void OnDestroy()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        if (buttons.Count != 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.RemoveListener(SetButtonSound);
            }
        }
    }
}