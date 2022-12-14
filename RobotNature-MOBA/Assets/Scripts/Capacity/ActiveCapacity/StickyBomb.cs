using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class StickyBomb : ActiveCapacity
{
    private Champion champion;
    
    public StickyBombSO SOType;
    private GameObject stickyBombGO;
    private GameObject explosionGO;
    private double timer;
    private Vector3 lookDir;

    public override void OnStart()
    {
        SOType = (StickyBombSO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    public override void CapacityPress()
    {
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation; 
        // TODO: Add animation event
        // champion.canRotate = false;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
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
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        // champion.canRotate = true;
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        if (entityAffect.name.Contains("Minion")) return;
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
        timer = 0;
        ExplodeBomb();
    }

    private void ExplodeBomb()
    {
        Collider[] entities = Physics.OverlapSphere(stickyBombGO.transform.position, SOType.radiusExplosion);
        foreach (Collider entity in entities)
        {
            Entity entityAffect = entity.GetComponent<Entity>();
            if (entityAffect == null) continue;
            if (caster.team == entityAffect.team) continue; 
            IActiveLifeable lifeable = entity.GetComponent<IActiveLifeable>();
            if (lifeable == null) continue;
            if (!lifeable.AttackAffected()) continue;
            lifeable.RequestDecreaseCurrentHp(caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage);
        }
        explosionGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, stickyBombGO.transform.position, Quaternion.identity, stickyBombGO.transform);
        explosionGO.GetComponent<ParticleSystem>().Play();
        GameStateMachine.Instance.OnTick += DestroyExplosion;
    }
    
    private void DestroyExplosion()
    {
        if(!explosionGO.GetComponent<ParticleSystem>().isStopped) return;
        GameStateMachine.Instance.OnTick -= DestroyExplosion;
        if(stickyBombGO) stickyBombGO.SetActive(false);
    }
}