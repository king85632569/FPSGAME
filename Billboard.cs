using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	Camera cam;

	void FixedUpdate()
	{
		if(cam == null)
			cam = Camera.main;

		if(cam == null)
			return;

		transform.LookAt(cam.transform);
		transform.Rotate(Vector3.up * 180);
	}
}
