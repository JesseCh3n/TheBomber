using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam);
    }

}