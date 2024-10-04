using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkMiniCameraController : NetworkBehaviour
{
    [SerializeField]
    private Camera _camera;

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
        base.OnNetworkSpawn();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "NetworkGameScene")
        {
            _camera.enabled = true;
        }
    }
}
