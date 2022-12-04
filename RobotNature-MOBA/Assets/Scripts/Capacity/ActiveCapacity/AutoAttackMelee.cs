using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttackMelee : ActiveCapacity
{
    private AffectCollider collider;
    private AutoAttackMeleeSO SOType;
    private double timer;
    private Vector3 lookDir;

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        SOType = (AutoAttackMeleeSO)SO;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        var zoneGO = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, casterTransform.position, Quaternion.LookRotation(lookDir));
        collider = zoneGO.GetComponent<AffectCollider>(); 
        collider.GetComponent<SphereCollider>().radius = SOType.maxRange;
        collider.capacitySender = this;
        collider.caster = caster;
        GameStateMachine.Instance.OnTick += DisableObject;
        return true;
    }

    public override void CollideEffect(Entity entityAffect)
    {
        IActiveLifeable lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable != null)
        {
            var angle = Vector3.Angle(lookDir.normalized, (entityAffect.transform.position - casterTransform.position).normalized);
            Debug.Log($"collide {entityAffect.gameObject.name} at {angle}°");
            if (lifeable.AttackAffected())
            { 
                if(angle>SOType.normalAmplitude)return; 
                if(angle <= SOType.perfectAmplitude)
                {
                    //Critic
                    Debug.Log("Critic");
                    entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, SOType.damageAmount*1.5f);
                    //lifeable.DecreaseCurrentHpRPC(SOType.damageAmount*1.5f);
                }
                else
                {
                    Debug.Log("Normal hit");
                    lifeable.DecreaseCurrentHpRPC(SOType.damageAmount);  
                }

            }
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }
    
    
    void DisableObject()
    {
        timer += 1;
        if(timer < 1.5f*GameStateMachine.Instance.tickRate)return;
        GameStateMachine.Instance.OnTick -= DisableObject;
        timer = 0;
        collider.gameObject.SetActive(false);
    }
}
