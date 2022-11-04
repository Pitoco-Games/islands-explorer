using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Player stats/Movement")]
public class MovementStatsSO : ScriptableObject
{
   [Header("Ground Movement")]
   public float MaxVelocity;
   public float Acceleration;
   public float Deceleration;
   public float TurnSpeed;
   public float JumpStrength;
   public float MovementAngleLimitToReverse;
   public float ReverseDeceleration;
   public float MaxSlopeAngle;

   [Header("Jump")]
   public float FallSpeedMultiplier;
   public float Gravity;
}