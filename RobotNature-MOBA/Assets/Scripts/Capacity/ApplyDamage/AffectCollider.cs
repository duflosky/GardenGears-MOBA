using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AffectCollider : MonoBehaviour
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public ActiveCapacity capacitySender;
    [SerializeField] private List<byte> effectIndex = new List<byte>();
    private bool affectEntityOnly;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 moveVector)
    {
        Debug.Log($"MoveVector: {moveVector}");
        rb.velocity = moveVector;
        Debug.Log(rb.velocity);
    }

    private void OnDisable()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (!PhotonNetwork.IsMasterClient)
        {
            if(entity != caster)gameObject.SetActive(false);
            return;
        }
        

        if (entity && entity != caster)
        {
            IActiveLifeable activeLifeable = entity.GetComponent<IActiveLifeable>();
                
            if (PhotonNetwork.IsMasterClient)
            {
                capacitySender.CollideEntityEffect(entity);
            }
        }
        else if (!entity && !affectEntityOnly)
        {
            
        }
    }
}
