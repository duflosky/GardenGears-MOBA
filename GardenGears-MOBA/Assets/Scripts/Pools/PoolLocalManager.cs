using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

//[RequireComponent(typeof(PhotonView))]
public class PoolLocalManager : MonoBehaviourPun
{
    [Serializable]
    public class ElementData
    {
        public GameObject Element;
        public uint amount;
        public float duration;
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

    public void RequestPoolInstantiate(GameObject GORef, Vector3 position, Quaternion rotation, Transform parent = null, float timer = 0)
    {
        var item = poolElements.FirstOrDefault(item => item.Element == GORef);
        if (item == default)
        {
            Debug.LogError($"GameObject Element {GORef.name} not Found in Pool");
            return;
        }
        var index = poolElements.IndexOf(item);
        photonView.RPC("SyncPoolInstantiate", RpcTarget.All, index, position, rotation, parent, timer);
        // PoolInstantiate(index, position, rotation, parent);
    }
    
    [PunRPC]
     void SyncPoolInstantiate(int GOIndex, Vector3 position, Quaternion rotation, Transform parent = null, float timer = 0)
    {
        PoolInstantiate(GOIndex, position, rotation, parent, timer);
    }

    public void PoolInstantiate(int GOindex, Vector3 position, Quaternion rotation, Transform parent = null, float timer = 0)
    {
        PoolInstantiate(poolElements[GOindex].Element , position, rotation, parent, timer);
    }
    
    
    public GameObject PoolInstantiate(GameObject GORef, Vector3 position, Quaternion rotation, Transform parent = null, float timer = 0)
    {
        GameObject returnGO;
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
                returnGO.transform.parent = parent;
                returnGO.SetActive(true);
            }
        }
        else
        {
            queuesDictionary.Add(GORef, new Queue<GameObject>());
            
            returnGO = Instantiate(GORef, position, rotation, parent);
            var queuer = returnGO.AddComponent<Enqueuer>();
            queuer.GORef = GORef;
        }

        if (timer != 0) StartCoroutine(TimeDisable(returnGO,timer));
        return returnGO;
    }
    
    IEnumerator TimeDisable(GameObject obj, float timer)
    {
        yield return new WaitForSeconds(timer);
        obj.SetActive(false);
    }

}