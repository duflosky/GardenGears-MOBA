using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/Range Passif", fileName = "new RangePassifSO")]
public class RangePassifSO : PassiveCapacitySO
{
    public int maxHeatStack = 10; 
    public int overheatStack = 8;

    public float stackDuration;
    
    public GameObject BurnFX;
    public float BurnDelay;

    public override Type AssociatedType()
    {
        return typeof(RangePassif);
    }
}
