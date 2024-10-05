using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerManager : NetworkBehaviour
{
    public ulong _playerID { get; private set; }
    public string _playerName { get; private set; }

    public NetworkPlayerState _currentState;
    public bool _gameReady = false;

    public override void OnNetworkSpawn()
    {
        _playerID = GetComponent<NetworkObject>().OwnerClientId;
        Debug.Log(_playerID);
        _playerName = GameNetworkManager.GetInstance().GetPlayerName();
        base.OnNetworkSpawn();
    }

    void Start()
    {
        _currentState = new NetworkPlayerAwakeState(this);
        _currentState.OnStateEnter();
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.OnStateUpdate();
    }

    public void ChangeState(NetworkPlayerState state)
    {
        _currentState.OnStateExit();
        _currentState = state;
        _currentState.OnStateEnter();
    }

    public void AddPlayer()
    {
        ConnectionNotificationManager.Singleton.SetPlayerStatus(gameObject.GetComponent<NetworkObject>());
    }

    public bool CheckPlayerExist()
    {
        return ConnectionNotificationManager.Singleton._playerStatusDict.ContainsKey(gameObject.GetComponent<NetworkObject>().OwnerClientId);
    }

    public bool CheckOwnership()
    {
        return IsOwner;
    }

    public bool CheckHealth()
    {
        if (IsOwner)
        {
            return (GetComponentInChildren<Health>()._maxHealth != 10);
        }
        else
        {
            return true;
        }
    }

}
