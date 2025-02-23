using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/Range Passif", fileName = "new RangePassifSO")]
public class RangePassiveSO : PassiveCapacitySO
{
    public int maxHeatStack = 10; 
    public int overheatStack = 8;

    public float stackDuration;
    
    public GameObject BurnFX;
    public float BurnDelay;

    public SpeedModifierPassiveSO actifSpeedBoost;
    public float actifDuration;
    
    public override Type AssociatedType()
    {
        return typeof(RangePassive);
    }
}
