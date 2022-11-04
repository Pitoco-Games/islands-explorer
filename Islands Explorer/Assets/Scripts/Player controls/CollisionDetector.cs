using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private QueryTriggerInteraction triggerInteraction;

    [Header("Sphere Parameters")]
    [SerializeField] private float defaultSphereRadius;

    [Header("Ray Parameters")]
    [SerializeField] private Vector3 defaultRayDirection;

    [Header("References")]
    [SerializeField] private Transform thisTransform;

    [Header("Debug")]
    [SerializeField] private bool drawSphere;
    [SerializeField] private bool drawRay;
    [SerializeField] private Color sphereColor;
    [SerializeField] private Color rayColor;

    private void Awake()
    {
        thisTransform ??= transform;
    }

    public bool OverlapSphereIsColliding()
    {
        return GetOverlapSphereCollisions().Length > 0;
    }

    public Collider[] GetOverlapSphereCollisions()
    {
        return Physics.OverlapSphere(thisTransform.position, defaultSphereRadius,layerMask, triggerInteraction);
    }

    public Collider[] GetOverlapSphereCollisions(float sphereRadius)
    {
        return Physics.OverlapSphere(thisTransform.position, sphereRadius,layerMask, triggerInteraction);
    }

    public bool TryGetRaycastHit(out RaycastHit raycastHit)
    {
        return Physics.Raycast(thisTransform.position, defaultRayDirection, out raycastHit, defaultRayDirection.magnitude, layerMask);
    }

    public bool TryGetRaycastHit(out RaycastHit raycastHit, Vector3 rayDirection)
    {
        return Physics.Raycast(thisTransform.position, rayDirection, out raycastHit, rayDirection.magnitude, layerMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (drawSphere)
        {
            Gizmos.color = sphereColor;
            Gizmos.DrawWireSphere(thisTransform.position, defaultSphereRadius);
        }
        if (drawRay)
        {
            Gizmos.color = rayColor;
            Gizmos.DrawRay(thisTransform.position, defaultRayDirection);
        }
    }
}