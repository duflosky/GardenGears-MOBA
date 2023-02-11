using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Ultimate Range", fileName = "new UltimateRangeSO")]
public class UltimateRangeSO : ChampionActiveCapacitySO
{
    [Tooltip("Radius of the collider")] public float colliderRadius;
    [Tooltip("Radius of the explosion")] public float explosionRadius;
    [Tooltip("Speed of the projectile")] public float speed;
    public float damagePercentage;
    public StunPassiveSO PassiveAfterHit;
    public float StunTimer;
    
    public override Type AssociatedType()
    {
        return typeof(UltimateRange);
    }
}
