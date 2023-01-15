using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Ultimate Range", fileName = "new UltimateRangeSO")]
public class UltimateRangeSO : ActiveCapacitySO
{
    [Tooltip("Width of the inner ray")] public float radius;
    [Tooltip("Speed of the projectile")] public float speed;
    public float damagePercentage;
    
    public override Type AssociatedType()
    {
        return typeof(UltimateRange);
    }
}
