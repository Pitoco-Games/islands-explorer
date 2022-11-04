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
        var zMovement = 0;

        if (Input.GetKeyDown("space"))
        {
            movement.Jump();
        }

        movement.Walk(xAxisInput, zAxisInput);
    }
}