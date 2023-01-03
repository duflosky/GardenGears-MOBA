using Entities;
using UnityEngine;

public class BenchBehavior : MonoBehaviour
{
    public Collider benchCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>() == null) return;
        GetComponent<Animator>().SetTrigger("isBroken");
        benchCollider.enabled = false;
    }
}