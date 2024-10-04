using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

//To do:
//Step 1: creating a menu two buttons to separte single vs multiplayer
//Step 2: disable network components on single player mode
//Step 3: using regular instantiate/destroy pattern
//Step 4: check playershoot Script in the course as a reference

//If objects are not in heriarchy, use Action instead of Unity Event

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelManager[] _levels;
    private LevelManager _currentLevel;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private SpawnManager _spawner;
    [SerializeField] private SceneChanger _sceneManager;
    [SerializeField] private UIManager _uiManager;
    private int _currentLevelIndex = 0;
    private bool _isInputActive = true;

    public AudioSource musicSource;
    public NavMeshTriangulation _triangulation;

    public UnityEvent _onGameStart;
    public UnityEvent _onGameOver;
    public UnityEvent _onGameEnd;

    public bool IsInputActive()
    {
        return _isInputActive;
    }

    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        return _instance;
    }

    public LevelManager GetCurrentLevel()
    {
        return _currentLevel;
    }

    public ScoreManager GetScoreManager()
    {
        return _scoreManager;
    }

    public SpawnManager GetSpawner()
    {
        return _spawner;
    }

    public SceneChanger GetSceneChanger()
    {
        return _sceneManager;
    }

    public UIManager GetUIManager()
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

    public void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(WaitUntilTriangulationDone());
    }

    // Start is called before the first frame update
    public void GameStart()
    {
        ChangeState(GameState.LevelStart, _levels[_currentLevelIndex]);
        _onGameStart?.Invoke();

        musicSource.Play();
        StopCoroutine(WaitUntilTriangulationDone());
    }


    private void InitiateLevel()
    {
        Debug.Log("Level Start");
        _isInputActive = true;
        _currentLevel.StartLevel();
        ChangeState(GameState.LevelIn, _currentLevel);
    }
    private void RunLevel()
    {
        _currentLevel.ExecuteLevel();
        Debug.Log("LevelIn");
    }
    private void CompleteLevel()
    {
        Debug.Log("Level End " + _levels[_currentLevelIndex].gameObject.name);
        ChangeState(GameState.LevelStart, _levels[++_currentLevelIndex]);
    }
    private void GameOver()
    {
        _isInputActive = false;
        StartCoroutine(GameOverInvoker());
        Debug.Log("Game Over, You Lose!");
    }
    private void GameEnd()
    {
        _isInputActive = false;
        StartCoroutine(GameEndInvoker());
        Debug.Log("Game End");
    }

    public void ChangeState(GameState state, LevelManager level)
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

    private IEnumerator WaitUntilTriangulationDone ()
    {
        while (!CalDone())
        {
            yield return null;
        }
        GameStart();
    }

    private bool CalDone()
    {
        return _triangulation.vertices.Length > 0;
    }

    private IEnumerator GameOverInvoker()
    {
        yield return new WaitForSeconds(3f);
        _onGameOver?.Invoke();
    }
    private IEnumerator GameEndInvoker()
    {
        yield return new WaitForSeconds(3f);
        _onGameEnd?.Invoke();
    }
}