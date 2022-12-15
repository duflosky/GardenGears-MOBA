using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccurateShootCollider : AffectCollider
{
    private Vector3 lastCountPos;
    private bool canRangeDestroy = true;

    protected override bool CanDisable()
    {
        if (!canRangeDestroy) return false;
        return base.CanDisable();
    }

    public void EnterWall()
    {
        lastCountPos = transform.position;
        canRangeDestroy = false;
    }

    public void ExitWall()
    {
        maxDistance += Vector3.Distance(lastCountPos, transform.position);
        canRangeDestroy = true;
    }
}
