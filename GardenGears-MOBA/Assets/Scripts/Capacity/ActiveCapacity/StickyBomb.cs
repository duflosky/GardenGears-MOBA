using Entities;
using Entities.Capacities;
using Entities.Minion;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBomb : ChampionActiveCapacity
{
    private Champion champion;

    private double timer;
    private GameObject stickyBombGO;
    private GameObject explosionGO;
    private IActiveLifeable liveable;
    private StickyBombSO SOType;
    private StickyBombCollider collider;
    private Vector3 direction;

    public override void OnStart()
    {
        SOType = (StickyBombSO)ActiveCapacitySO;
        champion = (Champion)Caster;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetsPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetsPositions)) return false;
        this.TargetPositions = targetsPositions;
        return true;
    }

    public override void CapacityPress()
    {
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
    }

    public override void CapacityEffect(Transform castTransform) { }

    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void CollideEntityEffect(Entity affectedEntity)
    {
        liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable != null)
        {
            // if (affectedEntity.name.Contains("Minion")) return;
            stickyBombGO.GetComponent<Rigidbody>().isKinematic = true;
            stickyBombGO.GetComponent<SphereCollider>().enabled = false;
            collider.transform.parent = affectedEntity.transform;
            stickyBombGO.transform.position = affectedEntity.transform.position + new Vector3(0, 2 * affectedEntity.transform.localScale.y + 1, 0);
            liveable.OnDecreaseCurrentHpCapacityFeedback += ExplodeBomb;
            OnAllyHit += ExplodeBomb;
        }
        else
        {
            if (!affectedEntity.tag.Contains("Projectile")) return;
            if (affectedEntity.GetComponent<AccurateShootCollider>() && affectedEntity.team != Caster.team)
            {
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionEnemy, SOType.percentageDamageEnemy);
            }
            else if (affectedEntity.GetComponent<AccurateShootCollider>() && affectedEntity.team == Caster.team)
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

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        champion.OnCastAnimationCastFeedback -= PlayFeedback;
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0] - Caster.transform.position;
        direction.y = 0;
        stickyBombGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, Caster.transform.position, Quaternion.identity);
        stickyBombGO.transform.parent = null;
        stickyBombGO.GetComponent<Entity>().team = Caster.team;
        collider = stickyBombGO.GetComponent<StickyBombCollider>();
        collider.isIgnite = false;
        collider.ActivateParticleSystem(true);
        collider.GetComponent<SphereCollider>().radius = SOType.radiusStick;
        collider.distance = SOType.maxRange;
        collider.capacity = this;
        collider.caster = Caster;
        collider.Launch(direction.normalized * SOType.speedBomb);
    }

    public void TimerBomb()
    {
        timer += 1;
        collider.timerImage.fillAmount = (float)((float)timer / (SOType.durationBomb * GameStateMachine.Instance.tickRate));
        if (timer < SOType.durationBomb * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
    }
    
    private void ExplodeBomb(byte capacityIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        var capacity = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex);
        
        if (capacity.name.Contains("AccurateShot"))
        {
            if (stickyBombGO.GetComponent<Entity>().team == stickyBombGO.transform.parent.GetComponent<Entity>().team)
            {
                OnAllyHit -= ExplodeBomb;
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionAlly, SOType.percentageDamageAlly);
            }
            else
            {
                OnAllyHit -= ExplodeBomb;
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionEnemy, SOType.percentageDamageEnemy);
            }
        }
        else
        {
            OnAllyHit -= ExplodeBomb;
            GameStateMachine.Instance.OnTick -= TimerBomb;
            timer = 0;
            ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
        }
    }

    private void ExplodeBomb(float amount, byte capacityIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        var capacity = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex);
        
        if (capacity.name.Contains("AccurateShot"))
        {
            if (stickyBombGO.GetComponent<Entity>().team == stickyBombGO.transform.parent.GetComponent<Entity>().team)
            {
                liveable.OnDecreaseCurrentHpCapacityFeedback -= ExplodeBomb;
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionAlly, SOType.percentageDamageAlly);
            }
            else
            {
                liveable.OnDecreaseCurrentHpCapacityFeedback -= ExplodeBomb;
                GameStateMachine.Instance.OnTick -= TimerBomb;
                timer = 0;
                ExplodeBomb(SOType.radiusExplosionEnemy, SOType.percentageDamageEnemy);
            }
        }
        else
        {
            liveable.OnDecreaseCurrentHpCapacityFeedback -= ExplodeBomb;
            GameStateMachine.Instance.OnTick -= TimerBomb;
            timer = 0;
            ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
        }
    }

    private void ExplodeBomb(float radiusExplosion, float percentageDamage)
    {
        if (liveable is not null) liveable.OnDecreaseCurrentHpCapacityFeedback -= ExplodeBomb;
        OnAllyHit -= ExplodeBomb;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        Vector3 position;
        if (stickyBombGO.transform.parent == null) position = stickyBombGO.transform.position;
        else if (stickyBombGO.transform.parent.GetComponent<Entity>()) position = stickyBombGO.transform.parent.position;
        else position = stickyBombGO.transform.position;
        if (radiusExplosion == SOType.radiusExplosion)
        {
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.explosionGO, position, Quaternion.identity);   
        }
        else if (radiusExplosion == SOType.radiusExplosionAlly)
        {
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.explosionAllyGO, position, Quaternion.identity);
        }
        else if (radiusExplosion == SOType.radiusExplosionEnemy)
        {
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.explosionEnemyGO, position, Quaternion.identity);
        }
        // var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        var entities = Physics.OverlapSphere(position, radiusExplosion);
        foreach (var entity in entities)
        {
            var affectedEntity = entity.GetComponent<Entity>();
            if (affectedEntity == null || Caster.team == affectedEntity.team) continue;
            var liveable = entity.GetComponent<IActiveLifeable>();
            if (liveable == null || !liveable.AttackAffected()) continue;
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
            // liveable.RequestDecreaseCurrentHpByCapacity(caster.GetComponent<Champion>().attackDamage * percentageDamage, capacityIndex);
            liveable.RequestDecreaseCurrentHp(Caster.GetComponent<Champion>().attackDamage * percentageDamage);
        }
        collider.Disable();
    }
}