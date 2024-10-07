using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class NetworkBossEnemyController : NetworkBehaviour, IDestroyable
{
    public Transform[] _patrolingPoint;
    public NavMeshAgent _agent;
    public NetworkBossNav _navigation;
    [HideInInspector] public GameObject[] _players;
    public Transform _shootPosition;
    public GameObject _bombPrefab;
    public float _bulletVelocity;
    public float _shootingRate;

    private Transform _playerTransform;
    private float _timer;
    private bool _playerToBeFound = false;

    //public UnityEvent _tankDie;
    //public Action _bossDie;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _navigation = new NetworkBossNav(this, _patrolingPoint);
        //_bossDie += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
        _timer = _shootingRate;
        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            _navigation.FreeRoaming();

            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                _playerToBeFound = true;
            }
            else if (_timer <= 0)
            {
                if(_playerToBeFound == true)
                {
                    FindPlayer();
                }
                if (_playerTransform != null)
                {
                    _playerToBeFound = false;
                    _agent.isStopped = true;
                    Rotate(_playerTransform);
                    Shoot();
                    _agent.isStopped = false;
                    _playerTransform = null;
                }
                _timer = _shootingRate;
            }
        }
    }

    public void Die()
    {
        NetworkObject netObj = this.gameObject.GetComponent<NetworkObject>();
        NetworkObjectPool.Singleton.ReturnNetworkObject(netObj, NetworkGameManager.GetInstance().GetSpawner()._bossPrefab);
        if (netObj.IsSpawned)
        {
            netObj.Despawn(false);
        }
    }

    public void Shoot()
    {
        NetworkObject pooledBullet = NetworkObjectPool.Singleton.GetNetworkObject(_bombPrefab, _shootPosition.position, _shootPosition.rotation);
        if (!pooledBullet.IsSpawned)
        {
            pooledBullet.Spawn(true);
        }
        Rigidbody bullet = pooledBullet.GetComponent<Rigidbody>();
        bullet.AddForce(bullet.transform.forward * _bulletVelocity, ForceMode.VelocityChange);
        StartCoroutine(ObjectCountDown(pooledBullet));
    }

    public void Rotate(Transform target)
    {
        gameObject.transform.LookAt(target.position);
    }

    public void FindPlayer()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        if (_players.Length == 0) return;
        int randIndex = UnityEngine.Random.Range(0, _players.Length);
        _playerTransform = _players[randIndex].transform;
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
