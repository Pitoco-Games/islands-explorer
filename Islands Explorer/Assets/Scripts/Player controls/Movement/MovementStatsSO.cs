using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Player stats/Movement")]
public class MovementStatsSO : ScriptableObject
{
   [Header("Ground Movement")]
   public float MaxVelocity;
   public float Acceleration;
   public float Deceleration;
   public float TurnSpeed;
   public float MovementAngleLimitToReverse;
   public float ReverseDeceleration;
   public float MaxSlopeAngle;

   [Header("Jump")]
   public float JumpStrength;
   public float FallSpeedMultiplier;
   public float Gravity;
   public int JumpQuantity;
   public float TerminalVelocity;

   [Header("Dash")]
   public float DashDistance;
   public float DashDuration;
   public int AirDashAmount;
   public float DashCooldown;

   [Header("Air movement")]
   public float AirAcceleration;
   public float AirDeceleration;
}