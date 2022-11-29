using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Entity : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
       OnStart(); 
    }
    protected virtual void OnStart(){}

    // Update is called once per frame
    void Update()
    {
     OnUpdate();   
    }
    protected virtual void OnUpdate(){}
}
