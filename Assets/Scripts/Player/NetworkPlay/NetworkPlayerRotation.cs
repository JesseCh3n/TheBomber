using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerRotation : NetworkBehaviour
{
    [SerializeField] private float _turnSpeed;
    [SerializeField] private Transform _prefabTransform;
    private float _returnSpeed = 80f;
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

        if (IsLocalPlayer && _isReady)
        {
            RotatePlayerServerRpc(_playerInput._horizontal);
            RotateTransformServerRpc(_playerInput._horizontal);
        }

    }

    // Rotate aircraft along y-axis. 
    [ServerRpc]
    private void RotatePlayerServerRpc(float horizontal)
    {
        transform.Rotate(Vector3.up * _turnSpeed * Time.deltaTime * (horizontal));
    }

    [ServerRpc]
    private void RotateTransformServerRpc(float horizontal)
    {
        // Aircraft rotate along z-axis, animation purpose only.
        float z1 = horizontal * Time.deltaTime * _turnSpeed;
        Vector3 rot1 = _prefabTransform.localRotation.eulerAngles - new Vector3(0f, 0f, z1);
        rot1.z = ClampAngle(rot1.z, -60f, 60f);
        _prefabTransform.localEulerAngles = rot1;

        // Aircraft return to 0 on z-axis, animation purpose only. 
        if (horizontal == 0 && _prefabTransform.localRotation.eulerAngles.z < 180f && _prefabTransform.localRotation.eulerAngles.z > 0f)
        {
            float z2 = Time.deltaTime * _returnSpeed;
            Vector3 rot2 = _prefabTransform.localRotation.eulerAngles - new Vector3(0f, 0f, z2);
            _prefabTransform.localEulerAngles = rot2;
            if (rot2.z < 0f)
            {
                _prefabTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (horizontal == 0 && _prefabTransform.localRotation.eulerAngles.z > 180f)
        {
            float z2 = Time.deltaTime * _returnSpeed;
            Vector3 rot2 = _prefabTransform.localRotation.eulerAngles + new Vector3(0f, 0f, z2);
            _prefabTransform.localEulerAngles = rot2;
            if (rot2.z > 360f)
            {
                _prefabTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f)
        {
            angle = 360 + angle;
        }
        if (angle > 180f)
        {
            return Mathf.Max(angle, 360 + from);
        }
        else
        {
            return Mathf.Min(angle, to);
        }
    }
}
