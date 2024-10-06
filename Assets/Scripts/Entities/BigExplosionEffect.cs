using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigExplosionEffect : MonoBehaviour
{
    //GameObject _explosion;
    PooledObject _pooledExplosion;
    AudioSource _audioSource;
    private bool _isExploded = false;

    /*
    private void Start()
    {
        _explosion = GameManager.GetInstance().GetSpawner()._bigExplosionPrefab;
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (_isExploded)
        {
            return;
        }
        else
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Ground"))
            {
                var collisionPoint = other.ClosestPointOnBounds(transform.position);
                //_pooledExplosion = ObjectPool.Singleton.GetPooledObject(_explosion, collisionPoint, other.transform.rotation);
                _pooledExplosion = GameManager.GetInstance().GetSpawner()._bigExplosionPool.GetPooledObject();
                _pooledExplosion.transform.position = collisionPoint;
                _pooledExplosion.transform.rotation = other.transform.rotation;
                _pooledExplosion.gameObject.SetActive(true);
                _isExploded = true;
                _audioSource = _pooledExplosion.gameObject.GetComponent<AudioSource>();
                _audioSource.Play();
                _pooledExplosion.Destroy(2f);
            }
        }
    }

    private void OnDisable()
    {
        _isExploded = false;
    }
}
