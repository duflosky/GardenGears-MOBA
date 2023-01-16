using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Sticky Bomb", fileName = "new StickyBombSO")]
public class StickyBombSO : ActiveCapacitySO
{
    public float percentageDamage;
    public float speedBomb;
    public float durationBomb;
    public float radiusStick;
    public float radiusExplosion;
    public GameObject explosionGO;

    public override Type AssociatedType()
    {
        return typeof(StickyBomb);
    }
}
