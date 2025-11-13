using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    public string tagEvent;
    public bool useEnter;
    public UnityEvent enterEvent;
    public bool useExit;
    public UnityEvent exitEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (useEnter)
        {
            if(other.CompareTag(tagEvent))
            {
                enterEvent.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(useExit)
        {
            if(other.CompareTag(tagEvent))
            {
                exitEvent.Invoke();
            }
        }
    }
}
