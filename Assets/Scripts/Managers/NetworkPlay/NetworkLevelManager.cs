using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Events;
using Unity.Netcode;

public class NetworkLevelManager : NetworkBehaviour
{
    [SerializeField] public Level lvl;
    [SerializeField] private bool _isFinalLevel;
    [SerializeField] private bool _isFirstLevel;

    public UnityEvent _onLevelStart;
    public UnityEvent _onLevelEnd;
    public Action _onAllPlayersDie;

    //network variable
    public NetworkVariable<int> _enemyKilled = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public NetworkVariable<int> _totalEnemy = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);


    private void Start()
    {
        _onAllPlayersDie += GameOverServer;
    }

    public void StartLevel()
    {

        if (IsServer)
        {
            UpdateParamsClientRpc(lvl._totalBomb, lvl._totalRocket, lvl._secondChance, lvl._playerHealth, lvl._playerSpeed);
            NetworkGameManager.GetInstance()._levelName.OnValueChanged += NetworkGameManager.GetInstance().OnLevelChangedSpawnEnemies;
            
            NetworkGameManager.GetInstance()._totalCannons = lvl._cannonNum;
            NetworkGameManager.GetInstance()._totalTanks = lvl._tankNum;
            NetworkGameManager.GetInstance()._totalTrucks = lvl._truckNum;
            NetworkGameManager.GetInstance()._totalBosses = lvl._bossNum;

            NetworkGameManager.GetInstance()._levelName.Value = lvl._levelName;

            _enemyKilled.Value = 0;
            _totalEnemy.Value = lvl._cannonNum + lvl._tankNum + lvl._truckNum + lvl._bossNum;

            _onLevelStart?.Invoke();
        }
    }

    [ClientRpc]
    public void UpdateParamsClientRpc(int bombs, int rockets, int undo, float playerHealth, float playerSpeed)
    {
        NetworkGameManager.GetInstance()._levelName.OnValueChanged += NetworkGameManager.GetInstance().OnLevelChanged;

        NetworkGameManager.GetInstance()._totalBombs = bombs;
        NetworkGameManager.GetInstance()._totalRockets = rockets;
        NetworkGameManager.GetInstance()._totalUndo = undo;
        NetworkGameManager.GetInstance()._playerHealth = playerHealth;
        NetworkGameManager.GetInstance()._playerSpeed = playerSpeed;
    }

    public void GameOverServer()
    {
        if (IsServer)
        {
            NetworkGameManager.GetInstance().ChangeState(NetworkGameManager.GameState.GameOver, this);
        }
    }

    public void ExecuteLevel()
    {
          //Level up when all enemies are killed.
    }

    public void EndLevel()
    {
        if (IsServer)
        {
            UnsubsribeClientRpc();
            NetworkGameManager.GetInstance()._levelName.OnValueChanged -= NetworkGameManager.GetInstance().OnLevelChangedSpawnEnemies;
            _onLevelEnd?.Invoke();

            if (_isFinalLevel)
            {
                NetworkGameManager.GetInstance().ChangeState(NetworkGameManager.GameState.GameEnd, this);
            }
            else
            {
                NetworkGameManager.GetInstance().ChangeState(NetworkGameManager.GameState.LevelEnd, this);
            }
        }
    }


    [ClientRpc]
    public void UnsubsribeClientRpc()
    {
        NetworkGameManager.GetInstance()._levelName.OnValueChanged -= NetworkGameManager.GetInstance().OnLevelChanged;
    }

    public void UpdateEnemyNum()
    {
        Debug.Log("Enemy update executed");
        _enemyKilled.Value += 1;
        if (_enemyKilled.Value == _totalEnemy.Value)
        {
            EndLevel();
        }
    }

}
