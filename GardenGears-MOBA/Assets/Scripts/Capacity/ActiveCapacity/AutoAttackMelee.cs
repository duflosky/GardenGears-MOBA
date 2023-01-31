using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttackMelee : ChampionActiveCapacity
{
    private GameObject feedbackObject;
    public AutoAttackMeleeSO SOType;
    private double timer;
    private Vector3 lookDir;


    public override void OnStart()
    {
        base.OnStart();
        SOType = (AutoAttackMeleeSO)SO;
    }
    
    public override void CapacityPress()
    {
        base.CapacityPress();
        champion.OnCastAnimationFeedback += AnimationFeedback;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        //champion.GetPassiveCapacity(SOType.attackSlowSO).OnRemoved();
        lookDir = casterTransform.GetChild(0).forward;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, casterTransform.position, Quaternion.LookRotation(lookDir));
        AffectCollider collider = feedbackObject.GetComponent<AffectCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.maxRange;
        collider.capacitySender = this;
        collider.caster = caster;
        GameStateMachine.Instance.OnTick += DisableObject;
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (caster.team == entity.team)
        {
            AllyHit(indexOfSOInCollection);
            return;
        }
        var liveable = entity.GetComponent<IActiveLifeable>();
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        if (liveable != null)
        {
            var angle = Vector3.Angle(lookDir.normalized, (entity.transform.position - casterTransform.position).normalized);
            //Debug.Log($"collide {entityAffect.gameObject.name} at {angle}°");
            if (liveable.AttackAffected())
            { 
                if(angle>SOType.normalAmplitude)return;
                var hitPos = entity.transform.position + (entity.transform.position - casterTransform.position).normalized*.5f;
                if(angle <= SOType.perfectAmplitude)
                {
                    //Critic
                    //Debug.Log("Critic");
                    PoolLocalManager.Instance.RequestPoolInstantiate(SOType.criticalHitPrefab, hitPos, Quaternion.identity, null, 1f);
                    entity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamageCrit, capacityIndex);
                }
                else
                {
                    //Debug.Log("Normal hit");
                    PoolLocalManager.Instance.RequestPoolInstantiate(SOType.hitPrefab, hitPos, Quaternion.identity, null, 1f);
                    liveable.DecreaseCurrentHpByCapacityRPC(caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage, capacityIndex);  
                }

            }
        }
    }

    public override void CapacityEndAnimation()
    {
        champion.GetPassiveCapacity(SOType.attackSlowSO).OnRemoved();
        champion.OnCastAnimationEnd -= CapacityEndAnimation; 

    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        lookDir = -casterTransform.GetChild(0).forward;
        lookDir.y = 0;
        PoolLocalManager.Instance.PoolInstantiate(SOType.fxPrefab, casterTransform.position, Quaternion.LookRotation(lookDir));
    }

    public override void AnimationFeedback()
    {
        lookDir = casterTransform.GetChild(0).forward;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.fxPrefab, casterTransform.position, Quaternion.LookRotation(-lookDir), casterTransform); 
        var col = feedbackObject.GetComponent<Collider>(); 
        if (col) col.enabled = false; 
        GameStateMachine.Instance.OnTick += DisableObject;
        champion.OnCastAnimationFeedback -= AnimationFeedback;
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
