using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    [SerializeField] private NetworkLevelManager[] _levels;
    private NetworkLevelManager _currentLevel;
    [SerializeField] private NetworkScoreManager _scoreManager;
    [SerializeField] private CoopSpawnManager _spawner;
    [SerializeField] private SceneUIManager _sceneManager;
    [SerializeField] private NetworkUIManager _uiManager;
    private int _currentLevelIndex = 0;
    private bool _isInputActive = false;

    public AudioSource musicSource;
    public NavMeshTriangulation _triangulation;

    public UnityEvent _onGameStart;
    public UnityEvent _onGameOver;
    public UnityEvent _onGameEnd;

    public int _totalCannons = 0;
    public int _totalTanks = 0;
    public int _totalTrucks = 0;
    public int _totalBosses = 0;
    public int _totalBombs = 0;
    public int _totalRockets = 0;
    public int _totalUndo = 0;
    public float _playerSpeed = 0f;
    public float _playerHealth = 0f;
    public bool _gameReady = false;

    public Action<string> _levelUpdated;
    public Action<int> _bombUpdated;
    public Action<int> _rocketUpdated;
    public Action<int> _undoUpdated;
    public Action<float> _healthUpdated;
    public Action<float> _speedUpdated;

    public NetworkVariable<int> _totalPlayers = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
 
    public NetworkVariable<FixedString64Bytes> _levelName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("Level"),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    /*
    public NetworkVariable<int> _totalCannons = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalTanks = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalTrucks = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalBosses = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalBombs = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalRockets = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> _totalUndo = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<float> _playerSpeed = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<float> _playerHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> _gameReady = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    */

    public void OnLevelChangedSpawnEnemies(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        if (previousValue != newValue)
        {
            if (IsServer)
            {
                Debug.Log("Client is here 6");
                GetSpawner().SpawnCannon(_totalCannons);
                GetSpawner().SpawnTruck(_totalTrucks);
                GetSpawner().SpawnTank(_totalTanks);
                GetSpawner().SpawnBoss(_totalBosses);
            }
        }
    }

    public void OnLevelChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        if (previousValue != newValue)
        {
            _bombUpdated?.Invoke(_totalBombs);
            _rocketUpdated?.Invoke(_totalRockets);
            _undoUpdated?.Invoke(_totalUndo);
            _healthUpdated?.Invoke(_playerHealth);
            _speedUpdated?.Invoke(_playerSpeed);
            _levelUpdated?.Invoke(newValue.ToString());

            _gameReady = true;
        }
    }

    public bool IsInputActive()
    {
        return _isInputActive;
    }

    private static NetworkGameManager _instance;

    public static NetworkGameManager GetInstance()
    {
        return _instance;
    }

    public NetworkLevelManager GetCurrentLevel()
    {
        return _currentLevel;
    }

    public NetworkScoreManager GetScoreManager()
    {
        return _scoreManager;
    }

    public CoopSpawnManager GetSpawner()
    {
        return _spawner;
    }

    public SceneUIManager GetSceneChanger()
    {
        return _sceneManager;
    }

    public NetworkUIManager GetUIManager()
    {
        return _uiManager;
    }

    public enum GameState
    {
        LevelStart,
        LevelIn,
        LevelEnd,
        GameOver,
        GameEnd
    }

    private GameState _currentState;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void GameStart1()
    {
        if (IsServer)
        {
            Debug.Log("Client is here 2");
            _triangulation = NavMesh.CalculateTriangulation();
            StartCoroutine(WaitUntilTriangulationDone());
        }
    }

    public void GameStart2()
    {
        if (IsServer)
        {
            Debug.Log("Client is here 3");
            ChangeState(GameState.LevelStart, _levels[_currentLevelIndex]);
            _onGameStart?.Invoke();

            musicSource.Play();
            StopCoroutine(WaitUntilTriangulationDone());
            _scoreManager.OnStart();
        }
    }

    private void InitiateLevel()
    {
        if (IsServer)
        {
            Debug.Log("Level Start " + _levels[_currentLevelIndex].gameObject.name);
            _isInputActive = true;
            _currentLevel.StartLevel();
            ChangeState(GameState.LevelIn, _currentLevel);
        }
    }

    private void RunLevel()
    {
        if (IsServer)
        {
            _currentLevel.ExecuteLevel();
            Debug.Log("Level In " + _levels[_currentLevelIndex].gameObject.name);
        }
    }

    private void CompleteLevel()
    {
        if (IsServer)
        {
            Debug.Log("Level End " + _levels[_currentLevelIndex].gameObject.name);
            ChangeState(GameState.LevelStart, _levels[++_currentLevelIndex]);
        }
    }

    private void GameOver()
    {
        if (IsServer)
        {
            DisablePlayerInputClientRpc();
            _isInputActive = false;
            StartCoroutine(GameOverInvoker());
            Debug.Log("Game Over, You Lose!");
        }
    }

    private void GameEnd()
    {
        if (IsServer)
        {
            DisablePlayerInputClientRpc();
            _isInputActive = false;
            StartCoroutine(GameEndInvoker());
            Debug.Log("Game End");
        }
    }

    public void ChangeState(GameState state, NetworkLevelManager level)
    {
        _currentState = state;
        _currentLevel = level;

        switch (_currentState)
        {
            case GameState.LevelStart:
                InitiateLevel();
                break;
            case GameState.LevelIn:
                RunLevel();
                break;
            case GameState.LevelEnd:
                CompleteLevel();
                break;
            case GameState.GameOver:
                GameOver();
                break;
            case GameState.GameEnd:
                GameEnd();
                break;
        }
    }

    private IEnumerator WaitUntilTriangulationDone()
    {
        yield return new WaitUntil(CalDone);
        GameStart2();
    }

    private bool CalDone()
    {
        return _triangulation.vertices.Length > 0;
    }

    private IEnumerator GameOverInvoker()
    {
        yield return new WaitForSeconds(3f);
        _onGameOver?.Invoke();
        UpdateGameOverUIClientRpc();
    }
    private IEnumerator GameEndInvoker()
    {
        yield return new WaitForSeconds(3f);
        _onGameEnd?.Invoke();
        GetScoreManager().GetHighScore();
    }

    public void UpdatePlayerNum()
    {
        Debug.Log("Player num update executed");
        if(_totalPlayers.Value > 0)
        {
            _totalPlayers.Value--;
        }
        else if (_totalPlayers.Value <= 0)
        {
            GetCurrentLevel()._onAllPlayersDie?.Invoke();
        }
    }

    [ClientRpc]
    public void UpdateGameOverUIClientRpc()
    {
        NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<NetworkUIManager>().GameOver();
    }

    [ClientRpc]
    public void DisablePlayerInputClientRpc()
    {
        NetworkManager.LocalClient.PlayerObject.gameObject.GetComponentInChildren<NetworkPlayerInput>()._isDiabled = true;
    }
}


