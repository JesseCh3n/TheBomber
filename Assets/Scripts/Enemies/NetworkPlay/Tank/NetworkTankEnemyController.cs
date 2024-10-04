using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NetworkTankEnemyController : NetworkBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public NetworkTankNav _navigation;
    //public float _roamingRadius;
    //public Transform _player;
    [HideInInspector] public GameObject[] _players;
    public NetworkTankEnemyState _currentState;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;
    public bool _playerShot = false;

    //public UnityEvent _tankDie;
    //public Action _tankDie;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            _currentState = new NetworkTankEnemyIdleState(this);
            _navigation = new NetworkTankNav(this, _patrolingPoint);
            _currentState.OnStateEnter();
            //_tankDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            _currentState.OnStateUpdate();
        }
    }

    public void ChangeState(NetworkTankEnemyState state)
    {
        if (IsServer)
        {
            _currentState.OnStateExit();
            _currentState = state;
            _currentState.OnStateEnter();
        }
    }

    public void Die()
    {
        if (IsServer)
        {
            NetworkObject netObj = this.gameObject.GetComponent<NetworkObject>();
            NetworkObjectPool.Singleton.ReturnNetworkObject(netObj, NetworkGameManager.GetInstance().GetSpawner()._tankPrefab);
            if (netObj.IsSpawned)
            {
                netObj.Despawn(false);
            }
        }
    }

    public void PlayerStartShooting()
    {
        _playerShot = true;
    }

    public void DestroyBomb(NetworkObject obj)
    {
        StartCoroutine(ObjectCountDown(obj));
    }

    private IEnumerator ObjectCountDown(NetworkObject obj)
    {
        yield return new WaitForSeconds(5f);
        if (obj.gameObject.activeSelf == true)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(obj, _bombPrefab);
            if (obj.IsSpawned)
            {
                obj.Despawn(false);
            }
        }
    }
}
