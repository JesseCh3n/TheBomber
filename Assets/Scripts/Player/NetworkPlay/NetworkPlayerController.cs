using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour, IDestroyable
{
    private Health _playerHealth;
    private NetworkPlayerMovement _playerMovement;
    private NetworkPlayerRotation _playerRotation;
    private NetworkTransformRotation _transformRotation;
    private NetworkPlayerShoot _playerShoot;
    [SerializeField]
    public NetworkPlayerInfo _playerInfo;
    private NetworkPlayerInput _playerInput;
    private NetworkObject _networkPlayer;
    private NetworkUIManager _uiManager;


    // Start is called before the first frame update
    void Start()
    {
        _networkPlayer = gameObject.GetComponentInParent<NetworkObject>();
        _playerInput = gameObject.GetComponent<NetworkPlayerInput>();
        _playerHealth = gameObject.GetComponent<Health>();
        _playerMovement = gameObject.GetComponent<NetworkPlayerMovement>();
        _playerRotation = gameObject.GetComponent<NetworkPlayerRotation>();
        _transformRotation = gameObject.GetComponentInChildren<NetworkTransformRotation>();
        _playerShoot = gameObject.GetComponent<NetworkPlayerShoot>();
        _uiManager = GetComponentInParent<NetworkUIManager>();
        NetworkGameManager.GetInstance()._bombUpdated += _playerShoot.SetBulletNum;
        NetworkGameManager.GetInstance()._rocketUpdated += _playerShoot.SetRocketNum;
        NetworkGameManager.GetInstance()._undoUpdated += _playerShoot.SetUndoChance;
        NetworkGameManager.GetInstance()._healthUpdated += SetPlayerHealth;
        NetworkGameManager.GetInstance()._speedUpdated += _playerMovement.SetPlayerSpeed;
    }

    public void SetPlayer()
    {
        _playerMovement.SetPlayerSpeed(NetworkGameManager.GetInstance()._playerSpeed);
        SetPlayerHealth(NetworkGameManager.GetInstance()._playerHealth);
        _playerShoot._bulletNum = NetworkGameManager.GetInstance()._totalBombs;
        _playerShoot._rocketNum = NetworkGameManager.GetInstance()._totalRockets;
        _playerShoot._undoChance = NetworkGameManager.GetInstance()._totalUndo;
        _uiManager.GameStarted();
    }

    public void OnStart()
    {
        _playerMovement.OnStart();
        _playerRotation.OnStart();
        _transformRotation.OnStart();
        if (IsOwner)
        {
            if (_uiManager != null)
            {
                Debug.Log("level " + NetworkGameManager.GetInstance().GetCurrentLevel());
                Debug.Log("level name " + NetworkGameManager.GetInstance()._levelName.Value.ToString());
                _uiManager.UpdateLevel(NetworkGameManager.GetInstance()._levelName.Value.ToString());
                NetworkGameManager.GetInstance()._levelUpdated += _uiManager.UpdateLevel;

                _playerHealth._onHealthUpdated += _uiManager.UpdateHealth;
                _playerHealth._onHealthUpdated += UpdatePlayerHealth;
                _playerHealth.OnStart();

                _playerShoot.OnStart();

                NetworkGameManager.GetInstance().GetScoreManager()._scoreUpdated += FetchPlayerScoreServerRpc;
                NetworkGameManager.GetInstance().GetScoreManager()._highScoreUpdated += FetchHighScoreServerRpc;
                AddPlayerScoreServerRpc();
            }
        }
    }

    public void Die()
    {
        if (IsOwner)
        {
            gameObject.tag = "Dead";
            ReducePlayerNumServerRpc();
            _playerInput._isDiabled = true;
            OnDie();
        }
    }

    public NetworkPlayerInfo GetPlayerInfo()
    {
        return _playerInfo;
    }

    public void SetPlayerHealth(float health)
    {
        if (IsOwner)
        {
            _playerHealth._maxHealth = health;
            ConnectionNotificationManager.Singleton.SetPlayerHealth(_networkPlayer, health);
            gameObject.GetComponentInParent<NetworkPlayerManager>().GetComponentInChildren<NetworkPlayerInfo>().SetHealth(health);
            //_playerHealth.OnReset();
        }
    }

    public Health GetPlayerHealth()
    {
        return _playerHealth;
    }

    public void UpdatePlayerHealth(float value)
    {
        if (IsOwner)
        {
            ConnectionNotificationManager.Singleton.SetPlayerHealth(_networkPlayer, value);
            gameObject.GetComponentInParent<NetworkPlayerManager>().GetComponentInChildren<NetworkPlayerInfo>().SetHealth(value);
        }
    }

    public NetworkPlayerMovement GetPlayerMovement()
    {
        return _playerMovement;
    }

    public NetworkPlayerRotation GetPlayerRotation()
    {
        return _playerRotation;
    }
    public NetworkPlayerShoot GetPlayerShoot()
    {
        return _playerShoot;
    }

    [ServerRpc]
    private void ReducePlayerNumServerRpc()
    {
        NetworkGameManager.GetInstance().UpdatePlayerNum();
    }

    [ServerRpc]
    public void AddPlayerScoreServerRpc()
    {
        NetworkGameManager.GetInstance().GetScoreManager().PopulateScore(gameObject.GetComponentInParent<NetworkObject>());
    }

    [ServerRpc]
    public void FetchPlayerScoreServerRpc()
    {
        NetworkGameManager.GetInstance().GetScoreManager().FetchScore(gameObject.GetComponentInParent<NetworkObject>());
    }

    [ServerRpc]
    public void FetchHighScoreServerRpc()
    {
        NetworkGameManager.GetInstance().GetScoreManager().FetchHighScore();
    }

    public void OnDie()
    {
        NetworkGameManager.GetInstance()._bombUpdated -= _playerShoot.SetBulletNum;
        NetworkGameManager.GetInstance()._rocketUpdated -= _playerShoot.SetRocketNum;
        NetworkGameManager.GetInstance()._undoUpdated -= _playerShoot.SetUndoChance;
        NetworkGameManager.GetInstance()._healthUpdated -= SetPlayerHealth;
        NetworkGameManager.GetInstance()._speedUpdated -= _playerMovement.SetPlayerSpeed;
        if (IsOwner)
        {
            _playerHealth._onHealthUpdated -= _uiManager.UpdateHealth;
            _playerHealth._onHealthUpdated -= UpdatePlayerHealth;
            _playerShoot.OnDie();
        }
    }
}
