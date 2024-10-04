using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Only attach this example component to the NetworkManager GameObject.
/// This will provide you with a single location to register for client
/// connect and disconnect events.  
/// </summary>
public class ConnectionNotificationManager : MonoBehaviour
{
    public static ConnectionNotificationManager Singleton { get; internal set; }

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

    /// <summary>
    /// This action is invoked whenever a client connects or disconnects from the game.
    ///   The first parameter is the ID of the client (ulong).
    ///   The second parameter is whether that client is connecting or disconnecting.
    /// </summary>
    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;
    public List<ulong> _playerList;
    public Dictionary<ulong, string> _playerNameDict = new Dictionary<ulong, string>();
    public Dictionary<ulong, float> _playerHealthDict = new Dictionary<ulong, float>();
    public Dictionary<ulong, bool> _playerStatusDict = new Dictionary<ulong, bool>();

    private void Awake()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(ConnectionNotificationManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

    private void Start()
    {
        if (Singleton != this)
        {
            return; // so things don't get even more broken if this is a duplicate >:(
        }

        if (NetworkManager.Singleton == null)
        {
            // Can't listen to something that doesn't exist >:(
            throw new Exception($"There is no {nameof(NetworkManager)} for the {nameof(ConnectionNotificationManager)} to do stuff with! " +
                $"Please add a {nameof(NetworkManager)} to the scene.");
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }


    private void OnDestroy()
    {
        // Since the NetworkManager can potentially be destroyed before this component, only
        // remove the subscriptions if that singleton still exists.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);
        _playerList.Add(clientId);
        NetworkGameManager.GetInstance()._totalPlayers.Value = _playerList.Count;
        Debug.Log(clientId);
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);
        _playerList.Remove(clientId);
    }

    public void SetPlayerName(NetworkObject playerNetObject, string name)
    {
        if (_playerNameDict.ContainsKey(playerNetObject.OwnerClientId))
        {
            _playerNameDict[playerNetObject.OwnerClientId] = name;
        }
        else
        {
            _playerNameDict.Add(playerNetObject.OwnerClientId, name);
        }
        //playerNetObject.GetComponentInChildren<NetworkPlayerInfo>().SetName(name);
    }

    public void SetPlayerHealth(NetworkObject playerNetObject, float health)
    {
        if (_playerHealthDict.ContainsKey(playerNetObject.OwnerClientId))
        {
            _playerHealthDict[playerNetObject.OwnerClientId] = health;
        }
        else
        {
            _playerHealthDict.Add(playerNetObject.OwnerClientId, health);
        }
        //playerNetObject.GetComponentInChildren<NetworkPlayerInfo>().SetHealth(health);
    }

    public void SetPlayerStatus(NetworkObject playerNetObject)
    {
        if (_playerStatusDict.ContainsKey(playerNetObject.OwnerClientId))
        {
            _playerStatusDict[playerNetObject.OwnerClientId] = true;
        }
        else
        {
            bool spawned = true;
            _playerStatusDict.Add(playerNetObject.OwnerClientId, spawned);
        }
    }

    public bool CheckSpawned()
    {
        if (_playerList.Count == _playerStatusDict.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}