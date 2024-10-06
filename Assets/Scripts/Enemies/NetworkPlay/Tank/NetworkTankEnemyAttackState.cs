using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkTankEnemyAttackState : NetworkTankEnemyState
{
    private float _timer;
    private Transform _playerTransform;

    public NetworkTankEnemyAttackState(NetworkTankEnemyController enemy) : base(enemy) { }
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
        _playerTransform = null;
        _enemy._players = GameObject.FindGameObjectsWithTag("Player");
        if (_enemy._players.Length == 0) return;
        int randIndex = Random.Range(0, _enemy._players.Length);
        Debug.Log("The player index enemy found is " + randIndex);
        _playerTransform = _enemy._players[randIndex].transform;
    }


    public override void OnStateEnter()
    {
        FindPlayer();
        _timer = _enemy._shootingRate;
        //Debug.Log("Tank enemy is attacking the player");
    }

    public override void OnStateExit()
    {
        //Debug.Log("Tank enemy Stopped attacking the Player");
    }

    public override void OnStateUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else if (_timer <= 0)
        {
            FindPlayer();
            if (_playerTransform != null)
            {
                Rotate(_playerTransform);
                Shoot();
            }
            _timer = _enemy._shootingRate;
        }
    }
}
