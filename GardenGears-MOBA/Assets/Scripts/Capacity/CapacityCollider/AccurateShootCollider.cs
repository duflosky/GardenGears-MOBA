using System.Collections.Generic;
using UnityEngine;

public class AccurateShootCollider : AffectCollider
{
    private bool _isInObject;
    private Dictionary<GameObject, Vector3> _gameObjectsCrossed = new();

    protected override bool CanDisable()
    {
        return !_isInObject && base.CanDisable();
    }

    public void EnterWall(GameObject target)
    {
        _gameObjectsCrossed.Add(target, transform.position);
        _isInObject = false;
    }

    public void ExitWall(GameObject target)
    {
        if (!_gameObjectsCrossed.ContainsKey(target)) return;
        var enterPosition = _gameObjectsCrossed[target]; 
        MaxRange += Vector3.Distance(enterPosition, transform.position);
        _gameObjectsCrossed.Remove(target);
        _isInObject = true;
    }
}
