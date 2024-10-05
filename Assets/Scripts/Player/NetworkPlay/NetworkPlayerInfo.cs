using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using TMPro;

public class NetworkPlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text _txtPlayerName;
    [SerializeField] private TMP_Text _txtPlayerHealth;
    [SerializeField] private NetworkPlayerController _playerController;
    //[SerializeField] private GameObject _player;

    public NetworkVariable<FixedString64Bytes> _playerName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("Player Name"),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> _playerHealth = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    //Subscribe 
    void Start()
    {
        //_onGameReady += SetPlayerServerRpc;
        _playerName.OnValueChanged += OnNameChanged;
        _playerHealth.OnValueChanged += OnHealthChanged;


        _txtPlayerName.SetText(_playerName.Value.ToString());
        _txtPlayerHealth.SetText(_playerHealth.Value.ToString());

        string name = GetComponentInParent<NetworkPlayerManager>()._playerName;
        if (IsOwner)
        {
            SetName(name);
        }
        if (IsServer)
        {
            ConnectionNotificationManager.Singleton.SetPlayerName(NetworkObject, name);
        }

        NetworkObject netObj = gameObject.GetComponentInParent<NetworkObject>();
        netObj.gameObject.name = "Player_" + _playerName.Value.ToString();
    }

    //Call back 
    private void OnNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        if (previousValue != newValue)
        {
            _txtPlayerName.SetText(newValue.Value);
            if(ConnectionNotificationManager.Singleton != null && IsLocalPlayer)
            {
                ConnectionNotificationManager.Singleton.SetPlayerName(NetworkObject, newValue.Value.ToString());
            }
        }
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        if (previousValue != newValue)
        {
            _txtPlayerHealth.SetText(newValue.ToString());
        }
    }

    //Unsubscribe
    public override void OnNetworkDespawn()
    {
        _playerName.OnValueChanged -= OnNameChanged;
    }

    public void SetName(string name)
    {
        _playerName.Value = new FixedString64Bytes(name);
    }

    public void SetHealth(float value)
    {
        _playerHealth.Value = value;
    }
}

