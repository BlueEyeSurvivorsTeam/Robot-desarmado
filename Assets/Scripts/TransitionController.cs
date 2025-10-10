using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
public class TransitionController : MonoBehaviour
{
    public float duration;
    public Animator anim;
    public IEnumerator Open(GameObject panel)
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

    public IEnumerator Close(GameObject panel)
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
    private IEnumerator TransitionClose()
    {
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Application.Quit();
    }
    private IEnumerator TransitionScene(string nameScene)
    {
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(nameScene);
    }
    public void LoadScene(string nameScene)
    {
        StartCoroutine(TransitionScene(nameScene));
    }
    public void CloseGame()
    {
        StartCoroutine(TransitionClose());
    }
}
