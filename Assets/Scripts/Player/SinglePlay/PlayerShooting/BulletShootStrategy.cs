using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShootStrategy : IshootStrategy
{
    PlayerShoot _playerShoot;
    Transform _shootPoint;
    PooledObject _pooledBullet;

    public BulletShootStrategy(PlayerShoot shoot)
    {
        _playerShoot = shoot;
        _shootPoint = shoot.GetShootPoint();
    }

    public void Shoot()
    {
        if (_playerShoot.GetBulletNum() > 0)
        {
            _pooledBullet = ObjectPool.Singleton.GetPooledObject(_playerShoot._bulletPrefab, _shootPoint.position, _shootPoint.rotation);
            Rigidbody bullet = _pooledBullet.GetComponent<Rigidbody>();
            //bullet.velocity = _shootPoint.forward * (_playerShoot.GetShootVelocity() + _pooledBullet.GetComponent<ProjectileInteract>()._shootVelocity);
            bullet.AddForce(bullet.transform.forward * (Mathf.Max(0, _playerShoot.GetShootVelocity()) + _pooledBullet.GetComponent<ProjectileInteract>()._shootVelocity), ForceMode.VelocityChange);
            _playerShoot.DeductBulletNum();
            _pooledBullet.Destroy(5f);
        }
    }

    public void Undo()
    {
        _playerShoot.DeductUndoChance();
        _playerShoot.IncreaseBulletNum();
    }
}
