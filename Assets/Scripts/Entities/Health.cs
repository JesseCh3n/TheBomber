using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] public float _maxHealth;

    public Action<float> _onHealthUpdated;
    public Action _onDeath;

    private IDestroyable _object;

    public bool _isDead;

    private float _health;

    // Start is called before the first frame update
    public void OnStart()
    {
        _object = gameObject.GetComponent<IDestroyable>();
        _health = _maxHealth;
        _isDead = false;
        if (_onHealthUpdated != null)
        {
            _onHealthUpdated?.Invoke(_maxHealth);
        }
    }

    public void DeductHealth(float value)
    {
        if (_isDead) return;
        _health -= value;

        if (_health <= 0)
        {
            _isDead = true;
            _object.Die();
            _health = 0;
        }
        _onHealthUpdated?.Invoke(_health);
    }

    public void OnReset()
    {
        if (_isDead) return;
        _health = _maxHealth;
        if(_onHealthUpdated != null)
        {
            _onHealthUpdated?.Invoke(_maxHealth);
        }
    }
}
