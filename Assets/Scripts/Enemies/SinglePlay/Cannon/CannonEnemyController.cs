using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CannonEnemyController : MonoBehaviour, IDestroyable
{
    //public Transform _player;
    [HideInInspector] public GameObject[] _players;
    public CannonEnemyState _currentState;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;
    public bool _playerShot = false;

    public PooledObject _pooledCannon;

    //public UnityEvent _cannonDie = new UnityEvent();
    //public Action _cannonDie;


    // Start is called before the first frame update
    void Start()
    {
        _pooledCannon = GetComponent<PooledObject>();
        _currentState = new CannonEnemyIdleState(this);
        _currentState.OnStateEnter();
        //InitializePool();
        //_cannonDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
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

    public void ChangeState(CannonEnemyState state)
    {
        _currentState.OnStateExit();
        _currentState = state;
        _currentState.OnStateEnter();
    }

    public void Die()
    {
        //_cannonDie();
        ObjectPool.Singleton.ReturnPooledObject(this._pooledCannon, GameManager.GetInstance().GetSpawner()._cannonPrefab);
    }

    public void PlayerStartShooting()
    {
        _playerShot = true;
    }

    /*
    private void OnDisable()
    {
        _cannonDie -= GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        GameManager.GetInstance().GetSpawner()._onPlayerShot -= PlayerStartShooting;
    }
    */
}
