using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Sticky Bomb", fileName = "new StickyBombSO")]
public class StickyBombSO : ActiveCapacitySO
{
    public GameObject stickyBombZone;
    public float percentageDamage;
    public float speedBomb;
    public float durationBomb;
    public float radiusStick;

    public override Type AssociatedType()
    {
        return typeof(StickyBomb);
    }
}
