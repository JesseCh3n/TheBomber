using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabRotation : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    private Quaternion _originalRotation;


    private void Start()
    {
        _originalRotation = transform.rotation;
        Debug.Log(_originalRotation);
    }
    // Update is called once per frame
    void Update()
    {
        RotateInFrame();
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, 1);
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
        } else
        {
            return Mathf.Min(angle, to);
        }
    }


    void RotateInFrame()
    {
        float mz = PlayerInput.GetInstance()._horizontal * Time.deltaTime * _rotateSpeed;
        Vector3 rot = transform.rotation.eulerAngles - new Vector3(0f, 0f, mz);
        Debug.Log(rot.z);
        rot.z = ClampAngle(rot.z, -60f, 60f);
        transform.eulerAngles = rot;

        if (PlayerInput.GetInstance()._horizontal == 0)
        {
            //Vector3 rotateToTarget = Vector3.zero;

            //var rotate = Quaternion.LookRotation(_originalRotation - transform.localPosition);
            //transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, 1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, 10f);
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rotate, 10f);
            //transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z);
        }
    }
}
