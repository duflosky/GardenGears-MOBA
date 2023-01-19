using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBombCollider : Entity
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public StickyBomb capacity;
    [HideInInspector] public float distance;
    [SerializeField] private GameObject[] particles;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!CanDisable()) return;
        if (!(Vector3.Distance(caster.transform.position, transform.position) > distance)) return;
        rb.isKinematic = true;
        ActivateParticleSystem(false);
        GameStateMachine.Instance.OnTick += capacity.TimerBomb;
        GetComponent<SphereCollider>().enabled = true;
    }

    protected virtual bool CanDisable()
    {
        return distance != 0;
    }

    public void Launch(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.velocity = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        var affectedEntity = other.GetComponent<Entity>();
        if (!affectedEntity || affectedEntity == caster) return;
        if (PhotonNetwork.IsMasterClient) capacity.CollideEntityEffect(affectedEntity);
    }

    public void Disable()
    {
        photonView.RPC("SyncDisableRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void SyncDisableRPC()
    {
        gameObject.SetActive(false);
    }
    
    public void ActivateParticleSystem(bool value)
    {
        photonView.RPC("SyncActivateParticleSystemRPC", RpcTarget.All, value);
    }
    
    [PunRPC]
    private void SyncActivateParticleSystemRPC(bool value)
    {
        foreach (var particle in particles)
        {
            particle.SetActive(value);
        }
    }
}