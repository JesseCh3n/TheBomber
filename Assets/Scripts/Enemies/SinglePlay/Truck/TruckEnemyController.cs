using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TruckEnemyController : MonoBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public TruckNav _navigation;
    //public float _roamingRadius;
    public TruckEnemyState _currentState;
    public bool _playerShot = false;

    public PooledObject _pooledTruck;

    //public UnityEvent _truckDie;
    //public Action _truckDie;


    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _pooledTruck = GetComponent<PooledObject>();
        _agent.enabled = false;
        _currentState = new TruckEnemyIdleState(this);
        _navigation = new TruckNav(this, _patrolingPoint);
        _currentState.OnStateEnter();
        //_truckDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
    }


    // Update is called once per frame
    void Update()
    {
        _currentState.OnStateUpdate();
    }


    public void ChangeState(TruckEnemyState state)
    {
        _currentState.OnStateExit();
        _currentState = state;
        _currentState.OnStateEnter();
    }

    public void Die()
    {
        gameObject.GetComponent<PooledObject>().Destroy();
        //_truckDie?.Invoke();
    }
    public void PlayerStartShooting()
    {
        _playerShot = true;
    }

    /*
    private void OnDisable()
    {
        _truckDie -= GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        GameManager.GetInstance().GetSpawner()._onPlayerShot -= PlayerStartShooting;
    }
    */
}
