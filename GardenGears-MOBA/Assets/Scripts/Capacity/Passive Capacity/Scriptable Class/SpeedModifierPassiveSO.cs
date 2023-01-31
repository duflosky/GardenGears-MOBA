using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/SpeedBoostPassive", fileName = "new SpeedBoostPassiveSO")]
public class SpeedModifierPassiveSO : PassiveCapacitySO
{
    public override Type AssociatedType()
    {
        return typeof(SpeedModifierPassive);
    }

    [Header("=== SpeedBoostPassiveSO")] 
    public bool isBuff = true;
    public bool isRatio;
    public float speedBonus=5;
    public int duration;
}
