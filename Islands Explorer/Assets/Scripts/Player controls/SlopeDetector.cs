using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlopeDetector : MonoBehaviour
{
    [SerializeField] private MovementStatsSO so;
    [SerializeField] private Transform[] rayOrigins;
    [SerializeField] private Vector3[] rayVectors;
    [SerializeField] private LayerMask layerMask;

    [Header("Debug")]
    [SerializeField] private Color rayGizmoColor;

    private Transform thisTransform;
    private Vector3 downRayVector;

    private void Awake()
    {
        thisTransform = transform;
    }

    public List<RaycastHit> GetAllHits()
    {
        var hits = new List<RaycastHit>();

        for (int i = 0 ; i < rayOrigins.Length ; i ++)
        {
            Transform currRayOrigin = rayOrigins[i];
            if (Physics.Raycast(currRayOrigin.position, rayVectors[i], out RaycastHit raycastHit, rayVectors[i].magnitude, layerMask))
            {
                hits.Add(raycastHit);
            }
        }

        return hits;
    }

    public bool GetSlopeIsClimbable(List<RaycastHit> hits)
    {
        for (int i = 0 ; i < hits.Count ; i ++)
        {
            if (Vector3.Angle(thisTransform.up, hits[i].normal) > so.MaxSlopeAngle)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = rayGizmoColor;

        for (int i = 0; i < rayOrigins.Length; i++)
        {
            Transform rayOrigin = rayOrigins[i];
            Gizmos.DrawRay(rayOrigin.position, rayVectors[i]);
        }
    }
}