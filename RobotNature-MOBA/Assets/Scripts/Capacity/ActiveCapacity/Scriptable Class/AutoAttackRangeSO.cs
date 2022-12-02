using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto-Attack Range", fileName = "new AutoAttackRangeSO")]
public class AutoAttackRangeSO : ActiveCapacitySO
{
    public GameObject bulletPrefab;

    public float bulletSpeed;
    public float bulletDamage;
    
    
    public override Type AssociatedType()
    {
        return typeof(AutoAttackRange);
    }
}