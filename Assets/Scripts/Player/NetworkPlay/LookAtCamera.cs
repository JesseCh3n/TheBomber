using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LookAtCamera : NetworkBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        FindCameraClientRpc();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam);
    }

    [ClientRpc]
    public void FindCameraClientRpc()
    {
        cam = Camera.main.transform;
    }
}