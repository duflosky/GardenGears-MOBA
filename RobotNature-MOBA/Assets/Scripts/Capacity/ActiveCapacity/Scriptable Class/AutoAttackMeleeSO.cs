using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto-Attack Melee", fileName = "new AutoAttackMeleeSO")]
public class AutoAttackMeleeSO : ActiveCapacitySO
{
    public GameObject damageZone;
    
    public GameObject fxPrefab;
    public GameObject hitPrefab;
    public GameObject criticalHitPrefab;
    
    public float percentageDamage;
    public float percentageDamageCrit;
    [Space]
    public float normalAmplitude;
    public float perfectAmplitude;
    [Space] public SpeedModifierPassiveSO attackSlowSO;

    public override Type AssociatedType()
    {
        return typeof(AutoAttackMelee);
    }
}
