using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBombCollider : Entity
{
    [HideInInspector] public bool isIgnite;
    [HideInInspector] public Entity caster;
    [HideInInspector] public float distance;
    [HideInInspector] public StickyBomb capacity;
    [SerializeField] private GameObject[] particles;

    private Rigidbody rb;
    private Vector3 lastPositionCaster;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!CanDisable()) return;
        if (!(Vector3.Distance(lastPositionCaster, transform.position) > distance) || isIgnite) return;
        ActivateParticleSystem(false);
        rb.isKinematic = true;
        GameStateMachine.Instance.OnTick += capacity.TimerBomb;
        GetComponent<SphereCollider>().enabled = true;
        isIgnite = true;
    }

    protected virtual bool CanDisable()
    {
        return distance != 0;
    }

    public void Launch(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.velocity = direction;
        lastPositionCaster = caster.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        var affectedEntity = other.GetComponent<Entity>();
        if (!affectedEntity) return;
        if (!PhotonNetwork.IsMasterClient) return; 
        capacity.CollideEntityEffect(affectedEntity);
    }

    public void Disable()
    {
        GetComponent<SphereCollider>().enabled = false;
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