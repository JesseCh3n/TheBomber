using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot")]
    public Transform _shootPoint;
    public GameObject _bulletPrefab;
    public GameObject _rocketPrefab;

    public int _bulletNum = 0;
    public int _rocketNum = 0;
    public int _undoChance = 0;
    private IshootStrategy _currentShootStrategy;
    private CommandInvoker _shootCommand;
    private float _playerVelocity;
    private bool _checkEnabled = false;

    //UnityEvent<int> _onSecondChanceUsed;
    //UnityEvent _onShootingStart;

    public Action<int> _onSecondChanceUsed;
    public Action<int> _onBulletShot;
    public Action<int> _onRocketShot;
    public Action _onShotFired;
    public Action _onAmmoRunout;

    // Start is called before the first frame update
    void Start()
    {
        _shootCommand = new CommandInvoker();
        _onSecondChanceUsed += GameManager.GetInstance().GetUIManager().UpdateSecondChance;
        _onBulletShot += GameManager.GetInstance().GetUIManager().UpdateBulletNum;
        _onRocketShot += GameManager.GetInstance().GetUIManager().UpdateRocketNum;
        _onAmmoRunout += GameManager.GetInstance().GetCurrentLevel().GameOver;
        _onAmmoRunout += PlayerController.GetInstance().Die;
        _onShotFired += GameManager.GetInstance().GetSpawner().PlayerShot;
        //InitializePool();
        _onBulletShot(_bulletNum);
        _onRocketShot(_rocketNum);
        _onSecondChanceUsed(_undoChance);
        _checkEnabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (_checkEnabled)
        {
            CheckAmmo();
            if (_currentShootStrategy == null)
            {
                _currentShootStrategy = new BulletShootStrategy(this);
            }
            else if (PlayerInput.GetInstance()._weapon1Pressed)//Select Bullet
            {
                _currentShootStrategy = new BulletShootStrategy(this);
            }
            else if (PlayerInput.GetInstance()._weapon2Pressed)//Select Rocket
            {
                _currentShootStrategy = new RocketShootStrategy(this);
            }

            if (PlayerInput.GetInstance()._primaryShootPressed && _currentShootStrategy != null)
            {
                // require server RPC 
                // get clientID
                _onShotFired();
                ICommand storedCommand = new ShootCommand(_currentShootStrategy);
                _shootCommand.AddCommand(storedCommand);
            }
            else if (PlayerInput.GetInstance()._undoPressed && _undoChance > 0)
            {
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

    public IshootStrategy getShootStrategy()
    {
        return _currentShootStrategy;
    }
    public Transform GetShootPoint()
    {
        return _shootPoint;
    }
    public float GetShootVelocity()
    {
        _playerVelocity = PlayerController.GetInstance().GetPlayerMovement().GetForwardSpeed();
        return _playerVelocity;
    }

    public void CheckAmmo()
    {
        if (GetBulletNum() == 0 && GetRocketNum() == 0 && GetUndoChance() == 0)
        {
            _checkEnabled = false;
            StartCoroutine(GameOverCoroutine());
            gameObject.GetComponentInParent<PlayerController>().GetPlayerHealth()._isDead = true;
        }
    }

    public void SetBulletNum(int num)
    {
        _bulletNum = num;
        _onBulletShot(num);
    }
    public int GetBulletNum()
    {
        return _bulletNum;
    }
    public void DeductBulletNum()
    {
        _bulletNum--;
        _onBulletShot(_bulletNum);
    }
    public void IncreaseBulletNum()
    {
        _bulletNum++;
        _onBulletShot(_bulletNum);
    }

    public void SetRocektNum(int num)
    {
        _rocketNum = num;
        _onRocketShot(_rocketNum);
    }
    public int GetRocketNum()
    {
        return _rocketNum;
    }
    public void DeductRocketNum()
    {
        _rocketNum--;
        _onRocketShot(_rocketNum);
    }
    public void IncreaseRocketNum()
    {
        _rocketNum++;
        _onRocketShot(_rocketNum);
    }

    public void SetUndoChance(int num)
    {
        _undoChance = num;
        _onSecondChanceUsed(_undoChance);
    }
    public int GetUndoChance()
    {
        return _undoChance;
    }
    public void DeductUndoChance()
    {
        _undoChance --;
        _onSecondChanceUsed(_undoChance);
    }

    private void OnDisable()
    {
        _onSecondChanceUsed -= GameManager.GetInstance().GetUIManager().UpdateSecondChance;
        _onBulletShot -= GameManager.GetInstance().GetUIManager().UpdateBulletNum;
        _onRocketShot -= GameManager.GetInstance().GetUIManager().UpdateRocketNum;
        StopCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(2);
        _onAmmoRunout();
    }
}
