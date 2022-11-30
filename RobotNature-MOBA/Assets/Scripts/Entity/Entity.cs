using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Entity : MonoBehaviourPun
{
    /// <summary>
    /// The viewID of the photonView of the entity.
    /// </summary>
    public int entityIndex;

    /// <summary>
    /// True if passiveCapacities can be added to the entity's passiveCapacitiesList. False if not.
    /// </summary>
    [SerializeField] private bool canAddPassiveCapacity = true;

    /// <summary>
    /// True if passiveCapacities can be removed from the entity's passiveCapacitiesList. False if not.
    /// </summary>
    [SerializeField] private bool canRemovePassiveCapacity = true;

    /// <summary>
    /// The list of PassiveCapacity on the entity.
    /// </summary>
    public readonly List<PassiveCapacity> passiveCapacitiesList = new List<PassiveCapacity>();

    /// <summary>
    /// The transform of the UI of the entity.
    /// </summary>
    public Transform TransformUI;

    /// <summary>
    /// The offset of the UI of the entity.
    /// </summary>
    public Vector3 OffsetUI = new Vector3(0, 2f, 0);
    
    /// <summary>
    /// Elements to show (UI of the entity). Useful for the FOW later.
    /// </summary>
    public List<GameObject> elementsToShow = new List<GameObject>();
    
    /// <summary>
    /// Team of the entity. Useful for the FOW later.
    /// </summary>
    public Enums.Team team;
    
    /// <summary>
    /// Boolean to know if the entity can change its team. Useful for the FOW later.
    /// </summary>
    public bool canChangeTeam;
    
    void Start()
    { 
        entityIndex = photonView.ViewID;
        EntityCollectionManager.AddEntity(this); 
        OnStart();
    }
    protected virtual void OnStart(){}
    
    void Update()
    {
     OnUpdate();   
    }
    protected virtual void OnUpdate(){}
    
    
    public PassiveCapacity GetPassiveCapacityBySOIndex(byte soIndex)
    {
        return passiveCapacitiesList.FirstOrDefault(item => item.indexOfSo == soIndex);
    }
    
    public void ShowElements()
    {
        for (int i = 0; i < elementsToShow.Count; i++)
        {
            elementsToShow[i].SetActive(true);
        }
        OnShowElementFeedback?.Invoke();
    }

    public event GlobalDelegates.NoParameterDelegate OnShowElement;
    public event GlobalDelegates.NoParameterDelegate OnShowElementFeedback;
    
    public void HideElements()
    {
        for (int i = 0; i < elementsToShow.Count; i++)
        {
            elementsToShow[i].SetActive(false);
        }
        OnHideElementFeedback?.Invoke();
    }

    public event GlobalDelegates.NoParameterDelegate OnHideElement;
    public event GlobalDelegates.NoParameterDelegate OnHideElementFeedback;
    
    public Enums.Team GetTeam()
    {
        return team;
    }

    public List<Enums.Team> GetEnemyTeams()
    {
        return Enum.GetValues(typeof(Enums.Team)).Cast<Enums.Team>().Where(someTeam => someTeam != team)
            .ToList(); //returns all teams that are not 'team'
    }

    public bool CanChangeTeam()
    {
        return canChangeTeam;
    }

    public void RequestChangeTeam(Enums.Team team)
    {
        photonView.RPC("ChangeTeamRPC", RpcTarget.MasterClient, (byte)team);
    }

    [PunRPC]
    public void SyncChangeTeamRPC(byte team)
    {
        this.team = (Enums.Team)team;
    }

    [PunRPC]
    public void ChangeTeamRPC(byte team)
    {
        photonView.RPC("SyncChangeTeamRPC", RpcTarget.All, team);
    }

    public event GlobalDelegates.BoolDelegate OnChangeTeam;
    public event GlobalDelegates.BoolDelegate OnChangeTeamFeedback;
}
