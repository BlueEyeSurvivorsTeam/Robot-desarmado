using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public IEnumerator Open(GameObject panel, float duration)
    {
        float elapsedTime = 0f;
        panel.transform.localScale = Vector3.zero;
        panel.SetActive(true);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            panel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        panel.transform.localScale = Vector3.one;
    }
    public IEnumerator Close(GameObject panel, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = panel.transform.localScale;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            panel.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }
        panel.transform.localScale = Vector3.zero;
        panel.SetActive(false);
    }
    public IEnumerator TransitionClose(Animator anim)
    {
        if(anim != null) anim.SetTrigger("Transition");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Application.Quit();
    }
    public IEnumerator TransitionScene(string nameScene, Animator anim)
    {
        if (anim != null) anim.SetTrigger("Transition");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(nameScene);
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
