using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IDestroyable
{
    private Health _playerHealth;
    private PlayerMovement _playerMovement;
    private PlayerRotation _playerRotation;
    private PlayerShoot _playerShoot;

    private static PlayerController _instance;

    //public UnityEvent _playerDie;

    public static PlayerController GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerHealth = gameObject.GetComponent<Health>();
        _playerMovement = gameObject.GetComponent<PlayerMovement>();
        _playerRotation = gameObject.GetComponent<PlayerRotation>();
        _playerShoot = gameObject.GetComponent<PlayerShoot>();
        _playerHealth._onHealthUpdated += GameManager.GetInstance().GetUIManager().UpdateHealth;
        _playerHealth._onDeath += GameManager.GetInstance().GetCurrentLevel().GameOver;

        _playerHealth.OnStart();
    }

    // Update is called once per frame

    public void Die()
    {
        //this.gameObject.SetActive(false);
        PlayerInput.GetInstance()._isDiabled = true;
    }

    public Health GetPlayerHealth()
    {
        return _playerHealth;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return _playerMovement;
    }

    public PlayerRotation GetPlayerRotation()
    {
        return _playerRotation;
    }
    public PlayerShoot GetPlayerShoot()
    {
        return _playerShoot;
    }

    public void OnDisable()
    {
        _playerHealth._onHealthUpdated -= GameManager.GetInstance().GetUIManager().UpdateHealth;
        _playerHealth._onDeath -= GameManager.GetInstance().GetCurrentLevel().GameOver;
    }
}
