using System;
using Entities.Capacities;
using UnityEngine;

[CreateAssetMenu(menuName = "Capacity/PassiveCapacitySO/StunPassive", fileName = "new StunPassiveSO")]
public class StunPassiveSO : PassiveCapacitySO
{
    [SerializeField] private GameObject _stunGameObject;
    
    public override Type AssociatedType()
    {
        return typeof(StunPassive);
    }
}