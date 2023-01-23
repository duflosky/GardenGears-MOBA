using System;
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
    [SerializeField] private int incrementRate = 1;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshProUGUI dominationPoints;
    [SerializeField] private int firstTeamControl;
    [SerializeField] private int secondTeamControl;
    private float pointCooldown;
    private float cooldownTimer = 12f;
    private Enums.Team dominatingTeam;
    [SerializeField] private List<Entity> firstTeamEntities = new();
    [SerializeField] private List<Entity> secondTeamEntities = new();

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameStateMachine.Instance.OnTick += CheckControl;
        GameStateMachine.Instance.OnTick += CheckEntities;
        GameStateMachine.Instance.OnTick += UpdateState;
        pointCooldown = 0;
    }

    private void UpdateState()
    {
        ChangeOwner();
        switch (dominatingTeam)
        {
            case Enums.Team.Neutral:
                if (firstTeamControl == 0 && secondTeamControl == 0) break;
                if (firstTeamControl is <= 50 and > 0) DecreaseControl(Enums.Team.Team1);
                if (secondTeamControl is <= 50 and > 0) DecreaseControl(Enums.Team.Team2);
                break;
            case Enums.Team.Team1:
                if (firstTeamControl == 50) break;
                if (secondTeamControl > 0) DecreaseControl(Enums.Team.Team2);
                else IncreaseControl(Enums.Team.Team1);
                break;
            case Enums.Team.Team2:
                if (secondTeamControl == 50) break;
                if (firstTeamControl > 0) DecreaseControl(Enums.Team.Team1);
                else IncreaseControl(Enums.Team.Team2);
                break;
            default:
                break;
        }

        pointCooldown += 1 / (float)GameStateMachine.Instance.tickRate;
        if (pointCooldown < cooldownTimer) return;
        if (firstTeamControl == 50) TryAddPoint(Enums.Team.Team1);
        else if (secondTeamControl == 50) TryAddPoint(Enums.Team.Team2);
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
        else if (firstTeamAmount == secondTeamAmount) dominatingTeam = default;
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
        else if (firstTeamEntities.Count == 0 && secondTeamEntities.Count == 0)
        {
            dominatingTeam = Enums.Team.Neutral;
        }
    }

    private void IncreaseControl(Enums.Team increaseTeam)
    {
        if (increaseTeam == Enums.Team.Team1)
        {
            firstTeamControl += incrementRate;
        }
        else
        {
            secondTeamControl += incrementRate;
        }
    }

    private void DecreaseControl(Enums.Team decreaseTeam)
    {
        if (decreaseTeam == Enums.Team.Team1)
        {
            firstTeamControl -= incrementRate;
        }
        else
        {
            secondTeamControl -= incrementRate;
        }
    }

    private void TryAddPoint(Enums.Team pointTeam)
    {
        pointCooldown = 0;
        ((InGameState)GameStateMachine.Instance.currentState).AddPoint(pointTeam);
    }

    private void ChangeOwner()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("ChangeOwnerRPC", RpcTarget.All, firstTeamControl, secondTeamControl);
    }
    
    [PunRPC]
    private void ChangeOwnerRPC(int firstTeam, int secondTeam)
    {
        if (firstTeam == 0 && secondTeam == 0) meshRenderer.material.color = Color.grey;
        else meshRenderer.material.color = Color.Lerp(Color.red, Color.blue, firstTeam / 100);
        dominationPoints.text = $"{firstTeam} / {secondTeam}";
    }
}
