using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Champion : Entity, IMovable
{
    private Rigidbody rb;

    protected override void OnStart()
    {
        base.OnStart();
        rb = GetComponent<Rigidbody>();
        currentMoveSpeed = referenceMoveSpeed;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        Move();
    }


    //======  Movement
    private Vector3 lastDir;
    private bool isMoving;

    [SerializeField] float referenceMoveSpeed;
    float currentMoveSpeed = 3;

    public void SetMoveDirection(Vector3 dir)
    {
        lastDir = dir;
        isMoving = (dir != Vector3.zero) ;
    }

    void Move()
    {
        rb.velocity = lastDir * currentMoveSpeed;
    }
    
}
