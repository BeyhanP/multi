using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int poolSize;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private int _counter;
    public static ObjectPooler instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        for (int i = 0; i < pools.Count; i++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int a = 0; a < pools[i].poolSize; a++)
            {
                GameObject poolObject = Instantiate(pools[i].prefab);
                poolObject.SetActive(false);
                poolObject.transform.parent = transform;
                objectPool.Enqueue(poolObject);
            }
            poolDictionary.Add(pools[i].tag, objectPool);
        }
        _counter = 0;
    }
    public void addObjectsToPool(string tag, int amount, int numberInPool)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject addedPoolObject = Instantiate(pools[numberInPool].prefab);
            addedPoolObject.SetActive(false);
            addedPoolObject.transform.parent = transform;
            poolDictionary[tag].Enqueue(addedPoolObject);
        }
        pools[numberInPool].poolSize += amount;

    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}