using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isPaused { get; private set; }

    public GameObject checkpointPos;
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
    public void SetCheckpointPos(GameObject pos)
    {
        checkpointPos = pos;
    }
}
