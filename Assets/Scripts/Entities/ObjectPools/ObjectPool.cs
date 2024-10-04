using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    private static ObjectPool _instance;

    public static ObjectPool Singleton { get { return _instance; } }

    [SerializeField] List<PoolConfigObject> _pooledPrefabsList;

    Dictionary<GameObject, Queue<PooledObject>> _pooledObjects = new Dictionary<GameObject, Queue<PooledObject>>();

    private bool _hasInitialized = false;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Start()
    {
        InitializePool();
    }

    public void OnDisable()
    {
        ClearPool();
    }

    public PooledObject GetPooledObject(GameObject prefab)
    {
        return GetPooledObjectInternal(prefab, Vector3.zero, Quaternion.identity);
    }

    public PooledObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GetPooledObjectInternal(prefab, position, rotation);
    }

    public void ReturnPooledObject(PooledObject pooledObject, GameObject prefab)
    {
        var go = pooledObject.gameObject;
        go.SetActive(false);
        _pooledObjects[prefab].Enqueue(pooledObject);
    }

    public void AddPrefab(GameObject prefab, int prewarmCount = 0)
    {
        RegisterPrefabInternal(prefab, prewarmCount);
    }

    private void RegisterPrefabInternal (GameObject prefab, int prewarmCount)
    {
        var prefabQueue = new Queue<PooledObject>();
        _pooledObjects[prefab] = prefabQueue;
        for (int i = 0; i< prewarmCount; i++)
        {
            var go = Instantiate(prefab);
            PooledObject pooledGo = go.GetComponent<PooledObject>();
            pooledGo.SetObjectPool(prefab, this);
            ReturnPooledObject(pooledGo, prefab);
        }
    }

    private PooledObject GetPooledObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var queue = _pooledObjects[prefab];

        PooledObject pooledObject;
        if (queue.Count > 0)
        {
            pooledObject = queue.Dequeue();
        }
        else
        {
            pooledObject = Instantiate(prefab).GetComponent<PooledObject>();
        }

        var go = pooledObject.gameObject;
        go.SetActive(true);

        go.transform.position = position;
        go.transform.rotation = rotation;

        return pooledObject;
    }

    public void InitializePool()
    {
        if (_hasInitialized) return;
        foreach (var configObject in _pooledPrefabsList)
        {
            RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
        }
        _hasInitialized = true;
    }

    public void ClearPool()
    {
        _pooledObjects.Clear();
    }

    [Serializable]
    struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }
}
