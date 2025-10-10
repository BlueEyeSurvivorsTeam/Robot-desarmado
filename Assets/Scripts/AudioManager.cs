
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(sfxSource);
            DontDestroyOnLoad(musicSource);
        }
        else
        {
            Destroy(gameObject);
            Destroy(sfxSource);
            Destroy(musicSource);
            return;
        }
        LoadAudioSettings();
    }
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip)
        {
            if (!musicSource.isPlaying) musicSource.Play();
            return;
        }
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    private void LoadAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
    public void RandomPitch()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
    }
    public void SetMusicVolume(float volumeValue)
    {
        musicSource.volume = volumeValue;
        PlayerPrefs.SetFloat(MUSIC_KEY, volumeValue);
        PlayerPrefs.Save();
    }
    public void SetSFXVolume(float volumeValue)
    {
        sfxSource.volume = volumeValue;
        PlayerPrefs.SetFloat(SFX_KEY, volumeValue);
        PlayerPrefs.Save();
    }
}
