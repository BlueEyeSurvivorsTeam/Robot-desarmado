using System.Collections;
using UnityEngine;
public class TransitionController : MonoBehaviour
{
    public float duration;
    public Animator anim;
    public void CallClose(GameObject panel)
    {
        if(GameManager.Instance != null) StartCoroutine(GameManager.Instance.Close(panel, duration));
    }
    public void CallOpen(GameObject panel)
    {
        if(GameManager.Instance != null) StartCoroutine(GameManager.Instance.Open(panel, duration));
    }
    public void SetPause(bool pause)
    {
        if(GameManager.Instance != null) GameManager.Instance.SetPause(pause);
    }
    public void StaticMouse(bool state)
    {
        if(state) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
    public void LoadScene(string nameScene)
    {
        if(GameManager.Instance != null) StartCoroutine(GameManager.Instance.TransitionScene(nameScene, anim));
    }
    public void LoadSceneReload(string nameScene)
    {
        if (GameManager.Instance != null) StartCoroutine(GameManager.Instance.TransitionSceneCheckpoint(nameScene, anim));
    }
    public void CloseGame()
    {
        if(GameManager.Instance != null) StartCoroutine(GameManager.Instance.TransitionClose(anim));
    }
}
