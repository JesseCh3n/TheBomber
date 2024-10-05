using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerMovement : NetworkBehaviour
{
    public Transform _cameraPos;
    public GameObject _camera;

    private CharacterController _characterController;
    private NetworkPlayerInput _playerInput;

    public float _playerConstantSpeed = 1f;
    private bool _isReady = false;


    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<NetworkPlayerInput>();
        _camera.GetComponent<CameraFollow>().setTarget(_cameraPos);
    }

    public void OnStart()
    {
        _isReady = true;
    }

    [ServerRpc]
    private void MovePlayerServerRpc(float vertical, float speed)
    {
        if(vertical != 0)
        {
            Debug.Log("input is " + vertical);
            Debug.Log("Speed is " + speed);
        }

        _characterController.Move((transform.forward * (vertical + 1f)) * speed * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isReady)
        {
            MovePlayerServerRpc(_playerInput._vertical, _playerConstantSpeed);
        }
    }

    public void SetPlayerSpeed(float speed)
    {
        _playerConstantSpeed = speed;
    }

    public float GetForwardSpeed()
    {
        if (_playerInput._vertical == 0)
        {
            return _playerConstantSpeed;
        }
        else
        {
            return _playerInput._vertical * _playerConstantSpeed;
        }
    }

}
