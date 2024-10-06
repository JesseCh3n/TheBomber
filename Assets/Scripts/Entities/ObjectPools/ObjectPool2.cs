using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool2 : MonoBehaviour
{

    private static ObjectPool2 _instance;

    public static ObjectPool2 Singleton { get { return _instance; } }

    [SerializeField] List<PoolConfigObject> _pooledPrefabsList;

    Dictionary<GameObject, Queue<PooledObject2>> _pooledObject2s = new Dictionary<GameObject, Queue<PooledObject2>>();

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

    public PooledObject2 GetpooledObject2(GameObject prefab)
    {
        return GetpooledObject2Internal(prefab, Vector3.zero, Quaternion.identity);
    }

    public PooledObject2 GetpooledObject2(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GetpooledObject2Internal(prefab, position, rotation);
    }

    public void ReturnpooledObject2(PooledObject2 pooledObject2, GameObject prefab)
    {
        var go = pooledObject2.gameObject;
        go.SetActive(false);
        _pooledObject2s[prefab].Enqueue(pooledObject2);
    }

    public void AddPrefab(GameObject prefab, int prewarmCount = 0)
    {
        RegisterPrefabInternal(prefab, prewarmCount);
    }

    private void RegisterPrefabInternal (GameObject prefab, int prewarmCount)
    {
        var prefabQueue = new Queue<PooledObject2>();
        _pooledObject2s[prefab] = prefabQueue;
        for (int i = 0; i< prewarmCount; i++)
        {
            var go = Instantiate(prefab);
            PooledObject2 pooledGo = go.GetComponent<PooledObject2>();
            pooledGo.SetObjectPool(prefab, this);
            ReturnpooledObject2(pooledGo, prefab);
            //go.SetActive(false);
        }
    }

    private PooledObject2 GetpooledObject2Internal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var queue = _pooledObject2s[prefab];

        PooledObject2 pooledObject2;
        if (queue.Count > 0)
        {
            Debug.Log("queue is greater than 0");
            Debug.Log(_pooledObject2s[prefab].Count);
            pooledObject2 = queue.Dequeue();
        }
        else
        {
            pooledObject2 = Instantiate(prefab).GetComponent<PooledObject2>();
        }

        var go = pooledObject2.gameObject;
        go.SetActive(true);

        go.transform.position = position;
        go.transform.rotation = rotation;

        return pooledObject2;
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
        _pooledObject2s.Clear();
    }

    [Serializable]
    struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }
}
