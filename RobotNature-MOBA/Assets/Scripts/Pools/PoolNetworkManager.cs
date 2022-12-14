using System;
using System.Collections.Generic;
using Entities;
using GameStates;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PoolNetworkManager : MonoBehaviourPun
{
    [Serializable]
    public class ElementData
    {
        public Entity Element;
        public uint amount;
    }

    private bool isSetup;

    public static PoolNetworkManager Instance;

    [SerializeField] private List<ElementData> poolElements;
    
    public static Dictionary<Entity, Queue<Entity>> queuesDictionary;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        SetupDictionary();
    }

    private void SetupDictionary()
    {
        queuesDictionary = new Dictionary<Entity, Queue<Entity>>();
        foreach (var elementData in poolElements)
        {
            Queue<Entity> newQueue = new Queue<Entity>();
            for (int i = 0; i < elementData.amount; i++)
            {
                Entity entity = PhotonNetwork.Instantiate(elementData.Element.gameObject.name, transform.position, Quaternion.identity).GetComponent<Entity>();
                entity.gameObject.SetActive(false);
                newQueue.Enqueue(entity);
            }
            queuesDictionary.Add(elementData.Element, newQueue);
        }
        isSetup = true;
    }

    public Entity PoolInstantiate(byte index, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (index >= poolElements.Count) return null;
        var entityRef = poolElements[index].Element;
        return PoolInstantiate(entityRef, position, rotation, parent);
    }

    public Entity PoolInstantiate(Entity entityRef, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if(!isSetup) SetupDictionary();
        Entity entity;
        if (parent == null) parent = transform;
        if (queuesDictionary.ContainsKey(entityRef))
        {
            var queue = queuesDictionary[entityRef];
            if (queue.Count == 0)
            {
                entity = PhotonNetwork.Instantiate(entityRef.gameObject.name, position, rotation).GetComponent<Entity>();
                var enqueuer = entity.AddComponent<Enqueuer>();
                enqueuer.isLocal = false;
                enqueuer.entityRef = entityRef;
            }
            else
            {
                entity = queue.Dequeue();
                photonView.RPC("SyncInstantiateRPC", RpcTarget.All, entity.entityIndex, position, rotation);
            }
        }
        else
        {
            queuesDictionary.Add(entityRef, new Queue<Entity>());
            entity = PhotonNetwork.Instantiate(entityRef.gameObject.name, position, rotation).GetComponent<Entity>();
            var enqueuer = entity.AddComponent<Enqueuer>();
            enqueuer.isLocal = false;
            enqueuer.entityRef = entityRef;
        }

        return entity;
    }
    
    public void EnqueuePool(Entity entityRef, Entity entity)
    {
        queuesDictionary[entityRef].Enqueue(entity);
    }
    
    [PunRPC]
    public void SyncInstantiateRPC(int entityRef, Vector3 position, Quaternion rotation)
    {
        Entity entity = EntityCollectionManager.GetEntityByIndex(entityRef);
        entity.transform.position = position;
        entity.transform.rotation = rotation;
        entity.gameObject.SetActive(true);
    }
}