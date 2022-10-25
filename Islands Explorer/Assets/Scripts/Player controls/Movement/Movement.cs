using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private MovementStatsSO so;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform camera;

    private float accumulatedWalkVelocity;

    private Vector3 calculatedDirection;
    private float lastXAxisInput;
    private float lastZAxisInput;

    private bool isReversing;

    private void Start()
    {
        calculatedDirection = Vector3.zero;
    }

    public void Walk(float xAxisInput, float zAxisInput)
    {
        if (isReversing)
        {
            DecelerateFromWalk(so.ReverseDeceleration);
            rb.velocity = targetTransform.forward * (accumulatedWalkVelocity * Time.deltaTime);

            if (rb.velocity.magnitude == 0)
            {
                isReversing = false;
            }

            return;
        }

        if (xAxisInput != 0 || zAxisInput != 0)
        {
            var cameraRelativeMoveDirection = CalculateCameraRelativeMoveDirection(xAxisInput, zAxisInput);

            calculatedDirection.x = cameraRelativeMoveDirection.x;
            calculatedDirection.z = cameraRelativeMoveDirection.z;
            if (Vector3.Angle(targetTransform.forward, calculatedDirection) > so.MovementAngleLimitToReverse)
            {
                if (rb.velocity.magnitude == 0)
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
                var newWalkVelocity = accumulatedWalkVelocity + so.Acceleration;
                accumulatedWalkVelocity = newWalkVelocity > so.MaxVelocity ? so.MaxVelocity : newWalkVelocity;
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
        var newWalkVelocity = accumulatedWalkVelocity - deceleration;
        accumulatedWalkVelocity = newWalkVelocity <= 0 ? 0 : newWalkVelocity;
    }

    public void Jump()
    {
        calculatedDirection.y += so.JumpStrength;
    }

    private void FixedUpdate()
    {
        rb.velocity = targetTransform.forward * (accumulatedWalkVelocity * Time.deltaTime);

        if (accumulatedWalkVelocity > 0 && lastXAxisInput == 0 && lastZAxisInput == 0)
        {
            DecelerateFromWalk(so.Deceleration);
        }
    }

    private void ApplyGravity()
    {
        //TODO: If is grounded, do nothing

        //movement.y -= gravity;
    }
}