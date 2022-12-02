using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enqueuer : MonoBehaviour
{
    public bool isLocal = true;
    public GameObject GORef;
    private void OnDisable()
    {
        if(isLocal)PoolLocalManager.Instance.EnqueuePool(GORef, gameObject);
    }
}
