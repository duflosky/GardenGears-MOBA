using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Entities.Champion;
using Entities.FogOfWar;
using Items;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public partial class Champion : Entity, IMovable, IInventoryable, IResourceable, ICastable, IActiveLifeable, IDeadable, IAttackable
{
    private Rigidbody rb;
    public ChampionSO championSo;

    [SerializeReference] public List<Item> items = new List<Item>();

    private UI.InGame.UIManager uiManager;

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
        uiManager = UI.InGame.UIManager.Instance;
        maxResource = so.maxRessource;
        currentResource = so.maxRessource;
        //viewRange = championSo.viewRange;
        referenceMoveSpeed = championSo.referenceMoveSpeed;
        currentMoveSpeed = referenceMoveSpeed;
        //attackDamage = championSo.attackDamage;
        //attackAbilityIndex = championSo.attackAbilityIndex;
        // TODO : Instantiate mesh champion ?
        var championMesh = Instantiate(championSo.championMeshPrefab, rotateParent.position, Quaternion.identity, rotateParent);
        championMesh.transform.localEulerAngles = Vector3.zero;
        abilitiesIndexes = championSo.activeCapacitiesIndexes;
        ultimateAbilityIndex = championSo.ultimateAbilityIndex;

        foreach (var passif in so.passiveCapacities)
        {
            
        }

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
        
        if (GameStates.GameStateMachine.Instance.GetPlayerTeam() != team)
        {
            championMesh.SetActive(false);
        }
        
        championMesh.GetComponent<ChampionMeshLinker>().LinkTeamColor(this.team);
        elementsToShow.Add(championMesh);

        respawnPos = transform.position = pos.position;

        if (uiManager != null)
        {
            uiManager.InstantiateHealthBarForEntity(entityIndex);
            uiManager.InstantiateResourceBarForEntity(entityIndex);
        }
    }
    
}