using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UltimateRangeCollider : Entity
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public ActiveCapacity capacity;
    [HideInInspector] public float range;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!CanDisable()) return;
        if (Vector3.Distance(caster.transform.position, transform.position) > range)
        {
            SyncDisableRPC();
        }
    }

    protected virtual bool CanDisable()
    {
        return range != 0;
    }

    public void Launch(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.velocity = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        var entity = other.GetComponent<Entity>();
        if (entity && entity != caster)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            capacity.CollideEntityEffect(entity);
        }
        else if (!entity)
        {
            capacity.CollideObjectEffect(other.gameObject);
        }
    }
    
    public virtual void Disable()
    {
        photonView.RPC("SyncDisableRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void SyncDisableRPC()
    {
        gameObject.SetActive(false);
    }
}
