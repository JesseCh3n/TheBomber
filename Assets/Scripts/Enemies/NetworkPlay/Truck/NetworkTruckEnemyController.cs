using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NetworkTruckEnemyController : NetworkBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public NetworkTruckNav _navigation;
    //public float _roamingRadius;
    public NetworkTruckEnemyState _currentState;
    public bool _playerShot = false;

    //public UnityEvent _truckDie;


    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            _currentState = new NetworkTruckEnemyIdleState(this);
            _navigation = new NetworkTruckNav(this, _patrolingPoint);
            _currentState.OnStateEnter();
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


    public void ChangeState(NetworkTruckEnemyState state)
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
            NetworkObjectPool.Singleton.ReturnNetworkObject(netObj, NetworkGameManager.GetInstance().GetSpawner()._truckPrefab);
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

}
