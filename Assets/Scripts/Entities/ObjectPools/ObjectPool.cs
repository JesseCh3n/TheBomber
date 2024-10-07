using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public int _startSize;
    public List<PooledObject> _objectPool = new List<PooledObject>();
    public List<PooledObject> _usedPool = new List<PooledObject>();
    private PooledObject _tempObject;
    [SerializeField] private PooledObject _objectToPool;
    // Start is called before the first frame update
    void Start()
    {
        Initialized();
    }
    private void Initialized()
    {
        for (int i = 0; i < _startSize; i++)
        {
            AddNewObject();
        }
    }

    private void AddNewObject()
    {
        _tempObject = Instantiate(_objectToPool, transform).GetComponent<PooledObject>();
        _tempObject.gameObject.SetActive(false);
        _tempObject.SetObjectPool(this);
        _objectPool.Add(_tempObject);
    }
    //Retrieves an Object from the pool
    public PooledObject GetPooledObject()
    {

        if (_objectPool.Count > 0)
        {
            _tempObject = _objectPool[0];
            _usedPool.Add(_tempObject);
            _objectPool.RemoveAt(0);
        }
        else
        {
            AddNewObject();
            _tempObject = GetPooledObject();
        }
        _tempObject.gameObject.SetActive(true);
        if (_tempObject.GetComponent<Rigidbody>() != null)
        {
            _tempObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        return _tempObject;

    }
    public void DestroyPooledObject(PooledObject obj, float time = 0)
    {
        if (time == 0)
        {
            obj.Destroy();
        }
        else
        {
            obj.Destroy(time);
        }
    }

    public void RestoreObject(PooledObject obj)
    {
        if (obj.GetComponent<Rigidbody>() != null)
        {
            obj.GetComponent<Rigidbody>().isKinematic = true;
        }
        obj.gameObject.SetActive(false);
        _usedPool.Remove(obj);
    }
}
