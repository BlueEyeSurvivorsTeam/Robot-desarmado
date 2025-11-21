using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip sound;

    public void PlaySound()
    {
        if(sound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.RandomPitch();
            AudioManager.Instance.PlaySFX(sound);
            AudioManager.Instance.ResetPitch();
        }
    }
}
