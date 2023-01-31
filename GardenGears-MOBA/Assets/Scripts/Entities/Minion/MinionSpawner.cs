using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Building;
using Entities.FogOfWar;
using Entities.Minion;
using GameStates;
using GameStates.States;
using Photon.Pun;
using UnityEngine;

public class MinionSpawner : MonoBehaviourPun
{
    [Header("Minion Prefab")]
    public Minion minionPrefab;

    [Header("Minion Spawn Settings")]
    public Transform spawnPointForMinion;
    public int spawnMinionAmount = 5;
    public float spawnMinionInterval = 1.7f;
    public float spawnCycleTime = 30;
    private float spawnSpeed = 30;

    [Header("Minion Path Settings")]
    public List<Transform> pathfinding = new();
    public List<Building> enemyTowers = new();
    public Enums.Team team;

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        spawnCycleTime += Time.deltaTime;
        if (spawnCycleTime < spawnSpeed) return;
        if (GameStateMachine.Instance.currentState is not InGameState) return;
        StartCoroutine(SpawnMinionCo());
        spawnCycleTime = 0;
    }
    
    private IEnumerator SpawnMinionCo()
    {
        for (int i = 0; i < spawnMinionAmount; i++)
        {
            SpawnMinion();
            yield return new WaitForSeconds(spawnMinionInterval);
        }
    }

    private void SpawnMinion()
    {
        var minionGO = PoolNetworkManager.Instance.PoolInstantiate(minionPrefab, spawnPointForMinion.position, Quaternion.identity, transform.root.root.root.root);
        photonView.RPC("SyncMinionRPC", RpcTarget.All, minionGO.photonView.ViewID);
    }

    [PunRPC]
    public void SyncMinionRPC(int photonID)
    {
        var minion = PhotonNetwork.GetPhotonView(photonID).GetComponent<Minion>();
        minion.RequestRevive();
        minion.myWaypoints = pathfinding;
        minion.towersList = enemyTowers;
        minion.team = team;
        // minion.meshParent.gameObject.SetActive(GameStateMachine.Instance.GetPlayerTeam() == team);
        if (minion.canView) FogOfWarManager.Instance.AddFOWViewable(minion);
    }
}