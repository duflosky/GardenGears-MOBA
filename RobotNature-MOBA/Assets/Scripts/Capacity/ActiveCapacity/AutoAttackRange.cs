using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AutoAttackRange : ActiveCapacity
{
    private Champion champion;
    
    private AutoAttackRangeSO SOType;
    private Vector3 lookDir;
    private GameObject bullet;
    private AffectCollider collider;
    private GameObject shotGizmo;

    public override void OnStart()
    {
        SOType = (AutoAttackRangeSO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
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

    public override void CapacityPress()
    {
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnAdded();
        DisplayGizmos(true);
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation; 
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.canRotate = false;
        DisplayGizmos(false);
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.overheatSO)).OnAdded();
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        if (champion.isOverheat)
        {
            var rdm = Random.Range(-(SOType.sprayAngle / 2), (SOType.sprayAngle / 2));
            lookDir += new Vector3(Mathf.Cos(rdm), 0, Mathf.Sin(rdm)).normalized;
        }

        var bulletPref = champion.isOverheat ? SOType.overheatBulletPrefab : SOType.bulletPrefab;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(bulletPref.GetComponent<Entity>(), casterTransform.position, Quaternion.LookRotation(lookDir)).gameObject;
        collider = bullet.GetComponent<AffectCollider>();
        collider.caster = caster;
        collider.casterPos = casterTransform.position;
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        collider.Launch(lookDir.normalized*SOType.bulletSpeed);
    }

    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnRemoved();
        champion.canRotate = true;
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (caster.team == entity.team) return;
        var lifeable = entity.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        entity.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage);
        collider.Disable();
    }

    public override void CollideFeedbackEffect(Entity affectedEntity)
    {
        if (caster.team == affectedEntity.team) return;
        PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
    }

    public override void CollideObjectEffect(GameObject obj)
    { 
        collider.Disable();
    }

    public override void DisplayGizmos(bool state)
    {

        if (state)
        {
            if (!shotGizmo)
            {
                shotGizmo = Object.Instantiate(SOType.shotGizmoPrefab, casterTransform.position, Quaternion.identity, casterTransform);
                var rect = shotGizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>(); //Désolé pour les yeux :3
                rect.localPosition =(new Vector3(0,0,SOType.maxRange/2));
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, SOType.maxRange);
            }
            else shotGizmo.SetActive(true);
            champion.CastUpdate += UpdateGizmos;
        }
        else
        {
            champion.CastUpdate -= UpdateGizmos;
            shotGizmo.SetActive(false);
        }
    }

    public override void UpdateGizmos()
    {
        shotGizmo.transform.LookAt(targetPositions[0]);
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }
}