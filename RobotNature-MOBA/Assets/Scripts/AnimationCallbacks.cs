using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviour
{
    [SerializeField] private Transform castTransform;
    public ICastable caster;

    
    public void AnimationCast()
    {
        caster.CastAnimationCast(castTransform);
    }

    public void AnimationEnd()
    {
        caster.CastAnimationEnd();
    }
}
