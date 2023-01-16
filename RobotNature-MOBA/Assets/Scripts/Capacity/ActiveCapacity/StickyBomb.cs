using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBomb : ActiveCapacity
{
    private Champion champion;

    private StickyBombSO SOType;
    private GameObject stickyBombGO;
    private GameObject explosionGO;
    private double timer;
    private Vector3 direction;
    private PhotonView photonView;

    public override void OnStart()
    {
        SOType = (StickyBombSO)SO;
        champion = (Champion)caster;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    public override void CapacityPress()
    {
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0]- caster.transform.position;
        direction.y = 0;
        stickyBombGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, caster.transform.position, Quaternion.LookRotation(direction));
        stickyBombGO.GetComponent<MeshRenderer>().enabled = true;
        var collider = stickyBombGO.GetComponent<StickyBombCollider>(); 
        collider.GetComponent<SphereCollider>().radius = SOType.radiusStick;
        collider.distance = SOType.maxRange;
        collider.capacity = this;
        collider.caster = caster;
        collider.Launch(direction.normalized * SOType.speedBomb);
        GameStateMachine.Instance.OnTick += TimerBomb;
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void CollideEntityEffect(Entity affectedEntity)
    {
        var liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable != null)
        {
            if (affectedEntity.name.Contains("Minion")) return;
            stickyBombGO.GetComponent<Rigidbody>().isKinematic = true;
            stickyBombGO.transform.parent = affectedEntity.transform;
            stickyBombGO.transform.position += new Vector3(0, affectedEntity.transform.localScale.y, 0) + Vector3.up * 2;
            stickyBombGO.GetComponent<ParticleSystem>().Stop();
            foreach (var componentParticleSystem in stickyBombGO.GetComponentsInChildren<ParticleSystem>())
            {
                componentParticleSystem.Stop();
            }
        }
        else
        {
            if (affectedEntity.GetComponent<AccurateShootCollider>() && affectedEntity.team != caster.team)
            {
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionEnemy, SOType.percentageDamageEnemy);
            }
            else if (affectedEntity.GetComponent<AccurateShootCollider>() && affectedEntity.team == caster.team)
            {
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionAlly, SOType.percentageDamageAlly);
            }
            else
            {
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
            }
        }
    }
    
    public override void CollideFeedbackEffect(Entity affectedEntity)
    {
        var liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable != null)
        {
            if (affectedEntity.name.Contains("Minion")) return;
            stickyBombGO.GetComponent<Rigidbody>().isKinematic = true;
            stickyBombGO.transform.parent = affectedEntity.transform;
            stickyBombGO.transform.position += new Vector3(0, affectedEntity.transform.localScale.y, 0) + Vector3.up * 2;
            stickyBombGO.GetComponent<ParticleSystem>().Stop();
            foreach (var componentParticleSystem in stickyBombGO.GetComponentsInChildren<ParticleSystem>())
            {
                componentParticleSystem.Stop();
            }
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        direction = targetPositions[0] - caster.transform.position;
        direction.y = 0;
        stickyBombGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, caster.transform.position, Quaternion.LookRotation(direction));
        stickyBombGO.GetComponent<MeshRenderer>().enabled = true;
        var collider = stickyBombGO.GetComponent<AffectCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.radiusStick;
        collider.maxDistance = SOType.maxRange;
        collider.casterPos = caster.transform.position;
        collider.capacitySender = this;
        collider.caster = caster;
        collider.Launch(direction.normalized * SOType.speedBomb);
        GameStateMachine.Instance.OnTick += TimerBomb;
    }

    private void TimerBomb()
    {
        timer += 1;
        if(timer < SOType.durationBomb * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
    }

    private void ExplodeBomb(float radiusExplosion, float percentageDamage)
    {
        explosionGO = PoolLocalManager.Instance.PoolInstantiate(SOType.explosionGO, stickyBombGO.transform.position, Quaternion.identity);
        var entities = Physics.OverlapSphere(stickyBombGO.transform.position, radiusExplosion);
        foreach (var entity in entities)
        {
            var affectedEntity = entity.GetComponent<Entity>();
            if (affectedEntity == null || caster.team == affectedEntity.team) continue;
            var liveable = entity.GetComponent<IActiveLifeable>();
            if (liveable == null || !liveable.AttackAffected()) continue;
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
            liveable.RequestDecreaseCurrentHp(caster.GetComponent<Champion>().attackDamage * percentageDamage);
        }
        stickyBombGO.GetComponent<MeshRenderer>().enabled = false;
        GameStateMachine.Instance.OnTick += DestroyExplosion;
    }

    private void DestroyExplosion()
    {
        if (!explosionGO.GetComponent<ParticleSystem>().isStopped) return;
        if (stickyBombGO) stickyBombGO.gameObject.SetActive(false);
        GameStateMachine.Instance.OnTick -= DestroyExplosion;
    }
}