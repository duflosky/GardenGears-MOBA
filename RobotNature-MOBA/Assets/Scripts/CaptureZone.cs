using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using GameStates;
using GameStates.States;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CaptureZone : MonoBehaviourPun
{
    private List<Entity> firstTeamEntities;
    private List<Entity> secondTeamEntities;
    private float firstTeamControl = 0;
    private float secondTeamControl = 0;
    [SerializeField] float incrementRate = .1f;
    private Enums.Team dominatingTeam;
    private float pointCD;
    private float CDTimer = 3f;

    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient)return;
        
    }

    void IncreaseControle(Enums.Team increaseTeam)
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

    void DecreaseControl(Enums.Team Decreaseteam)
    {
        if (Decreaseteam == Enums.Team.Team1)
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

    void CheckControl()
    {
        var firstTeamAmount = firstTeamEntities.Count;
        var secondTeamamount = secondTeamEntities.Count;
        if (firstTeamAmount > secondTeamamount) dominatingTeam = Enums.Team.Team1;
        else if (firstTeamAmount < secondTeamamount) dominatingTeam = Enums.Team.Team2;
        else dominatingTeam = Enums.Team.Neutral;
    }

    private void UpdateState()
    {
        switch (dominatingTeam)
        {
            case Enums.Team.Neutral:
                if(firstTeamControl<50 && firstTeamControl>0)DecreaseControl(Enums.Team.Team1);
                if(secondTeamControl<50 && secondTeamControl>0)DecreaseControl(Enums.Team.Team2);
                break;
            case Enums.Team.Team1:
                if(secondTeamControl>0)DecreaseControl(Enums.Team.Team2);
                else IncreaseControle(Enums.Team.Team1);
                break;
            case Enums.Team.Team2:
                if(firstTeamControl>0)DecreaseControl(Enums.Team.Team1);
                else IncreaseControle(Enums.Team.Team2);
                break;
        }

        if (pointCD > 0) pointCD -= (float)GameStateMachine.Instance.tickRate;
        else
        {
            if(firstTeamControl == 50)TryAddPoint(Enums.Team.Team1);
            else if (secondTeamControl == 50)TryAddPoint(Enums.Team.Team2);
        }
    }

    void TryAddPoint(Enums.Team pointTeam)
    {
        if (pointCD > 0) return;
        ((InGameState)GameStateMachine.Instance.currentState).AddPoint(pointTeam);
        pointCD = CDTimer;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            if(entity.GetTeam() == Enums.Team.Team1) firstTeamEntities.Add(entity);
            else secondTeamEntities.Add(entity);
            
            CheckControl();
            UpdateState();
        }
    }
}
