using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.AI;
using Unity.Netcode;
using TMPro;

public class CoopSpawnManager : NetworkBehaviour
{
    public GameObject _cannonPrefab;
    public GameObject _tankPrefab;
    public GameObject _truckPrefab;
    public GameObject _bossPrefab;
    public GameObject _bombPrefab;
    public GameObject _smallExplosionPrefab;
    public GameObject _bigExplosionPrefab;
    public GameObject _bulletPrefab;
    public GameObject _rocketPrefab;
    public Transform[] _cannonSpawnPoint;
    public Transform[] _truckSpawnPoint;
    public Transform[] _playerSpawnPoint;
    public Transform[] _navPoint;


    public Action _onPlayerShot;

    public void SpawnCannon(int num)
    {
        if (IsServer)
        {
            for (int i = 0; i < num; i++)
            {
                int randInt = UnityEngine.Random.Range(0, _cannonSpawnPoint.Length);
                Vector3 randomSpawnPosition = _cannonSpawnPoint[randInt].position;
                NetworkObject pooledCannon = NetworkObjectPool.Singleton.GetNetworkObject(_cannonPrefab, randomSpawnPosition, Quaternion.identity);
                if (!pooledCannon.IsSpawned)
                {
                    pooledCannon.Spawn(true);
                }
                _onPlayerShot += pooledCannon.gameObject.GetComponent<NetworkCannonEnemyController>().PlayerStartShooting;
            }
        }
    }

    public void SpawnTank(int num)
    {
        if (IsServer)
        {
            NavMeshHit Hit;

            for (int i = 0; i < num; i++)
            {
                int VertexIndex = UnityEngine.Random.Range(0, NetworkGameManager.GetInstance()._triangulation.vertices.Length);
                NetworkObject pooledTank = NetworkObjectPool.Singleton.GetNetworkObject(_tankPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
                if (!pooledTank.IsSpawned)
                {
                    pooledTank.Spawn(true);
                }
                GameObject tank = pooledTank.gameObject;
                if (NavMesh.SamplePosition(NetworkGameManager.GetInstance()._triangulation.vertices[VertexIndex], out Hit, 2f, -1))
                {
                    tank.GetComponent<NavMeshAgent>().Warp(Hit.position);
                    tank.GetComponent<NavMeshAgent>().enabled = true;
                }
                _onPlayerShot += tank.GetComponent<NetworkTankEnemyController>().PlayerStartShooting;
                for (int j = 0; j < _navPoint.Length; j++)
                {
                    tank.GetComponent<NetworkTankEnemyController>()._patrolingPoint = _navPoint;
                }
            }
        }
    }

    public void SpawnTruck(int num)
    {
        if (IsServer)
        {
            for (int i = 0; i < num; i++)
            {
                int randInt = UnityEngine.Random.Range(0, _truckSpawnPoint.Length);
                Vector3 randomSpawnPosition = _truckSpawnPoint[randInt].position;
                NetworkObject pooledTruck = NetworkObjectPool.Singleton.GetNetworkObject(_truckPrefab, randomSpawnPosition, Quaternion.identity);
                if (!pooledTruck.IsSpawned)
                {
                    pooledTruck.Spawn(true);
                }
                _onPlayerShot += pooledTruck.gameObject.GetComponent<NetworkTruckEnemyController>().PlayerStartShooting;
                for (int j = 0; j < _navPoint.Length; j++)
                {
                    pooledTruck.gameObject.GetComponent<NetworkTruckEnemyController>()._patrolingPoint = _navPoint;
                }
            }
        }
    }

    public void SpawnBoss(int num)
    {
        if (IsServer)
        {
            NavMeshHit Hit;

            for (int i = 0; i < num; i++)
            {
                int VertexIndex = UnityEngine.Random.Range(0, NetworkGameManager.GetInstance()._triangulation.vertices.Length);
                NetworkObject pooledBoss = NetworkObjectPool.Singleton.GetNetworkObject(_bossPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
                if (!pooledBoss.IsSpawned)
                {
                    pooledBoss.Spawn(true);
                }
                GameObject boss = pooledBoss.gameObject;
                if (NavMesh.SamplePosition(NetworkGameManager.GetInstance()._triangulation.vertices[VertexIndex], out Hit, 2f, -1))
                {
                    boss.GetComponent<NavMeshAgent>().Warp(Hit.position);
                }
                for (int j = 0; j < _navPoint.Length; j++)
                {
                    boss.GetComponent<NetworkBossEnemyController>()._patrolingPoint = _navPoint;
                }
            }
        }
    }

    public void PlayerShot()
    {
        _onPlayerShot();
    }

}
