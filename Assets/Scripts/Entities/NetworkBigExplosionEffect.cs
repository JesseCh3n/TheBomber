using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkBigExplosionEffect : NetworkBehaviour
{
    GameObject _explosionPrefab;
    NetworkObject _pooledExplosion;
    AudioSource _audioSource;
    private bool _isExploded = false;

    private void Start()
    {
        _explosionPrefab = NetworkGameManager.GetInstance().GetSpawner()._bigExplosionPrefab;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Ground"))
        {
            var collisionPoint = other.ClosestPointOnBounds(transform.position);
            var collisionRotation = other.transform.rotation;
            SpawnBigExplosion(collisionPoint, collisionRotation);
        }
    }

    private void SpawnBigExplosion(Vector3 point, Quaternion rotation)
    {
        if (IsServer)
        {
            if (_isExploded)
            {
                return;
            }
            else
            {
                _pooledExplosion = NetworkObjectPool.Singleton.GetNetworkObject(_explosionPrefab, point, rotation);
                if (!_pooledExplosion.IsSpawned)
                {
                    _pooledExplosion.Spawn(true);
                }
                _isExploded = true;
                _audioSource = _pooledExplosion.gameObject.GetComponent<AudioSource>();
                _audioSource.Play();
                StartCoroutine(DespawnCountDown());
            }
        }
    }

    private IEnumerator DespawnCountDown()
    {
        yield return new WaitForSeconds(1f);
        NetworkObjectPool.Singleton.ReturnNetworkObject(_pooledExplosion, _explosionPrefab);
        if (_pooledExplosion.IsSpawned)
        {
            _pooledExplosion.Despawn(false);
        }
        _isExploded = false;
    }
}
