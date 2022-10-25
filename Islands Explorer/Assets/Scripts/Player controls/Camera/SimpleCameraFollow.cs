using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform camTransform;

    private void Update()
    {
        camTransform.position = followTarget.position + offset;
    }
}