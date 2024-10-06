using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public ObjectPool _cannonPool;
    public ObjectPool _tankPool;
    public ObjectPool _truckPool;
    public ObjectPool _bossPool;
    public ObjectPool _bombPool;
    public ObjectPool _rocketPool;
    public ObjectPool _bulletPool;
    public ObjectPool _smallExplosionPool;
    public ObjectPool _bigExplosionPool;

    public GameObject _playerPrefab;

    public Transform[] _cannonSpawnPoint;
    public Transform[] _truckSpawnPoint;
    public Transform[] _navPoint;

    public bool _playerSpawned = false;

    private GameObject _player;


    public Action _onPlayerShot;

    private void Awake()
    {
        if (_player != null)
        {
            Destroy(_player);
        }
    }


    public void SpawnCannon(int num)
    {
        for(int i=0; i<num; i++)
        {
            int randInt = UnityEngine.Random.Range(0, _cannonSpawnPoint.Length);
            Vector3 randomSpawnPosition = _cannonSpawnPoint[randInt].position;
            //Instantiate(_cannonPrefab, randomSpawnPosition, Quaternion.identity);
            PooledObject pooledCannon = _cannonPool.GetPooledObject();
            pooledCannon.transform.position = randomSpawnPosition;
            pooledCannon.transform.rotation = Quaternion.identity;
            pooledCannon.gameObject.SetActive(true);
            _onPlayerShot += pooledCannon.gameObject.GetComponent<CannonEnemyController>().PlayerStartShooting;
        }
    }


    public void SpawnTank(int num)
    {

        NavMeshHit Hit;

        for (int i = 0; i < num; i++)
        {
            int VertexIndex = UnityEngine.Random.Range(0, GameManager.GetInstance()._triangulation.vertices.Length);
            //PooledObject pooledTank = ObjectPool.Singleton.GetPooledObject(_tankPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
            PooledObject pooledTank = _tankPool.GetPooledObject();
            pooledTank.transform.position = new Vector3(-100, -100, -100);
            pooledTank.transform.rotation = Quaternion.identity;
            pooledTank.gameObject.SetActive(true);
            GameObject tank = pooledTank.gameObject;
            if (NavMesh.SamplePosition(GameManager.GetInstance()._triangulation.vertices[VertexIndex], out Hit, 2f, -1))
            {
                tank.GetComponent<NavMeshAgent>().Warp(Hit.position);
                tank.GetComponent<NavMeshAgent>().enabled = true;
            }
            _onPlayerShot += tank.GetComponent<TankEnemyController>().PlayerStartShooting;
            for (int j = 0; j < _navPoint.Length; j++)
            {
                tank.GetComponent<TankEnemyController>()._patrolingPoint = _navPoint;
            }            
        }
    }
    public void SpawnTruck(int num)
    {
        for (int i = 0; i < num; i++)
        {
            int randInt = UnityEngine.Random.Range(0, _truckSpawnPoint.Length);
            Vector3 randomSpawnPosition = _truckSpawnPoint[randInt].position;
            //Instantiate(_truckPrefab, randomSpawnPosition, Quaternion.identity);
            //PooledObject pooledTruck = ObjectPool.Singleton.GetPooledObject(_truckPrefab, randomSpawnPosition, Quaternion.identity);
            PooledObject pooledTruck = _truckPool.GetPooledObject();
            pooledTruck.transform.position = randomSpawnPosition;
            pooledTruck.transform.rotation = Quaternion.identity;
            pooledTruck.gameObject.SetActive(true);
            _onPlayerShot += pooledTruck.gameObject.GetComponent<TruckEnemyController>().PlayerStartShooting;
            for (int j = 0; j < _navPoint.Length; j++)
            {
                pooledTruck.gameObject.GetComponent<TruckEnemyController>()._patrolingPoint = _navPoint;
            }
        }
    }

    public void SpawnBoss(int num)
    {

        NavMeshHit Hit;

        for (int i = 0; i < num; i++)
        {
            int VertexIndex = UnityEngine.Random.Range(0, GameManager.GetInstance()._triangulation.vertices.Length);
            //GameObject boss = Instantiate(_bossPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
            //PooledObject pooledBoss = ObjectPool.Singleton.GetPooledObject(_bossPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
            PooledObject pooledBoss = _bossPool.GetPooledObject();
            pooledBoss.transform.position = new Vector3(-100, -100, -100);
            pooledBoss.transform.rotation = Quaternion.identity;
            pooledBoss.gameObject.SetActive(true);
            GameObject boss = pooledBoss.gameObject;
            if (NavMesh.SamplePosition(GameManager.GetInstance()._triangulation.vertices[VertexIndex], out Hit, 2f, -1))
            {
                boss.GetComponent<NavMeshAgent>().Warp(Hit.position);
            }
            for (int j = 0; j < _navPoint.Length; j++)
            {
                boss.GetComponent<BossEnemyController>()._patrolingPoint = _navPoint;
            }
        }
    }

    public void SpawnPlayer(int bombNum, int rocketNum, int undoNum, float playerSpeed, float playerHealth)
    {
        float randHeight = UnityEngine.Random.Range(18f, 22f);
        Vector3 randomSpawnPosition = new Vector3(UnityEngine.Random.Range(20, 80), randHeight, UnityEngine.Random.Range(20, 80));
        if (_player != null)
        {
            Destroy(_player.gameObject);
            _player = Instantiate(_playerPrefab, randomSpawnPosition, Quaternion.identity);
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._bulletNum = bombNum;
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._rocketNum = rocketNum;
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._undoChance = undoNum;
            _player.gameObject.GetComponentInChildren<PlayerMovement>()._playerConstantSpeed = playerSpeed;
            _player.gameObject.GetComponentInChildren<Health>()._maxHealth = playerHealth;

            _playerSpawned = true;
        }
        else
        {
            _player = Instantiate(_playerPrefab, randomSpawnPosition, Quaternion.identity);
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._bulletNum = bombNum;
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._rocketNum = rocketNum;
            _player.gameObject.GetComponentInChildren<PlayerShoot>()._undoChance = undoNum;
            _player.gameObject.GetComponentInChildren<PlayerMovement>()._playerConstantSpeed = playerSpeed;
            _player.gameObject.GetComponentInChildren<Health>()._maxHealth = playerHealth;

            _playerSpawned = true;
        }
    }

    public void SetPlayer(GameObject obj, int bombNum, int rocketNum, int undoNum, float playerSpeed, float playerHealth)
    {
        Debug.Log("set player");
        obj.transform.position = new Vector3(UnityEngine.Random.Range(20, 80), UnityEngine.Random.Range(18f, 22f), UnityEngine.Random.Range(20, 80));
        obj.GetComponentInChildren<PlayerController>().GetPlayerShoot().SetBulletNum(bombNum);
        obj.GetComponentInChildren<PlayerController>().GetPlayerShoot().SetRocektNum(rocketNum);
        obj.GetComponentInChildren<PlayerController>().GetPlayerShoot().SetUndoChance(undoNum);
        obj.GetComponentInChildren<PlayerController>().GetPlayerMovement().SetPlayerSpeed(playerSpeed);
        obj.GetComponentInChildren<Health>()._maxHealth = playerHealth;
        obj.GetComponentInChildren<Health>().OnReset();
    }

    public GameObject GetPlayerInstance()
    {
        if (_playerSpawned)
        {
            return _player;
        }
        return null;
    }

    public void PlayerShot()
    {
        _onPlayerShot();
    }

}
