using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

public class RangePassifSO : PassiveCapacitySO
{
    [SerializeField] private int maxHeatStack = 10; 
    [SerializeField] private int overheatStack = 8; 

    public override Type AssociatedType()
    {
        return typeof(RangePassif);
    }
}
