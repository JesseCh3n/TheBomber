using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform playerTransform;

	// Update is called once per frame
	void Update()
	{
		if (playerTransform != null)
		{
			transform.position = playerTransform.position;
			transform.rotation = playerTransform.rotation;
		}
	}

	public void setTarget(Transform target)
	{
		playerTransform = target;
	}
}
