using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkTransformRotation : NetworkBehaviour
{
    private float _returnSpeed = 80f;
    private NetworkPlayerInput _playerInput;
    private bool _isReady = false;

    private void Start()
    {
        _playerInput = GetComponentInParent<NetworkPlayerInput>();
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
            RotateTransformServerRpc(_playerInput._horizontal);
        }
    }

    [ServerRpc]
    private void RotateTransformServerRpc(float horizontal)
    {
        // Aircraft rotate along z-axis, animation purpose only.
        float z1 = horizontal * Time.fixedDeltaTime * 200;
        Vector3 rot1 = transform.localRotation.eulerAngles - new Vector3(0f, 0f, z1);
        rot1.z = ClampAngle(rot1.z, -60f, 60f);
        transform.localEulerAngles = rot1;

        // Aircraft return to 0 on z-axis, animation purpose only. 
        if (horizontal == 0 && transform.localRotation.eulerAngles.z < 180f && transform.localRotation.eulerAngles.z > 0f)
        {
            float z2 = Time.fixedDeltaTime * _returnSpeed;
            Vector3 rot2 = transform.localRotation.eulerAngles - new Vector3(0f, 0f, z2);
            transform.localEulerAngles = rot2;
            if (rot2.z < 0f)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (horizontal == 0 && transform.localRotation.eulerAngles.z > 180f)
        {
            float z2 = Time.fixedDeltaTime * _returnSpeed;
            Vector3 rot2 = transform.localRotation.eulerAngles + new Vector3(0f, 0f, z2);
            transform.localEulerAngles = rot2;
            if (rot2.z > 360f)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
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
