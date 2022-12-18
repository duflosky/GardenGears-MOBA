using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccurateShootCollider : AffectCollider
{
    private bool canRangeDestroy = true;

    private Dictionary<GameObject, Vector3> gameObjectDistances = new Dictionary<GameObject, Vector3>();

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
        //Debug.Log($"Add {Vector3.Distance(lastCountPos, transform.position)} to maxDistance");
        canRangeDestroy = true;
    }

    public override void Disable()
    {
        //Debug.Log($"disable with maxdistance : {maxDistance}");
        base.Disable();
    }
}
