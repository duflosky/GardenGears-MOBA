using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto-Attack Melee", fileName = "new AutoAttackMeleeSO")]
public class AutoAttackMeleeSO : ActiveCapacitySO
{
    public GameObject damageZone;
    public float damageAmount = 1;

    
    public float normalAmplitude;
    public float perfectAmplitude;

    public override Type AssociatedType()
    {
        return typeof(AutoAttackMelee);
    }
}
