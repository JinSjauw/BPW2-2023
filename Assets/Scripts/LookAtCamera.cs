using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform camera;

    private void Awake()
    {
        camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 directionToCamera = (camera.position - transform.position).normalized;
        transform.LookAt(transform.position + directionToCamera * -1);
    }
}
