using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Player stats/Movement")]
public class MovementStatsSO : ScriptableObject
{
   public float MaxVelocity;
   public float Acceleration;
   public float Deceleration;
   public float TurnSpeed;
   public float JumpStrength;
   public float Gravity;
   public float MovementAngleLimitToReverse;
   public float ReverseDeceleration;
}