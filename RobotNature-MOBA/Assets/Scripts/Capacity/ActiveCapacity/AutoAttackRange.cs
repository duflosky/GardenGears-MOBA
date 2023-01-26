using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AutoAttackRange : ChampionActiveCapacity
{
    private AutoAttackRangeSO SOType;
    private Vector3 lookDir;
    private GameObject bullet;
    private AffectCollider collider;
    private GameObject shotGizmo;

    public override void OnStart()
    {
        base.OnStart();
        SOType = (AutoAttackRangeSO)SO;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!onCooldown)
        {
            InitiateCooldown();
            this.targetsEntityIndexes = targetsEntityIndexes;

            this.targetPositions = targetPositions;
            CapacityPress();
            return true;
        }
        else return false;
    }
    
    public override void CapacityShotEffect(Transform transform)
    {
        champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        var direction = casterTransform.GetChild(0).forward;
        PoolLocalManager.Instance.RequestPoolInstantiate(SOType.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.canRotate = false;
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.overheatSO)).OnAdded();
        lookDir = casterTransform.GetChild(0).forward;
        float rotateAngle = champion.isOverheat ? Random.Range(-(SOType.sprayAngle / 2), (SOType.sprayAngle / 2)) : 0f;
        Debug.Log($"RotateAngle: {rotateAngle}");

        var bulletPref = champion.isOverheat ? SOType.overheatBulletPrefab : SOType.bulletPrefab;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(bulletPref.GetComponent<Entity>(), casterTransform.position, casterTransform.GetChild(0).rotation).gameObject;
        collider = bullet.GetComponent<AffectCollider>();
        collider.caster = caster;
        collider.casterPos = casterTransform.position;
        if(champion.isOverheat)collider.transform.Rotate(new Vector3(0,rotateAngle,0));
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        //collider.Launch(lookDir.normalized*SOType.bulletSpeed);
        collider.Launch(collider.transform.forward*SOType.bulletSpeed);
    }

    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnRemoved();
        champion.canRotate = true;
    }

    public override void CollideEntityEffect(Entity affectedEntity)
    {
        if (caster.team == affectedEntity.team)
        {
            AllyHit(indexOfSOInCollection);
            return;
        }
        var lifeable = affectedEntity.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        affectedEntity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage, capacityIndex);
        PoolLocalManager.Instance.RequestPoolInstantiate(champion.isOverheat ? SOType.hitOverheatPrefab : SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
        collider.Disable();
    }

    public override void CollideObjectEffect(GameObject obj)
    { 
        collider.Disable();
    }

}