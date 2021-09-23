using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMap : MonoBehaviour
{
    private InputState InputState {  get { return PlayerController.Instance == null ? new InputState() : PlayerController.Instance.InputState; } }

    public void OnThrust(InputAction.CallbackContext value)
    {
        InputState.Thrust = value.ReadValue<float>();
    }
    public void OnHorizontalThrust(InputAction.CallbackContext value)
    {
        InputState.HorizontalThrust = value.ReadValue<float>();
    }
    public void OnVerticalThrust(InputAction.CallbackContext value)
    {
        InputState.VerticalThrust = value.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext value)
    {
        InputState.Roll = value.ReadValue<float>();
    }
    public void OnPitch(InputAction.CallbackContext value)
    {
        InputState.Pitch = -value.ReadValue<float>();
    }
    public void OnYaw(InputAction.CallbackContext value)
    {
        InputState.Yaw = value.ReadValue<float>();
    }

    public void OnA(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            InputState.Fire1 = true;
        }
        if (value.canceled)
        {
            InputState.Fire1 = false;
        }
    }

    public void OnDpadUp(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            InputState.GyrosActive = !InputState.GyrosActive;
        }
    }

    public void OnSelect(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            //floop to shipeditor
        }
    }
}
