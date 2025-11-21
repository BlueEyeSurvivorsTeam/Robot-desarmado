using UnityEngine;

public class StepSound : MonoBehaviour
{
    public AudioClip stepSound;
    public JumpDetector jd;

    public void Step()
    {
        if (stepSound != null && AudioManager.Instance != null && jd.IsGrounded())
        {
            AudioManager.Instance.RandomPitch();
            AudioManager.Instance.PlaySFX(stepSound);
            AudioManager.Instance.ResetPitch();
        }
    }
}
