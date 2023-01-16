using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AffectCollider : Entity
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public ActiveCapacity capacitySender;
    [HideInInspector] public float maxDistance;
    [HideInInspector] public Vector3 casterPos;
    [Header("=== AFFECT COLLIDER")]
    [SerializeField] private bool affectEntityOnly;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!CanDisable()) return;
        if (Vector3.Distance(casterPos, transform.position) > maxDistance) Disable();
    }

    protected virtual bool CanDisable()
    {
        return maxDistance != 0;
    }

    public void Launch(Vector3 moveVector)
    {
        rb.isKinematic = false;
        rb.velocity = moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<Entity>();
        if (entity && entity != caster)
        {
            capacitySender.CollideFeedbackEffect(entity);
            if (!PhotonNetwork.IsMasterClient) return;
            capacitySender.CollideEntityEffect(entity);
        }
        else if (!entity && !affectEntityOnly)
        {
            capacitySender.CollideObjectEffect(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if(PhotonNetwork.IsMasterClient)capacitySender.CollideExitEffect(other.gameObject);
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