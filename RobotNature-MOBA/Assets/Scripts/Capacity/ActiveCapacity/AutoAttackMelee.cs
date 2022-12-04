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
    private GameObject feedbackObject;
    public AutoAttackMeleeSO SOType;
    private double timer;
    private Vector3 lookDir;


    public override void OnStart()
    {
        SOType = (AutoAttackMeleeSO)SO;
        casterTransform = caster.transform;
    }

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        //SOType = (AutoAttackMeleeSO)SO;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, casterTransform.position, Quaternion.LookRotation(lookDir));
        AffectCollider collider = feedbackObject.GetComponent<AffectCollider>(); 
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
        Debug.Log("Play FeedBack");
        if(PhotonNetwork.IsMasterClient) return;
        Debug.Log("Play FeedBack as Client");
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, casterTransform.position, Quaternion.LookRotation(lookDir));
       var col = feedbackObject.GetComponent<Collider>();
       if (col) col.enabled = false;
       GameStateMachine.Instance.OnTick += DisableObject;
    }
    
    
    void DisableObject()
    {
        timer += 1;
        if(timer < 1.5f*GameStateMachine.Instance.tickRate)return;
        GameStateMachine.Instance.OnTick -= DisableObject;
        timer = 0;
        feedbackObject.SetActive(false);
    }
}
