using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//[RequireComponent(typeof(PhotonView))]
public class PoolLocalManager : MonoBehaviour
{
    [Serializable]
    public class ElementData
    {
        public GameObject Element;
        public uint amount;
    }

    public static PoolLocalManager Instance;

    [SerializeField] private List<ElementData> poolElements;

    public static Dictionary<GameObject, Queue<GameObject>> queuesDictionary;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetupDictionary();
    }

    private void SetupDictionary()
    {
        queuesDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (var elementData in poolElements)
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            for (int i = 0; i < elementData.amount; i++)
            {
                GameObject GO = Instantiate(elementData.Element, transform);
                GO.SetActive(false);
                newQueue.Enqueue(GO);
            }
            queuesDictionary.Add(elementData.Element, newQueue);
        }
    }

    public void EnqueuePool(GameObject objectPrefab, GameObject go)
    {
        queuesDictionary[objectPrefab].Enqueue(go);
    }

    public GameObject PoolInstantiate(GameObject GORef, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject returnGO;
        Debug.Log($"Instantiate {GORef.name} to {parent}");
        if (parent == null) parent = transform;
        if (queuesDictionary.ContainsKey(GORef))
        {
            var queue = queuesDictionary[GORef];
            if (queue.Count == 0)
            {
                returnGO = Instantiate(GORef, position, rotation, parent);
                var queuer = returnGO.AddComponent<Enqueuer>();
                queuer.GORef = GORef;
            }
            
            else
            {
                returnGO = queue.Dequeue();
                returnGO.transform.position = position;
                returnGO.transform.rotation = rotation;
                returnGO.SetActive(true);
            }
        }
        else
        {
            Debug.Log("New pool of " + GORef.name);
            queuesDictionary.Add(GORef, new Queue<GameObject>());
            
            returnGO = Instantiate(GORef, position, rotation, parent);
            var queuer = returnGO.AddComponent<Enqueuer>();
            queuer.GORef = GORef;
        }
        
        return returnGO;
    }

}