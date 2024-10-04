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

    /*
    [ServerRpc]
    public void ActivatePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            player.gameObject.GetComponentInChildren<CharacterController>().enabled = true;
            player.gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;
            player.gameObject.GetComponentInChildren<NetworkPlayerMovement>().enabled = true;
            player.gameObject.GetComponentInChildren<NetworkPlayerRotation>().enabled = true;
            player.gameObject.GetComponentInChildren<NetworkPlayerInfo>().enabled = true;
        }
    }

    [ServerRpc]
    public void InitiatePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Client is here 13");
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            NetworkGameManager.GetInstance().GetCurrentLevel().OnPlayerJoined(client.PlayerObject);
        }
    }

    public void LevelChanged()
    {
        Debug.Log("Client is here 9");
        SetPlayerServerRpc();
    }

    [ServerRpc]
    public void SetPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Client is here 10");
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            Debug.Log("Client is here 11");
            var client = NetworkManager.ConnectedClients[clientId];
            NetworkGameManager.GetInstance().GetCurrentLevel().SetPlayer(client.PlayerObject);
        }
    }
    */
}
