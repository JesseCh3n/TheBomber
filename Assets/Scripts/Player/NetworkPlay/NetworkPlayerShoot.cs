
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerShoot : NetworkBehaviour
{
    [Header("Shoot")]
    public Transform _shootPoint;

    public int _bulletNum = 0;
    public int _rocketNum = 0;
    public int _undoChance = 0;
    public ulong _playerID;

    private bool _isReady = false;
    private bool _checkEnabled = false;

    private NetworkPlayerInput _playerInput;
    private NetworkBulletShootStrategy _bulletStrategy;
    private NetworkRocketShootStrategy _rocketStrategy;
    private IshootStrategy _currentShootStrategy;
    private CommandInvoker _shootCommand;
    private NetworkUIManager _uiManager;
    private float _playerVelocity;

    //UnityEvent<int> _onSecondChanceUsed;
    //UnityEvent _onShootingStart;

    public Action<int> _onSecondChanceUsed;
    public Action<int> _onBulletShot;
    public Action<int> _onRocketShot;
    //public Action _onShotFired;
    public Action _onAmmoRunout;


    // Start is called before the first frame update
    void Start()
    {
        _playerID = GetComponentInParent<NetworkPlayerManager>()._playerID;

        _playerInput = GetComponent<NetworkPlayerInput>();
        _bulletStrategy = GetComponent<NetworkBulletShootStrategy>();
        _rocketStrategy = GetComponent<NetworkRocketShootStrategy>();
        _uiManager = GetComponentInParent<NetworkUIManager>();

        _shootCommand = new CommandInvoker();
    }

    public void OnStart()
    {
        if (IsOwner)
        {
            _onSecondChanceUsed += _uiManager.UpdateSecondChance;
            _onBulletShot += _uiManager.UpdateBulletNum;
            _onRocketShot += _uiManager.UpdateRocketNum;
            _onBulletShot(_bulletNum);
            _onRocketShot(_rocketNum);
            _onSecondChanceUsed(_undoChance);
            _onAmmoRunout += gameObject.GetComponentInParent<NetworkPlayerController>().Die;
            
            if(_undoChance != 0)
            {
                _isReady = true;
                _checkEnabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isReady)
        {
            CheckAmmo();
            if (_currentShootStrategy == null)
            {
                _currentShootStrategy = _bulletStrategy;
            }
            else if (_playerInput._weapon1Pressed)//Select Bullet
            {
                _currentShootStrategy = _bulletStrategy;
            }
            else if (_playerInput._weapon2Pressed)//Select Rocket
            {
                _currentShootStrategy = _rocketStrategy;
            }

            if (_playerInput._primaryShootPressed && _currentShootStrategy != null)
            {
                Debug.Log("shoot button pressed " + _playerInput._primaryShootPressed);
                if (IsSpawned)
                {
                    OnPlayerShotServerRpc();
                }
                ICommand storedCommand = new ShootCommand(_currentShootStrategy);
                _shootCommand.AddCommand(storedCommand);
            }
            else if (_playerInput._undoPressed && _undoChance > 0)
            {
                Debug.Log("undo button pressed " + _playerInput._undoPressed);
                if (_undoChance == 0)
                {
                    return;
                }
                else
                {
                    _onSecondChanceUsed(GetUndoChance());
                    _shootCommand.UndoCommand();
                }
            }
        }
    }

    public IshootStrategy GetShootStrategy()
    {
        return _currentShootStrategy;
    }
    public Transform GetShootPoint()
    {
        return _shootPoint;
    }
    public float GetShootVelocity()
    {
        _playerVelocity = gameObject.GetComponent<NetworkPlayerController>().GetPlayerMovement().GetForwardSpeed();
        return _playerVelocity;
    }

    public void CheckAmmo()
    {
        if (_checkEnabled)
        {
            if (GetBulletNum() == 0 && GetRocketNum() == 0 && GetUndoChance() == 0)
            {
                _checkEnabled = false;
                StartCoroutine(GameOverCoroutine());
                gameObject.GetComponentInParent<NetworkPlayerController>().GetPlayerHealth()._isDead = true;
            }
        }
    }

    public void SetBulletNum(int num)
    {
        _bulletNum = num;
        if (IsOwner)
        {
            _onBulletShot(num);
        }
    }
    public int GetBulletNum()
    {
        return _bulletNum;
    }
    public void DeductBulletNum()
    {
        _bulletNum--;
        if (IsOwner)
        {
            _onBulletShot(_bulletNum);
        }
    }
    public void IncreaseBulletNum()
    {
        _bulletNum++;
        if (IsOwner)
        {
            _onBulletShot(_bulletNum);
        }
    }

    public void SetRocketNum(int num)
    {
        _rocketNum = num;
        if (IsOwner)
        {
            _onRocketShot(_rocketNum);
        }
    }

    public int GetRocketNum()
    {
        return _rocketNum;
    }
    public void DeductRocketNum()
    {
        _rocketNum--;
        if (IsOwner)
        {
            _onRocketShot(_rocketNum);
        }
    }

    public void IncreaseRocketNum()
    {
        _rocketNum++;
        if (IsOwner)
        {
            _onRocketShot(_rocketNum);
        }
    }

    public void SetUndoChance(int num)
    {
        _undoChance = num;
        if (IsOwner)
        {
            _onSecondChanceUsed(_undoChance);
        }
    }

    public int GetUndoChance()
    {
        return _undoChance;
    }
    public void DeductUndoChance()
    {
        _undoChance--;
        if (IsOwner)
        {
            _onSecondChanceUsed(_undoChance);
        }
    }

    public void OnDie()
    {
        _onSecondChanceUsed -= _uiManager.UpdateSecondChance;
        _onBulletShot -= _uiManager.UpdateBulletNum;
        _onRocketShot -= _uiManager.UpdateRocketNum;
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(2);
        _onAmmoRunout();
    }

    [ServerRpc]
    private void OnPlayerShotServerRpc()
    {
        NetworkGameManager.GetInstance().GetSpawner().PlayerShot();
    }
}
