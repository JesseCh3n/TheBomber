using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerRotation : NetworkBehaviour
{
    [SerializeField] private float _turnSpeed;
    [SerializeField] private Transform _prefabTransform;
    private NetworkPlayerInput _playerInput;
    private bool _isReady = false;

    private void Start()
    {
        _playerInput = GetComponent<NetworkPlayerInput>();
    }
    public void OnStart()
    {
        _isReady = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isReady)
        {
            RotatePlayerServerRpc(_playerInput._horizontal);
        }
    }

    // Rotate aircraft along y-axis. 
    [ServerRpc]
    private void RotatePlayerServerRpc(float horizontal)
    {
        transform.Rotate(Vector3.up * _turnSpeed * Time.fixedDeltaTime * (horizontal));
    }

}
