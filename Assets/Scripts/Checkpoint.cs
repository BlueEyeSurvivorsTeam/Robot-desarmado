using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void SetPosCheckpoint()
    {
        GameManager.Instance.SetPos(gameObject);
    }
}
