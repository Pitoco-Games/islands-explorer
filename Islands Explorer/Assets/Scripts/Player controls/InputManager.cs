using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Movement movement;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var xAxisInput = Input.GetAxisRaw("Horizontal");
        var zAxisInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown("space"))
        {
            movement.Jump();
        }

        if (Input.GetKeyDown("left shift"))
        {
            movement.Dash(xAxisInput, zAxisInput);
        }

        movement.Walk(xAxisInput, zAxisInput);
    }
}