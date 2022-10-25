using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Movement movement;

    void Update()
    {
        var xAxisInput = Input.GetAxisRaw("Horizontal");
        var zAxisInput = Input.GetAxisRaw("Vertical");
        var zMovement = 0;

        if (Input.GetKey("space"))
        {
            movement.Jump();
        }

        movement.Walk(xAxisInput, zAxisInput);
    }
}