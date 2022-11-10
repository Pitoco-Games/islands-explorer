using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    private int remainingJumps;
    private int remainingDashes;

    private bool slopeIsClimbable;
    private bool isReversing;
    private bool isGrounded;
    private bool isDashing;
    private bool dashIsInCooldown;
    private bool jumped;

    public void Walk(float xAxisInput, float zAxisInput)
    {
        if (isDashing)
        {
            return;
        }

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
        if (isDashing || remainingJumps == 0)
        {
            return;
        }

        remainingJumps--;
        if (accumulatedVerticalVelocity < 0)
        {
            accumulatedVerticalVelocity = 0;
        }

        accumulatedVerticalVelocity += so.JumpStrength;
    }

    public void Dash(float xAxisInput, float zAxisInput)
    {
        if (isDashing || dashIsInCooldown || remainingDashes == 0)
        {
            return;
        }

        isDashing = true;
        dashIsInCooldown = true;
        remainingDashes--;

        if (xAxisInput == 0 && zAxisInput == 0)
        {
            var forward = targetTransform.forward;
            xAxisInput = forward.x;
            zAxisInput = forward.z;
        }
        StartCoroutine(DashCoroutine(xAxisInput, zAxisInput));
    }

    private IEnumerator DashCoroutine(float xAxisInput, float zAxisInput)
    {
        float remainingDashTime = so.DashDuration;

        var dashDirection = CalculateCameraRelativeMoveDirection(xAxisInput, zAxisInput).normalized;
        Vector3 currPosition = targetTransform.position;
        var finalPosition = currPosition + dashDirection * so.DashDistance;

        double moveDelta = Vector3.Distance(finalPosition, currPosition);
        float moveSpeed = (float) moveDelta / so.DashDuration;

        targetTransform.forward = dashDirection;
        while (remainingDashTime > 0 && slopeIsClimbable)
        {
            //RotateCharacterTowardsMoveDirection(xAxisInput, zAxisInput, so.TurnSpeed);
            rb.velocity = GetSlopeCorrectedMovementVector(dashDirection) * moveSpeed;

            remainingDashTime -= Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        var dashCooldown = so.DashCooldown;
        while (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
            yield return null;
        }

        dashIsInCooldown = false;
    }

    private void FixedUpdate()
    {
        currRaycastHits = slopeDetector.GetAllHits();
        slopeIsClimbable = slopeDetector.GetSlopeIsClimbable(currRaycastHits);

        if (isDashing)
        {
            return;
        }

        isGrounded = groundedDetector.OverlapSphereIsColliding();
        if (isGrounded)
        {
            remainingJumps = so.JumpQuantity;
            remainingDashes = so.AirDashAmount;
        }

        Vector3 slopeCorrectedMoveVector = GetSlopeCorrectedMovementVector(targetTransform.forward);
        float moveAmount = accumulatedHorizontalVelocity * Time.deltaTime;
        rb.velocity = slopeCorrectedMoveVector * moveAmount + Vector3.up * accumulatedVerticalVelocity;

        if (accumulatedHorizontalVelocity > 0 && lastXAxisInput == 0 && lastZAxisInput == 0)
        {
            DecelerateFromWalk(so.Deceleration);
        }

        ApplyGravity();
    }

    private Vector3 GetSlopeCorrectedMovementVector(Vector3 moveDirection)
    {
        if (currRaycastHits.Count == 0)
        {
            return moveDirection;
        }

        if (slopeIsClimbable)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, currRaycastHits[0].normal);
            Vector3 adjustedVector = slopeRotation * moveDirection;

            Debug.DrawRay(transform.position, adjustedVector, Color.blue);

            return adjustedVector;
        }

        return Vector3.zero;
    }

    private void ApplyGravity()
    {
        if (isDashing)
        {
            return;
        }

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
                accumulatedVerticalVelocity = accumulatedVerticalVelocity < so.TerminalVelocity
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