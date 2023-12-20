using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractActions;
    private PlayerinputActions playerInputAction;

    private void Awake()
    {
        playerInputAction = new PlayerinputActions();
        playerInputAction.Player.Enable();

        playerInputAction.Player.Interact.performed += Interact_performed1;
    }

    private void Interact_performed1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        OnInteractActions?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalize()

    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}