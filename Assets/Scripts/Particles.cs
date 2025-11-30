using UnityEngine;

public class Particles : MonoBehaviour
{
    public void InvokeParticle(GameObject prefab)
    {
        GameObject particle = Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(particle, 3f);
    }
}
