using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCannonEnemyAttackState : NetworkCannonEnemyState
{
    private float _timer;
    private Transform _playerTransform;
    private bool _playerToBeFound = false;

    public NetworkCannonEnemyAttackState(NetworkCannonEnemyController enemy) : base(enemy) { }
    public void Shoot()
    {
        NetworkObject pooledBullet = NetworkObjectPool.Singleton.GetNetworkObject(_enemy._bombPrefab, _enemy._shootPosition.position, _enemy._shootPosition.rotation);
        if (!pooledBullet.IsSpawned)
        {
            pooledBullet.Spawn(true);
        }
        Rigidbody bullet = pooledBullet.GetComponent<Rigidbody>();
        bullet.AddForce(bullet.transform.forward * _enemy._bulletVelocity, ForceMode.VelocityChange);
        _enemy.DestroyBomb(pooledBullet);
    }

    public void Rotate(Transform target)
    {
        _enemy.gameObject.transform.LookAt(target.position);
    }

    public void FindPlayer()
    {
        if (_enemy.CheckServer())
        {
            _enemy._players = GameObject.FindGameObjectsWithTag("Player");
            if (_enemy._players.Length == 0) return;
            int randIndex = Random.Range(0, _enemy._players.Length);
            Debug.Log("The player index enemy found is " + randIndex);
            _playerTransform = _enemy._players[randIndex].transform;
        }
    }

    public override void OnStateEnter()
    {
        FindPlayer();
        _timer = _enemy._shootingRate;
        //Debug.Log("Cannon enemy is attacking the player");
    }

    public override void OnStateExit()
    {
        //Debug.Log("Cannon enemy Stopped attacking the Player");
    }

    public override void OnStateUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _playerToBeFound = true;
        }
        else if (_timer <= 0)
        {
            if (_playerToBeFound == true)
            {
                FindPlayer();
            }
            if (_playerTransform != null)
            {
                _playerToBeFound = false;
                Rotate(_playerTransform);
                Shoot();
                _playerTransform = null;
            }
            _timer = _enemy._shootingRate;
        }
    }

}
