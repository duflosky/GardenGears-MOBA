using System;
using Entities.Capacities;
using UnityEngine;

public class StunPassiveSO : PassiveCapacitySO
{
    [SerializeField] private GameObject _stunGameObject;
    
    public override Type AssociatedType()
    {
        return typeof(StunPassive);
    }
}