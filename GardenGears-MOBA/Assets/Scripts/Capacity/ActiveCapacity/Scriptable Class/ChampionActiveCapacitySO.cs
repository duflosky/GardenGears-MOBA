using Entities.Capacities;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ChampionActiveCapacitySO : ActiveCapacitySO
{
    [FormerlySerializedAs("gizmoPrefab")] [Space] public GameObject Gizmo;
    
    [FormerlySerializedAs("capacitySlow")] public SpeedModifierPassiveSO CapacitySlowSO;
}