using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }
    public List<Pool> Pools;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;


    #region Singleton
    static private ObjectPooler instance;
    static public ObjectPooler Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        #region Fill Dictionary
        foreach (Pool pool in Pools)
        {
            var poolContainer = transform.Find(pool.Tag);

            if (!poolContainer)
            {
                poolContainer = new GameObject(pool.Tag).transform;
                poolContainer.SetParent(transform, true);
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab, poolContainer.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            PoolDictionary.Add(pool.Tag, objectPool);
        }
        #endregion

    }

    public GameObject SpawnFromPool(string tag)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("No existe pool con el tag: " + tag);
            return null;
        }
        if (PoolDictionary[tag].Count <= 0)
        {
            //Debug.LogWarning("No hay " + tag + " disponibles");
            foreach (Pool pool in Pools)
            {
                var poolContainer = transform.Find(pool.Tag);

                if (!poolContainer)
                {
                    poolContainer = new GameObject(pool.Tag).transform;
                    poolContainer.SetParent(transform, true);
                }
                if (pool.Tag == tag)
                {
                    GameObject toSpawn = Instantiate(pool.Prefab, poolContainer);
                    toSpawn.SetActive(false);
                    PoolDictionary[tag].Enqueue(toSpawn);
                    Debug.Log("se creo un nuevo prefab");
                    break;
                }
            }
        }
        GameObject objectToSpawn = PoolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        

        return objectToSpawn;
    }

    public void Enqueue(string tag, GameObject toEnqueue)
    {
        var poolContainer = transform.Find(tag);
        if (!poolContainer)
        {
            poolContainer = new GameObject(tag).transform;
            poolContainer.SetParent(transform, true);
        }
        toEnqueue.transform.parent = poolContainer;
        toEnqueue.SetActive(false);
        PoolDictionary[tag].Enqueue(toEnqueue);
    }

}
