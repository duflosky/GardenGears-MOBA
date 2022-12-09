using System.Linq;
using Entities;
using Entities.Capacities;
using GameStates;
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
        collider.GetComponent<SphereCollider>().radius = SOType.radiusStick;
        collider.maxDistance = SOType.maxRange;
        collider.casterPos = caster.transform.position;
        collider.capacitySender = this;
        collider.caster = caster;
        collider.Launch(shootDir.normalized * SOType.speedBomb);
        GameStateMachine.Instance.OnTick += TimerBomb;
        return true;
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        IActiveLifeable lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable != null)
        {
            if(!lifeable.AttackAffected()) return;
            stickyBombGO.GetComponent<Rigidbody>().velocity = Vector3.zero;
            stickyBombGO.transform.parent = entityAffect.transform;
            stickyBombGO.transform.localPosition += new Vector3(0, entityAffect.transform.localScale.y, 0) + Vector3.up;
        }
    }

    public override void CollideObjectEffect(GameObject obj)
    {
        stickyBombGO.transform.position = obj.transform.position;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }

    private void TimerBomb()
    {
        timer += 1;
        if(timer < SOType.durationBomb * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        ExplodeBomb();
        timer = 0;
    }

    private void ExplodeBomb()
    {
        Collider[] entities = Physics.OverlapSphere(stickyBombGO.transform.position, SOType.radiusExplosion);
        foreach (Collider entity in entities)
        {
            IActiveLifeable lifeable = entity.GetComponent<IActiveLifeable>();
            if (lifeable != null)
            {
                if (lifeable.AttackAffected())
                {
                    lifeable.RequestDecreaseCurrentHp(caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage);
                }
            }
        }
        if(stickyBombGO) stickyBombGO.SetActive(false);
    }
}