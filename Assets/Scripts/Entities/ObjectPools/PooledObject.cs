using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private float _timer;
    private float _destroyTime = 0;
    private bool _setToDestroy = false;

    GameObject _prefab;
    ObjectPool _associatedPool;

    public void SetObjectPool(GameObject prefab, ObjectPool pool)
    {
        _prefab = prefab;
        _associatedPool = pool;
        _timer = 0;
        _destroyTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_setToDestroy)
        {
            _timer += Time.deltaTime;
            if (_timer >= _destroyTime)
            {
                _setToDestroy = false;
                _timer = 0;
                Destroy();
            }
        }
    }
    public void Destroy()
    {
        if (_associatedPool != null)
        {
            _associatedPool.ReturnPooledObject(this, _prefab);
        }
    }

    public void Destroy(float time)
    {
        _setToDestroy = true;
        _destroyTime = time;
    }


}
