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
    private Champion champion;


    public override void OnStart()
    {
        SOType = (AutoAttackMeleeSO)SO;
        champion = (Champion)caster;
        casterTransform = caster.transform;
    }
    
    public override void CapacityPress()
    {
        champion.GetPassiveCapacity(SOType.attackSlowSO).OnAdded();
        champion.OnCastAnimationCast += CapacityEffect;
        
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.GetPassiveCapacity(SOType.attackSlowSO).OnRemoved();
        lookDir = targetPositions[0] - casterTransform.position;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, casterTransform.position, Quaternion.LookRotation(lookDir));
        AffectCollider collider = feedbackObject.GetComponent<AffectCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.maxRange;
        collider.capacitySender = this;
        collider.caster = caster;
        GameStateMachine.Instance.OnTick += DisableObject;
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        if (caster.team == entityAffect.team) return;
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
                    entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamageCrit);
                }
                else
                {
                    Debug.Log("Normal hit");
                    lifeable.DecreaseCurrentHpRPC(caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage);  
                }

            }
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    { 
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.fxPrefab, casterTransform.position, Quaternion.LookRotation(-lookDir), casterTransform); 
        var col = feedbackObject.GetComponent<Collider>(); 
        if (col) col.enabled = false; 
        GameStateMachine.Instance.OnTick += DisableObject;
    }
    
    
    private void DisableObject()
    {
        timer += 1;
        if(timer < 1.5f*GameStateMachine.Instance.tickRate)return;
        GameStateMachine.Instance.OnTick -= DisableObject;
        timer = 0;
        if(feedbackObject)feedbackObject.SetActive(false);
    }
}
