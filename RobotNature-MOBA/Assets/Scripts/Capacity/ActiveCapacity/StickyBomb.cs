using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class StickyBomb : ChampionActiveCapacity
{
    private Champion champion;

    private double timer;
    private Entity stickyBombGO;
    private GameObject explosionGO;
    private IActiveLifeable liveable;
    private StickyBombSO SOType;
    private StickyBombCollider collider;
    private Vector3 direction;

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
        direction = targetPositions[0] - caster.transform.position;
        direction.y = 0;
        stickyBombGO = PoolNetworkManager.Instance.PoolInstantiate(SOType.feedbackPrefab.GetComponent<Entity>(), caster.transform.position, Quaternion.identity);
        stickyBombGO.transform.parent = null;
        stickyBombGO.team = caster.team;
        collider = stickyBombGO.GetComponent<StickyBombCollider>();
        stickyBombGO.GetComponent<SphereCollider>().enabled = true;
        collider.ActivateParticleSystem(true);
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
        liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable != null)
        {
            // if (affectedEntity.name.Contains("Minion")) return;
            stickyBombGO.GetComponent<Rigidbody>().isKinematic = true;
            stickyBombGO.GetComponent<SphereCollider>().enabled = false;
            collider.ActivateParticleSystem(false);
            collider.transform.parent = affectedEntity.transform;
            stickyBombGO.transform.position = affectedEntity.transform.position + new Vector3(0, 2 * affectedEntity.transform.localScale.y + 1, 0);
            liveable.OnDecreaseCurrentHpCapacityFeedback += ExplodeBomb;
        }
        else
        {
            if (!affectedEntity.tag.Contains("Projectile")) return;
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

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }

    private void TimerBomb()
    {
        timer += 1;
        if (timer < SOType.durationBomb * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= TimerBomb;
        timer = 0;
        ExplodeBomb(SOType.radiusExplosion, SOType.percentageDamage);
    }

    private void ExplodeBomb(float amount, byte capacityIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var capacity = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex);
        
        if (capacity.name.Contains("AccurateShot"))
        {
            if (stickyBombGO.team == stickyBombGO.transform.parent.GetComponent<Entity>().team)
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
        Vector3 position;
        if (stickyBombGO.transform.parent == null) position = stickyBombGO.transform.position;
        else if (stickyBombGO.transform.parent.GetComponent<Entity>()) position = stickyBombGO.transform.parent.position;
        else position = stickyBombGO.transform.position;
        SOType.explosionGO.transform.localScale = new Vector3(radiusExplosion, radiusExplosion, radiusExplosion) / 3;
        PoolLocalManager.Instance.RequestPoolInstantiate(SOType.explosionGO, position, Quaternion.identity);
        // var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        var entities = Physics.OverlapSphere(position, radiusExplosion);
        foreach (var entity in entities)
        {
            var affectedEntity = entity.GetComponent<Entity>();
            if (affectedEntity == null || caster.team == affectedEntity.team) continue;
            var liveable = entity.GetComponent<IActiveLifeable>();
            if (liveable == null || !liveable.AttackAffected()) continue;
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
            // liveable.RequestDecreaseCurrentHpByCapacity(caster.GetComponent<Champion>().attackDamage * percentageDamage, capacityIndex);
            liveable.RequestDecreaseCurrentHp(caster.GetComponent<Champion>().attackDamage * percentageDamage);
        }
        collider.Disable();
    }
}