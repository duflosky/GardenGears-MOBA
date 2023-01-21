using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Sticky Bomb", fileName = "new StickyBombSO")]
public class StickyBombSO : ChampionActiveCapacitySO
{
    public float percentageDamage;
    public float percentageDamageAlly;
    public float percentageDamageEnemy;
    public float speedBomb;
    public float durationBomb;
    public float radiusStick;
    public float radiusExplosion;
    public float radiusExplosionAlly;
    public float radiusExplosionEnemy;
    public GameObject explosionGO;
    public GameObject explosionAllyGO;
    public GameObject explosionEnemyGO;

    public override Type AssociatedType()
    {
        return typeof(StickyBomb);
    }
}
