using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform _location;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _location.position;

        transform.rotation = Quaternion.Euler(90f, _location.eulerAngles.y, 0f);
    }
}
