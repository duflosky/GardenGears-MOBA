using System;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Entities.Champion;
using Items;
using Photon.Pun;
using UI.InGame;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public partial class Champion : Entity, IMovable, IInventoryable, IResourceable, ICastable, IActiveLifeable, IDeadable, IAttackable
{
    private Rigidbody rb;
    public ChampionSO championSo;

    [SerializeReference] public List<Item> items = new();

    private UIManager uiManager;
    private Animator animator;

    protected override void OnStart()
    {
        base.OnStart();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        Move();
        Rotate();
        CastUpdate?.Invoke();
    }

    private void OnEnable()
    {
        GameStateMachine.Instance.OnTick += DecreaseCooldown;
    }

    private void OnDisable()
    {
        GameStateMachine.Instance.OnTick -= DecreaseCooldown;
    }

    public void ApplyChampionSO(byte championSoIndex, Enums.Team newTeam)
    {
        var so = GameStateMachine.Instance.allChampionsSo[championSoIndex];
        championSo = so;
        maxHp = championSo.maxHp;
        currentHp = maxHp;
        uiManager = UIManager.Instance;
        maxResource = so.maxRessource;
        currentResource = 0;
        //viewRange = championSo.viewRange;
        referenceMoveSpeed = championSo.referenceMoveSpeed;
        attackSpeed = championSo.referenceAttackSpeed;
        currentMoveSpeed = referenceMoveSpeed;
        attackDamage = championSo.attackDamage;
        //attackAbilityIndex = championSo.attackAbilityIndex;
        
        var championMesh = Instantiate(championSo.championMeshPrefab, rotateParent.position, Quaternion.identity);
        championMesh.transform.SetParent(rotateParent);
        championMesh.GetComponent<PhotonView>().ViewID = PhotonNetwork.AllocateViewID(gameObject.GetComponent<PhotonView>().OwnerActorNr);
        if (gameObject.GetComponent<PhotonView>().Owner != PhotonNetwork.LocalPlayer) championMesh.GetComponent<PhotonView>().TransferOwnership(gameObject.GetComponent<PhotonView>().Owner);
        championMesh.transform.localEulerAngles = Vector3.zero;
        animator = championMesh.GetComponent<Animator>();
        animator.SetFloat("attackSpeed", championSo.referenceAttackSpeed);
        if (animator) animator.GetComponent<AnimationCallbacks>().caster = this;
        elementsToShow.Add(championMesh);
        neverHideElements.Add(championMesh);

        abilitiesIndexes = championSo.activeCapacitiesIndexes;
        ultimateAbilityIndex = championSo.ultimateAbilityIndex;

        foreach (var passif in so.passiveCapacities)
        {
           PassiveCapacity capa = CapacitySOCollectionManager.Instance.CreatePassiveCapacity(passif, this);
           passiveCapacitiesList.Add(capa);
        }

        CheckSpawnPos(newTeam);
        
        if (GameStateMachine.Instance.GetPlayerTeam() == Enums.Team.Team1)
        {
            if (!championMesh.GetComponentInChildren<SkinnedMeshRenderer>()) return;
            foreach (var skinnedMeshRenderer in championMesh.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                skinnedMeshRenderer.material = so.materials[0];   
            }
        }
        else
        {
            if (!championMesh.GetComponentInChildren<MeshRenderer>()) return;
            foreach (var skinnedMeshRenderer in championMesh.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                skinnedMeshRenderer.material = so.materials[1];   
            }
            // championMesh.SetActive(false);
        }
        
        if (uiManager == null) return;
        EntityCollectionManager.AddEntity(this);
        uiManager.InstantiateHealthBarForEntity(entityIndex);
        uiManager.InstantiateResourceBarForEntity(entityIndex);
    }

    private void CheckSpawnPos(Enums.Team newTeam)
    {
        team = newTeam;
        Transform pos = transform;
        switch (team)
        {
            case Enums.Team.Team1:
            {
                for (int i = 0; i < MapLoaderManager.Instance.firstTeamBasePoint.Length; i++)
                {
                    if (MapLoaderManager.Instance.firstTeamBasePoint[i].champion == null)
                    {
                        pos = MapLoaderManager.Instance.firstTeamBasePoint[i].position;
                        MapLoaderManager.Instance.firstTeamBasePoint[i].champion = this;
                        break;
                    }
                }

                break;
            }
            case Enums.Team.Team2:
            {
                for (int i = 0; i < MapLoaderManager.Instance.secondTeamBasePoint.Length; i++)
                {
                    if (MapLoaderManager.Instance.secondTeamBasePoint[i].champion == null)
                    {
                        pos = MapLoaderManager.Instance.secondTeamBasePoint[i].position;
                        MapLoaderManager.Instance.secondTeamBasePoint[i].champion = this;
                        break;
                    }
                }

                break;
            }
            default:
                Debug.LogError("Team is not valid.");
                pos = transform;
                break;
        }

        respawnPos = transform.position = pos.position;
    }
}