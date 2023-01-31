using Entities;
using Photon.Pun;
using UnityEngine;

public class BenchBehavior : MonoBehaviourPun
{
    public Collider benchCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>() == null) return;
        photonView.RPC("BreakBenchRPC", RpcTarget.All);
    }
    
    [PunRPC]
    private void BreakBenchRPC()
    {
        GetComponent<Animator>().SetTrigger("isBroken");
        benchCollider.enabled = false;
    }
}