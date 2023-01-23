using System.Collections.Generic;
using System.Linq;
using Entities;
using GameStates;
using GameStates.States;
using Photon.Pun;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CaptureZone : MonoBehaviourPun
{
    [SerializeField] private float incrementRate = .1f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshProUGUI dominationPoints;
    private float firstTeamControl;
    private float secondTeamControl;
    private float pointCD;
    private float CDTimer = 3f;
    private Enums.Team dominatingTeam;
    private List<Entity> firstTeamEntities;
    private List<Entity> secondTeamEntities;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameStateMachine.Instance.OnTick += UpdateState;
        GameStateMachine.Instance.OnTick += CheckControl;
        GameStateMachine.Instance.OnTick += CheckEntities;
    }

    private void UpdateState()
    {
        switch (dominatingTeam)
        {
            case Enums.Team.Neutral:
                if (firstTeamControl<50 && firstTeamControl>0) DecreaseControl(Enums.Team.Team1);
                if (secondTeamControl<50 && secondTeamControl>0) DecreaseControl(Enums.Team.Team2);
                break;
            case Enums.Team.Team1:
                if (secondTeamControl>0) DecreaseControl(Enums.Team.Team2);
                else IncreaseControl(Enums.Team.Team1);
                break;
            case Enums.Team.Team2:
                if (firstTeamControl>0) DecreaseControl(Enums.Team.Team1);
                else IncreaseControl(Enums.Team.Team2);
                break;
        }

        if (pointCD > 0) pointCD -= (float)GameStateMachine.Instance.tickRate;
        else
        {
            if (firstTeamControl == 50) TryAddPoint(Enums.Team.Team1);
            else if (secondTeamControl == 50) TryAddPoint(Enums.Team.Team2);
        }
    }

    private void OnDisable()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        GameStateMachine.Instance.OnTick -= UpdateState;
        GameStateMachine.Instance.OnTick -= CheckControl;
        GameStateMachine.Instance.OnTick -= CheckEntities;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var entity = other.GetComponent<Champion>();
        if (!entity) return;
        if(entity.GetTeam() == Enums.Team.Team1) firstTeamEntities.Add(entity);
        else secondTeamEntities.Add(entity);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var entity = other.GetComponent<Champion>();
        if (!entity) return;
        if (firstTeamEntities.Contains(entity) || secondTeamEntities.Contains(entity)) return;
        if(entity.GetTeam() == Enums.Team.Team1) firstTeamEntities.Add(entity);
        else secondTeamEntities.Add(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var entity = other.GetComponent<Champion>();
        if (!entity) return;
        if(entity.GetTeam() == Enums.Team.Team1) firstTeamEntities.Remove(entity);
        else secondTeamEntities.Remove(entity);
    }

    private void CheckControl()
    {
        var firstTeamAmount = firstTeamEntities.Count;
        var secondTeamAmount = secondTeamEntities.Count;
        if (firstTeamAmount > secondTeamAmount) dominatingTeam = Enums.Team.Team1;
        else if (firstTeamAmount < secondTeamAmount) dominatingTeam = Enums.Team.Team2;
        else dominatingTeam = Enums.Team.Neutral;
    }

    private void CheckEntities()
    {
        if (firstTeamEntities.Count > 0)
        {
            foreach (var firstTeamEntity in firstTeamEntities.Where(firstTeamEntity => !firstTeamEntity.GetComponent<Champion>().isAlive))
            {
                firstTeamEntities.Remove(firstTeamEntity);
            }
        }
        else if (secondTeamEntities.Count > 0)
        {
            foreach (var secondTeamEntity in secondTeamEntities.Where(secondTeamEntity => !secondTeamEntity.GetComponent<Champion>().isAlive))
            {
                secondTeamEntities.Remove(secondTeamEntity);
            }
        }
    }

    private void IncreaseControl(Enums.Team increaseTeam)
    {
        if (increaseTeam == Enums.Team.Team1)
        {
            if (50-firstTeamControl <= incrementRate) firstTeamControl = 50;
            else secondTeamControl += incrementRate;
        }
        else
        {
            if (50-secondTeamControl <= incrementRate)secondTeamControl = 50;
            else secondTeamControl += incrementRate;
        }
    }

    private void DecreaseControl(Enums.Team decreaseTeam)
    {
        if (decreaseTeam == Enums.Team.Team1)
        {
            if (firstTeamControl <= incrementRate) firstTeamControl = 0;
            else secondTeamControl -= incrementRate;
        }
        else
        {
            if (secondTeamControl <= incrementRate) secondTeamControl = 0;
            else secondTeamControl -= incrementRate;
        }
    }

    private void TryAddPoint(Enums.Team pointTeam)
    {
        if (pointCD > 0) return;
        ((InGameState)GameStateMachine.Instance.currentState).AddPoint(pointTeam);
        pointCD = CDTimer;
    }

    private void ChangeOwner()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("ChangeOwnerRPC", RpcTarget.All, firstTeamControl, secondTeamControl);
    }
    
    [PunRPC]
    private void ChangeOwnerRPC(float firstTeam, float secondTeam)
    {
        if (firstTeam == 0 && secondTeam == 0) meshRenderer.material.color = Color.grey;
        else meshRenderer.material.color = Color.Lerp(Color.red, Color.blue, firstTeam / 100);
        dominationPoints.text = $"{firstTeam} / {secondTeam}";
    }
}
