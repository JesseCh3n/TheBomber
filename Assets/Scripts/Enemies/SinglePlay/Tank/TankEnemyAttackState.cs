using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemyAttackState : TankEnemyState
{
    private float _timer;
    private Transform _playerTransform;

    public TankEnemyAttackState(TankEnemyController enemy) : base(enemy) { }
    public void Shoot()
    {
        PooledObject pooledBullet = ObjectPool.Singleton.GetPooledObject(_enemy._bombPrefab, _enemy._shootPosition.position, _enemy._shootPosition.rotation);
        Rigidbody bullet = pooledBullet.GetComponent<Rigidbody>();
        bullet.AddForce(bullet.transform.forward * _enemy._bulletVelocity, ForceMode.VelocityChange);
        //ProjectileMotion bullet = pooledBullet.GetComponent<ProjectileMotion>();
        //bullet.SetSpeed(_enemy._bulletVelocity);
        //bullet.SetTarget(_playerTransform);
        pooledBullet.Destroy(5f);
    }

    public void Rotate(Transform target)
    {
        _enemy.gameObject.transform.LookAt(target.position);
    }

    public void FindPlayer()
    {
        _enemy._players = GameObject.FindGameObjectsWithTag("Player");
        int randIndex = Random.Range(0, _enemy._players.Length);
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
        if (_playerTransform == null)
        {
            FindPlayer();
        }
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        } else if (_timer <= 0)
        {
            if (_playerTransform != null)
            {
                Rotate(_playerTransform);
                Shoot();
            }
            _timer = _enemy._shootingRate;
        }
    }

}
