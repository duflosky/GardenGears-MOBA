using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBomb : ActiveCapacity
{
    public StickyBombSO SOType;
    private GameObject stickyBombGO;
    private double timer;
    private Vector3 lookDir;

    public override void OnStart()
    {
        SOType = (StickyBombSO)SO;
        casterTransform = caster.transform;
    }
    
    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        var shootDir = lookDir;
        stickyBombGO = PoolLocalManager.Instance.PoolInstantiate(SOType.stickyBombZone, casterTransform.position, Quaternion.LookRotation(lookDir));
        AffectCollider collider = stickyBombGO.GetComponent<AffectCollider>(); 
        collider.GetComponent<SphereCollider>().radius = SOType.maxRange;
        collider.capacitySender = this;
        collider.caster = caster;
        GameStateMachine.Instance.OnTick += DisableObject;
        return true;
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        IActiveLifeable lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable != null)
        {
            // TODO: Stick to the entity and explode after a delay
            if (lifeable.AttackAffected())
            { 
                stickyBombGO.transform.parent = entityAffect.transform;
                stickyBombGO.transform.localPosition += Vector3.up;
            }
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }
    
    void DisableObject()
    {
        timer += 1;
        if(timer < 1.5f * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= DisableObject;
        timer = 0;
        if(stickyBombGO) stickyBombGO.SetActive(false);
    }
}
