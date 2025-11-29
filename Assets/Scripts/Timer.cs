using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent loseEvent;
    public TextMeshProUGUI timerText;
    public float timeInSeconds = 300f;
    float currentTime;
    bool end;

    private void Start()
    {
        if(GameManager.Instance.currentTime < timeInSeconds)
        {
            currentTime = GameManager.Instance.currentTime;
        }
        else
        {
            currentTime = timeInSeconds;
        }
    }
    void Update()
    {
        if (!GameManager.Instance.isPaused)
        {
            currentTime -= Time.deltaTime;
            if (timeInSeconds < 0)
            {
                timeInSeconds = 0;
                if(!end)
                {
                    loseEvent.Invoke();
                    end = true;
                }
            }
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    public void SetTimeCheckpoint()
    {
        GameManager.Instance.SetCurrentTime(currentTime);
    }
    public void ResetTime()
    {
        GameManager.Instance.SetCurrentTime(timeInSeconds);
    }
}
