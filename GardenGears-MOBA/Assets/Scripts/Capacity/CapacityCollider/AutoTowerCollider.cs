using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoTowerCollider : Entity
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public Entity target;
    [HideInInspector] public ActiveCapacity capacitySender;

    [SerializeField] private double desiredDuration = 1f;
    private double elapsedTime;
    private double percentageComplete;

    private void OnEnable()
    {
        GameStateMachine.Instance.OnTick += HandlePosition;
    }
    
    private void OnDisable()
    {
        elapsedTime = 0;
        percentageComplete = 0;
        GameStateMachine.Instance.OnTick -= HandlePosition;
    }

    private void HandlePosition()
    {
        if (!CanDisable()) return;
        if (!target.GetComponent<IDeadable>().IsAlive()) Disable();
        elapsedTime += Time.deltaTime;
        percentageComplete = elapsedTime / desiredDuration * GameStateMachine.Instance.tickRate;
        transform.position = Vector3.Lerp(caster.transform.position, target.transform.position, (float)percentageComplete);
    }

    protected virtual bool CanDisable()
    {
        if (!target) return false;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (!entity || entity == caster || entity != target) return;
        capacitySender.CollideFeedbackEffect(entity);
        if (!PhotonNetwork.IsMasterClient) return;
        capacitySender.CollideEntityEffect(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        capacitySender.CollideExitEffect(other.gameObject);
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
