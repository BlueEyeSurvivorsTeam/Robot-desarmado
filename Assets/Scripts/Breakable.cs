using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public UnityEvent breakEvent;
    
    public void Break()
    {
        breakEvent.Invoke();
    }
}
