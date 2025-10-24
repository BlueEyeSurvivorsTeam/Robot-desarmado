using UnityEngine;

public class PartDetector : MonoBehaviour
{
    bool partClose = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<MoveController>() && other != this.GetComponentInParent<MoveController>().gameObject)
        {
            partClose = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<MoveController>() && other != this.GetComponentInParent<MoveController>().gameObject)
        {
            partClose = false;
        }
    }
    public bool IsPartClose()
    {
        return partClose;
    }
}
