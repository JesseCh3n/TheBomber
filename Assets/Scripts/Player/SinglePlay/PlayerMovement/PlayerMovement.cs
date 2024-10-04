using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform _cameraPos;

    private CharacterController _characterController;

    public float _playerConstantSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Camera.main.GetComponent<CameraFollow>().setTarget(_cameraPos);
    }

    private void MovePlayer()
    {
        _characterController.Move((transform.forward * (PlayerInput.GetInstance()._vertical + 1f)) * _playerConstantSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    public void SetPlayerSpeed(float speed)
    {
        _playerConstantSpeed = speed;
    }

    public float GetForwardSpeed()
    {
        if (PlayerInput.GetInstance()._vertical == 0)
        {
            return _playerConstantSpeed;
        } else
        {
            return PlayerInput.GetInstance()._vertical * _playerConstantSpeed;
        }
    }
}
