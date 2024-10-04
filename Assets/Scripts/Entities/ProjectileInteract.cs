using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInteract : MonoBehaviour
{
    private PooledObject _pooledProjectile;
    [SerializeField] float _damage;
    [SerializeField] public float _shootVelocity;

    public Action _onEnemyHit;

    // Start is called before the first frame update
    void Start()
    {
        _pooledProjectile = GetComponent<PooledObject>();
        _onEnemyHit += GameManager.GetInstance().GetScoreManager().IncrementScore;
        _onEnemyHit += GameManager.GetInstance().GetCurrentLevel().UpdateEnemyNum;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Health damageable = other.gameObject.GetComponent<Health>();
            damageable.DeductHealth(_damage);
            _pooledProjectile.Destroy(1f);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            _onEnemyHit();
            other.gameObject.GetComponent<IDestroyable>().Die();
            _pooledProjectile.Destroy(1f);
        }
    }

}
