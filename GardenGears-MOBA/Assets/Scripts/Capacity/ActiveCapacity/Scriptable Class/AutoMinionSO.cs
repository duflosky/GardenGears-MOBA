using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/ActiveCapacitySO/Auto Attack Minion", fileName = "AI Auto Attack for Minion")]
public class AutoMinionSO : ActiveCapacitySO
{
    public override Type AssociatedType()
    {
        return typeof(AutoMinion);
    }
}