using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkCameraController : NetworkBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private AudioListener _audio;

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
            _audio.enabled = true;
        }
    }
}
