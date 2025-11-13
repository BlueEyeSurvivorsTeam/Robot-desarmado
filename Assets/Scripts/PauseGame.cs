using UnityEngine;
using UnityEngine.Events;

public class PauseGame : MonoBehaviour
{
    public bool canPause = true;
    public UnityEvent pauseEvent;
    public KeyCode pauseKeyCode = KeyCode.Escape;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if(GameManager.Instance != null && !GameManager.Instance.isPaused && canPause && Input.GetKeyDown(pauseKeyCode))
        {
            pauseEvent.Invoke();
        }
    }
}
