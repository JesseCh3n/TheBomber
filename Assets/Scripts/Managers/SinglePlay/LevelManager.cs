using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public Level lvl;
    [SerializeField] private bool _isFinalLevel;

    public UnityEvent _onLevelStart;
    public UnityEvent _onLevelEnd;

    //network variable
    private int _enemyKilled;
    private int _totalEnemy;

    public void StartLevel()
    {
        _onLevelStart?.Invoke();
        _enemyKilled = 0;
        _totalEnemy = lvl._cannonNum + lvl._tankNum + lvl._truckNum + lvl._bossNum;
        GameManager.GetInstance().GetSpawner().SpawnCannon(lvl._cannonNum);
        GameManager.GetInstance().GetSpawner().SpawnTruck(lvl._truckNum);
        GameManager.GetInstance().GetSpawner().SpawnTank(lvl._tankNum);
        GameManager.GetInstance().GetSpawner().SpawnBoss(lvl._bossNum);
        if (GameManager.GetInstance().GetSpawner()._playerSpawned)
        {
            GameManager.GetInstance().GetSpawner().SetPlayer(GameManager.GetInstance().GetSpawner().GetPlayerInstance(), lvl._totalBomb, lvl._totalRocket, lvl._secondChance, lvl._playerSpeed, lvl._playerHealth);
        } else
        {
            GameManager.GetInstance().GetSpawner().SpawnPlayer(lvl._totalBomb, lvl._totalRocket, lvl._secondChance, lvl._playerSpeed, lvl._playerHealth);
        }
    }

    public void GameOver()
    {
        GameManager.GetInstance().ChangeState(GameManager.GameState.GameOver, this);
    }

    public void ExecuteLevel()
    {
        //Level up when all enemies are killed.

    }

    public void EndLevel()
    {
        _onLevelEnd?.Invoke();

        if (_isFinalLevel)
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.GameEnd, this);
        }
        else
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.LevelEnd, this);
        }
    }

    public void UpdateEnemyNum()
    {
        _enemyKilled += 1;
        if (_enemyKilled == _totalEnemy)
        {
            EndLevel();
        }
    }

}
