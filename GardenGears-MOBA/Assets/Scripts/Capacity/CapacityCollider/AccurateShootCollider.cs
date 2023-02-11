using System.Collections.Generic;
using UnityEngine;

public class AccurateShootCollider : AffectCollider
{
    private bool canRangeDestroy = true;

    private Dictionary<GameObject, Vector3> gameObjectDistances = new();

    protected override bool CanDisable()
    {
        if (!canRangeDestroy) return false;
        return base.CanDisable();
    }

    public void EnterWall(GameObject go)
    {
        gameObjectDistances.Add(go, transform.position);
        canRangeDestroy = false;
    }

    public void ExitWall(GameObject go)
    {
        if (!gameObjectDistances.ContainsKey(go)) return;
        Vector3 lastCountPos = gameObjectDistances[go]; 
        maxDistance += Vector3.Distance(lastCountPos, transform.position);
        gameObjectDistances.Remove(go);
        canRangeDestroy = true;
    }
}
