using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isPaused { get; private set; }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void SetPause(bool boolean)
    {
        isPaused = boolean;
    }
}
