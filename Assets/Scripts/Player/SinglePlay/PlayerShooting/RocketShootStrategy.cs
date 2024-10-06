using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShootStrategy : IshootStrategy
{
    PlayerShoot _playerShoot;
    Transform _shootPoint;
    PooledObject _pooledRocket;

    public RocketShootStrategy(PlayerShoot shoot)
    {
        _playerShoot = shoot;
        _shootPoint = _playerShoot.GetShootPoint();
    }

    public void Shoot()
    {
        if(_playerShoot.GetRocketNum() > 0)
        {
            //_pooledRocket = ObjectPool.Singleton.GetPooledObject(_playerShoot._rocketPrefab, _shootPoint.position, _shootPoint.rotation);
            _pooledRocket = GameManager.GetInstance().GetSpawner()._rocketPool.GetPooledObject();
            _pooledRocket.transform.position = _shootPoint.position;
            _pooledRocket.transform.rotation = _shootPoint.rotation;
            _pooledRocket.gameObject.SetActive(true);
            Rigidbody rocket = _pooledRocket.GetComponent<Rigidbody>();
            //rocket.velocity = _shootPoint.forward * (_playerShoot.GetShootVelocity() + _pooledRocket.GetComponent<ProjectileInteract>()._shootVelocity);
            rocket.AddForce(rocket.transform.forward * (Mathf.Max(0, _playerShoot.GetShootVelocity()) + _pooledRocket.GetComponent<ProjectileInteract>()._shootVelocity), ForceMode.VelocityChange);
            _playerShoot.DeductRocketNum();
            _pooledRocket.Destroy(5f);
        }
    }

    public void Undo()
    {
        _playerShoot.DeductUndoChance();
        _playerShoot.IncreaseRocketNum();
    }
}
