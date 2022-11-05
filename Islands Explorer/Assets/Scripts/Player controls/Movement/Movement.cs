using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementStatsSO so;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform camera;
    [SerializeField] private CollisionDetector groundedDetector;
    [SerializeField] private SlopeDetector slopeDetector;

    private float accumulatedHorizontalVelocity;
    private float accumulatedVerticalVelocity;
    private float lastXAxisInput;
    private float lastZAxisInput;
    private List<RaycastHit> currRaycastHits;
    private bool slopeIsClimbable;

    private int remainingJumps;
    private bool isReversing;
    private bool isGrounded;
    private bool jumped;

    public void Walk(float xAxisInput, float zAxisInput)
    {
        if (isReversing)
        {
            DecelerateFromWalk(so.ReverseDeceleration);

            if (accumulatedHorizontalVelocity == 0)
            {
                isReversing = false;
            }

            return;
        }

        if (xAxisInput != 0 || zAxisInput != 0)
        {
            var cameraRelativeMoveDirection = CalculateCameraRelativeMoveDirection(xAxisInput, zAxisInput);

            if (Vector3.Angle(targetTransform.forward, cameraRelativeMoveDirection) > so.MovementAngleLimitToReverse)
            {
                if (accumulatedHorizontalVelocity == 0)
                {
                    RotateCharacterTowardsMoveDirection(xAxisInput, zAxisInput, so.TurnSpeed);
                }
                else
                {
                    isReversing = true;
                    return;
                }
            }
            else
            {
                var newWalkVelocity = accumulatedHorizontalVelocity + so.Acceleration;
                accumulatedHorizontalVelocity = newWalkVelocity > so.MaxVelocity ? so.MaxVelocity : newWalkVelocity;
                RotateCharacterTowardsMoveDirection(xAxisInput, zAxisInput, so.TurnSpeed);
            }
        }

        lastXAxisInput = xAxisInput;
        lastZAxisInput = zAxisInput;
    }

    private void RotateCharacterTowardsMoveDirection(float xAxisInput, float zAxisInput, float turnSpeed)
    {
        var direction = CalculateCameraRelativeMoveDirection(xAxisInput, zAxisInput);
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        targetTransform.rotation =
            Quaternion.RotateTowards(targetTransform.rotation, toRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 CalculateCameraRelativeMoveDirection(float xAxisInput, float zAxisInput)
    {
        Vector3 right = camera.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        return (right * xAxisInput) + (forward * zAxisInput);
    }

    private void DecelerateFromWalk(float deceleration)
    {
        var newWalkVelocity = accumulatedHorizontalVelocity - deceleration;
        accumulatedHorizontalVelocity = newWalkVelocity <= 0 ? 0 : newWalkVelocity;
    }

    public void Jump()
    {
        if (remainingJumps > 0)
        {
            remainingJumps--;
            if (accumulatedVerticalVelocity < 0)
            {
                accumulatedVerticalVelocity = 0;
            }

            accumulatedVerticalVelocity += so.JumpStrength;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = groundedDetector.OverlapSphereIsColliding();
        if (isGrounded)
        {
            remainingJumps = so.JumpQuantity;
        }

        currRaycastHits = slopeDetector.GetAllHits();
        slopeIsClimbable = slopeDetector.GetSlopeIsClimbable(currRaycastHits);

        rb.velocity = GetSlopeCorrectedWalkVector() + Vector3.up * accumulatedVerticalVelocity;

        if (accumulatedHorizontalVelocity > 0 && lastXAxisInput == 0 && lastZAxisInput == 0)
        {
            DecelerateFromWalk(so.Deceleration);
        }

        ApplyGravity();
    }

    private Vector3 GetSlopeCorrectedWalkVector()
    {
        float deltaTimeCorrectedWalkVelocity = accumulatedHorizontalVelocity * Time.deltaTime;

        if (currRaycastHits.Count == 0)
        {
            return deltaTimeCorrectedWalkVelocity * targetTransform.forward;
        }

        if (slopeIsClimbable)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, currRaycastHits[0].normal);
            Vector3 adjustedVector = slopeRotation * targetTransform.forward * deltaTimeCorrectedWalkVelocity;

            Debug.DrawRay(transform.position, adjustedVector, Color.blue);

            return adjustedVector;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && accumulatedVerticalVelocity < 0 && slopeIsClimbable)
        {
            accumulatedVerticalVelocity = 0;
            return;
        }

        if(!isGrounded || !slopeIsClimbable)
        {
            if (accumulatedVerticalVelocity < 0)
            {
                accumulatedVerticalVelocity -= so.Gravity * so.FallSpeedMultiplier * Time.deltaTime;
                accumulatedVerticalVelocity = accumulatedVerticalVelocity > so.TerminalVelocity
                    ? so.TerminalVelocity
                    : accumulatedVerticalVelocity;
            }
            else
            {
                accumulatedVerticalVelocity -= so.Gravity * Time.deltaTime;
            }
        }
    }
}