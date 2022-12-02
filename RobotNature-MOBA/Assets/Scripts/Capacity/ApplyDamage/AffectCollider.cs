using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AffectCollider : MonoBehaviour
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public ActiveCapacity capacitySender;
    [SerializeField] private List<byte> effectIndex = new List<byte>();
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 moveVector)
    {
        rb.velocity = moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Entity entity = other.GetComponent<Entity>();

        if (entity && entity != caster)
        {
            IActiveLifeable activeLifeable = entity.GetComponent<IActiveLifeable>();
                
            if (PhotonNetwork.IsMasterClient)
            {
                capacitySender.CollideEffect(entity);
            }
        }
    }
}
