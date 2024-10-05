using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCannonEnemyController : NetworkBehaviour, IDestroyable
{
    //public Transform _player;
    [HideInInspector] public GameObject[] _players;
    public NetworkCannonEnemyState _currentState;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;
    public bool _playerShot = false;


    //public UnityEvent _cannonDie = new UnityEvent();
    //public Action _cannonDie;


    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            _currentState = new NetworkCannonEnemyIdleState(this);
            _currentState.OnStateEnter();
            //_cannonDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
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

    public void ChangeState(NetworkCannonEnemyState state)
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
            NetworkObjectPool.Singleton.ReturnNetworkObject(netObj, NetworkGameManager.GetInstance().GetSpawner()._cannonPrefab);
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

    public bool CheckServer()
    {
        return IsServer;
    }
}
