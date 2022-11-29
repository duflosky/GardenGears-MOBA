using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInstantiate : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        Vector3 pos = new Vector3(Random.Range(0f, 10f), 1, Random.Range(0f, 10f));
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
    }
}
