using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Tower", fileName = "AI Auto Attack for ")]
public class AutoTowerSO : ActiveCapacitySO
{
    public float damage;
    
    public override Type AssociatedType()
    {
        return typeof(AutoTower);
    }
}
