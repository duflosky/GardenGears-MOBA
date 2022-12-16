using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/AccurateShotSO", fileName = "new AccurateShotSO")]
public class AccurateShotSO : ActiveCapacitySO
{
    public GameObject bulletPrefab;

    public float bulletSpeed;
    public float percentageDamage;
    public SpeedModifierPassiveSO SlowEffectSO;
    [Space]
    public SpeedModifierPassiveSO attackAnimationSlowSO;

    public override Type AssociatedType()
    {
        return typeof(AccurateShot);
    }
}
