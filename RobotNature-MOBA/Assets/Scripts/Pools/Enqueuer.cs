using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class Enqueuer : MonoBehaviour
{
    public bool isLocal = true;
    public GameObject GORef;
    public Entity entityRef;
    
    private void OnDisable()
    {
        if(isLocal)PoolLocalManager.Instance.EnqueuePool(GORef, gameObject);
        PoolNetworkManager.Instance.EnqueuePool(entityRef, GetComponent<Entity>());
    }
}
