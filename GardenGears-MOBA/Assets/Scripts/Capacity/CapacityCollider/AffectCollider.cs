using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class AffectCollider : MonoBehaviour
{
    [HideInInspector] public Entity Caster;
    [HideInInspector] public ActiveCapacity Capacity;
    [HideInInspector] public float MaxRange;
    [HideInInspector] public Vector3 CasterPosition;
    
    [SerializeField] private bool _affectEntityOnly;
    [SerializeField] private Rigidbody _rb;

    private void Update()
    {
        if (!CanDisable()) return;
        if (Vector3.Distance(CasterPosition, transform.position) > MaxRange) Disable();
    }

    protected virtual bool CanDisable()
    {
        return MaxRange != 0;
    }

    public void Launch(Vector3 moveVector)
    {
        _rb.isKinematic = false;
        _rb.velocity = moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<Entity>();
        if (entity && entity != Caster)
        {
            Capacity.CollideEntityEffect(entity);
        }
        else if (!entity && !_affectEntityOnly)
        {
            Capacity.CollideObjectEffect(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if (PhotonNetwork.IsMasterClient) Capacity.CollideExitEffect(other.gameObject);
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}