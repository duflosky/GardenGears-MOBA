using Photon.Pun;
using UnityEngine;

public partial class Champion
{
    [Header("=== MOUVEMENT")] private Vector3 lastDir;
    private bool isMoving;

    [SerializeField] float referenceMoveSpeed;
    float currentMoveSpeed = 3;

    public Transform rotateParent;
    public float currentRotateSpeed;
    public bool canRotate = true;
    private Vector3 rotateDirection;

    public void SetMoveDirection(Vector3 dir)
    {
        lastDir = dir;
        isMoving = (dir != Vector3.zero);
    }
    
    void Move()
    {
        if (!animator) return;
        rb.velocity = lastDir * currentMoveSpeed;
        if (isMoving) animator.SetBool("isRunning", true);
        else animator.SetBool("isRunning", false);
    }

    private void RotateMath()
    {
        if (!photonView.IsMine) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity)) return;

        rotateDirection = -(transform.position - hit.point);
        rotateDirection.y = 0;
    }

    private void Rotate()
    {
        RotateMath();
        if(rotateDirection != Vector3.zero && canRotate)rotateParent.transform.rotation = Quaternion.Lerp(rotateParent.transform.rotation,Quaternion.LookRotation(rotateDirection),Time.deltaTime * currentRotateSpeed);
    }

    public float GetReferenceMoveSpeed()
    {
        return referenceMoveSpeed;
    }

    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }

    public void RequestSetReferenceMoveSpeed(float value)
    {
        photonView.RPC("SetReferenceMoveSpeedRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        OnSetReferenceMoveSpeedFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        OnSetReferenceMoveSpeed?.Invoke(value);
        photonView.RPC("SyncSetReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

    public void RequestIncreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed = amount;
        OnIncreaseReferenceMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed += amount;
        OnIncreaseReferenceMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncIncreaseReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

    public void RequestDecreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed = amount;
        OnDecreaseReferenceMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed -= amount;
        OnDecreaseReferenceMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncDecreaseReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

    public void RequestSetCurrentMoveSpeed(float value)
    {
        photonView.RPC("SetCurrentMoveSpeedRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        OnSetCurrentMoveSpeedFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        OnSetCurrentMoveSpeed?.Invoke(value);
        photonView.RPC("SyncSetCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

    public void RequestIncreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed = amount;
        OnIncreaseCurrentMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed += amount;
        OnIncreaseCurrentMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncIncreaseCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

    public void RequestDecreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed = amount;
        OnDecreaseCurrentMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed -= amount;
        OnDecreaseCurrentMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;

}
