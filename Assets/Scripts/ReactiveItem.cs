using UnityEngine;

public class ReactiveItem : MonoBehaviour
{
    public float timeToReactive = 10f;
    float currentTime;
    Item item;

    private void Start()
    {
        item = GetComponent<Item>();
        currentTime = timeToReactive;
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            return;
        }
        if (item.enabled == false)
        {
            currentTime -= Time.deltaTime;
        }
        if(currentTime <= 0)
        {
            item.enabled = true;
            item.model.SetActive(true);
            currentTime = timeToReactive;
        }
    }
}
