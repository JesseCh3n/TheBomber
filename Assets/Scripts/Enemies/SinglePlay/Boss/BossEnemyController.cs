using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BossEnemyController : MonoBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public BossNav _navigation;
    [HideInInspector] public GameObject[] _players;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;

    private Transform _playerTransform;
    private float _timer;

    public PooledObject _pooledBoss;

    //public UnityEvent _tankDie;
    //public Action _bossDie;

    // Start is called before the first frame update
    void Start()
    {
        _pooledBoss = GetComponent<PooledObject>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _navigation = new BossNav(this, _patrolingPoint);
        //_bossDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        _timer = _shootingRate;
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        _navigation.FreeRoaming();
        if (_players == null)
        {
            FindPlayer();
        }
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else if (_timer <= 0)
        {
            if (_playerTransform != null)
            {
                _agent.isStopped = true;
                Rotate(_playerTransform);
                Shoot();
                _agent.isStopped = false;
            }
            _timer = _shootingRate;
        }
    }

    public void Die()
    {
        //_bossDie();
        ObjectPool.Singleton.ReturnPooledObject(this._pooledBoss, GameManager.GetInstance().GetSpawner()._bossPrefab);
    }


    public void Shoot()
    {
        PooledObject pooledBullet = ObjectPool.Singleton.GetPooledObject(_bombPrefab, _shootPosition.position, _shootPosition.rotation);
        Rigidbody bullet = pooledBullet.GetComponent<Rigidbody>();
        bullet.AddForce(bullet.transform.forward * _bulletVelocity, ForceMode.VelocityChange);
        pooledBullet.Destroy(5f);
    }

    public void Rotate(Transform target)
    {
        gameObject.transform.LookAt(target.position);
    }

    public void FindPlayer()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        int randIndex = UnityEngine.Random.Range(0, _players.Length);
        _playerTransform = _players[randIndex].transform;
    }
}
