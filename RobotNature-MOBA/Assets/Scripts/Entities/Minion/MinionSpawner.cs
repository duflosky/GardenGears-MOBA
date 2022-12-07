using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Building;
using Entities.Minion;
using Photon.Pun;
using UnityEngine;

// TODO: Add a way to spawn minions at the start of the game not immediately after the scene loads

public class MinionSpawner : Building
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
    
    //TODO: Possibility to use GameState to spawn minions
    private void Update()
    {
        // Spawn de minion
        // TODO: Find a way to stop the spawn when too many minions are on the map
        spawnCycleTime += Time.deltaTime;
        if (spawnCycleTime >= spawnSpeed)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient) StartCoroutine(SpawnMinionCo());
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
        
        Minion minionScript = minionGO.GetComponent<Minion>();
        minionScript.myWaypoints = pathfinding;
        minionScript.TowersList = enemyTowers;
        minionScript.team = unitTag.Contains(Enums.Team.Team1.ToString()) ? Enums.Team.Team1 : Enums.Team.Team2;
        minionScript.tag = unitTag;
        minionScript.meshParent.GetComponent<MeshRenderer>().material.color = minionColor;
    }
}