using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TankEnemyController : MonoBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public TankNav _navigation;
    //public float _roamingRadius;
    //public Transform _player;
    [HideInInspector] public GameObject[] _players;
    public TankEnemyState _currentState;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;
    public bool _playerShot = false;

    public PooledObject _pooledTank;

    //public UnityEvent _tankDie;
    //public Action _tankDie;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _pooledTank = GetComponent<PooledObject>();
        _agent.enabled = false;
        _currentState = new TankEnemyIdleState(this);
        _navigation = new TankNav(this, _patrolingPoint);
        _currentState.OnStateEnter();
        //InitializePool();
        //_tankDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
    }

    /*
    private void InitializePool()
    {
        ObjectPool.Singleton.AddPrefab(_bombPrefab, 5);
    }
    */

    // Update is called once per frame
    void Update()
    {
        _currentState.OnStateUpdate();
    }


    public void ChangeState(TankEnemyState state)
    {
        _currentState.OnStateExit();
        _currentState = state;
        _currentState.OnStateEnter();
    }

    public void Die()
    {
        //_tankDie();
        ObjectPool.Singleton.ReturnPooledObject(this._pooledTank, GameManager.GetInstance().GetSpawner()._tankPrefab);
    }

    public void PlayerStartShooting()
    {
        _playerShot = true;
    }

    /*
    private void OnDisable()
    {
        _tankDie -= GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        GameManager.GetInstance().GetSpawner()._onPlayerShot -= PlayerStartShooting;
    }
    */
}
