using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Building;
using Entities.FogOfWar;
using Entities.Minion;
using GameStates;
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
    private readonly float spawnSpeed = 30;
    public Color minionColor;
    
    [Header("Minion Path Settings")]
    public List<Transform> pathfinding = new();
    public List<Building> enemyTowers = new();
    public string unitTag;
    
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        spawnCycleTime += Time.deltaTime;
        if (spawnCycleTime >= spawnSpeed)
        {
            StartCoroutine(SpawnMinionCo());
            spawnCycleTime = 0;
        }
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
        Entity minionGO = PoolNetworkManager.Instance.PoolInstantiate(minionPrefab, spawnPointForMinion.position, Quaternion.identity, transform.root.root.root.root);
        photonView.RPC("SyncMinionRPC", RpcTarget.All, minionGO.photonView.ViewID);
    }

    [PunRPC]
    public void SyncMinionRPC(int photonID)
    {
        Minion minion = PhotonNetwork.GetPhotonView(photonID).GetComponent<Minion>();
        minion.myWaypoints = pathfinding;
        minion.TowersList = enemyTowers;
        minion.team = unitTag.Contains(Enums.Team.Team1.ToString()) ? Enums.Team.Team1 : Enums.Team.Team2;
        minion.tag = unitTag;
        minion.meshParent.GetComponent<MeshRenderer>().material.color = minionColor;
        if (minion.canView && GameStateMachine.Instance.GetPlayerTeam() == minion.team) FogOfWarManager.Instance.AddFOWViewable(minion);
    }
}