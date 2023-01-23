using System;
using System.Linq;
using Entities;
using GameStates;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class StickyBombCollider : Entity
{
    [HideInInspector] public bool isIgnite;
    [HideInInspector] public Entity caster;
    [HideInInspector] public float distance;
    [HideInInspector] public StickyBomb capacity;
    [SerializeField] private GameObject[] particles;
    [SerializeField] private Material[] outlineMaterials;
    [SerializeField] private MeshRenderer outlineMeshRenderer;
    [SerializeField] private Color[] teamColors;
    public Image timerImage;

    private Rigidbody rb;
    private Vector3 lastPositionCaster;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timerImage.enabled = false;
    }

    private void OnEnable()
    {
        isIgnite = false;
        timerImage.fillAmount = 0;
        timerImage.enabled = false;
    }

    private void OnDisable()
    {
        isIgnite = false;
        timerImage.fillAmount = 0;
        timerImage.enabled = false;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!CanDisable()) return;
        if (!(Vector3.Distance(lastPositionCaster, transform.position) > distance) || isIgnite) return;
        ActivateParticleSystem(false);
        rb.isKinematic = true;
        timerImage.enabled = true;
        GameStateMachine.Instance.OnTick += capacity.TimerBomb;
        GetComponent<SphereCollider>().enabled = true;
        isIgnite = true;
    }

    protected virtual bool CanDisable()
    {
        return distance != 0;
    }

    public void Launch(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.velocity = direction;
        lastPositionCaster = caster.transform.position;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
        switch (caster.team)
        {
            case Enums.Team.Team1:
                outlineMeshRenderer.materials = outlineMeshRenderer.materials.Append(outlineMaterials[0]).ToArray();
                timerImage.color = teamColors[0];
                break;
            case Enums.Team.Team2:
                outlineMeshRenderer.materials = outlineMeshRenderer.materials.Append(outlineMaterials[1]).ToArray();
                timerImage.color = teamColors[1];
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var affectedEntity = other.GetComponent<Entity>();
        if (!affectedEntity) return;
        if (!PhotonNetwork.IsMasterClient) return; 
        capacity.CollideEntityEffect(affectedEntity);
    }

    public void Disable()
    {
        GetComponent<SphereCollider>().enabled = false;
        photonView.RPC("SyncDisableRPC", RpcTarget.All);
    }
    
    [PunRPC]
    public void SyncDisableRPC()
    {
        gameObject.SetActive(false);
    }
    
    public void ActivateParticleSystem(bool value)
    {
        photonView.RPC("SyncActivateParticleSystemRPC", RpcTarget.All, value);
    }
    
    [PunRPC]
    private void SyncActivateParticleSystemRPC(bool value)
    {
        foreach (var particle in particles)
        {
            particle.SetActive(value);
        }
    }
}