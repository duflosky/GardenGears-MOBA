using System;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto-Attack Range", fileName = "new AutoAttackRangeSO")]
public class AutoAttackRangeSO : ChampionActiveCapacitySO
{
    [FormerlySerializedAs("overheatBulletPrefab")] [Space]
    public GameObject OverheatProjectile;

    [FormerlySerializedAs("HitOverheat")] [FormerlySerializedAs("hitOverheatPrefab")]
    public GameObject OverheatHit;

    [FormerlySerializedAs("bulletSpeed")] public float Speed;

    [FormerlySerializedAs("percentageDamage")]
    public float PercentDamage;

    [FormerlySerializedAs("overheatSO")] [Space]
    public PassiveCapacitySO OverheatPassiveCapacitySO;

    [FormerlySerializedAs("sprayAngle")] public float OverheatAngle = 60;

    public override Type AssociatedType()
    {
        return typeof(AutoAttackRange);
    }
}