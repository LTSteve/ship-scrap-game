using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipEditor))]
public class ShipEditorInputMap : MonoBehaviour
{
    private ShipEditor ShipEditor;

    [SerializeField]
    private PlayerInput PlayerInput;

    private void Start()
    {
        ShipEditor = GetComponent<ShipEditor>();
    }

    public void OnSelect(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            //floop to shipeditor
            var active = ShipEditor.ToggleActivation();

            //change control scheme
            if (active)
                PlayerInput.SwitchCurrentActionMap("Ship Builder Controls");
            else
                PlayerInput.SwitchCurrentActionMap("Generic Controls");
        }
    }

    public void OnLeftStickHorizontal(InputAction.CallbackContext value)
    {
        ShipEditor.MoveTarget = new Vector2(value.ReadValue<float>(), ShipEditor.MoveTarget.y);
    }
    public void OnLeftStickVertical(InputAction.CallbackContext value)
    {
        ShipEditor.MoveTarget = new Vector2(ShipEditor.MoveTarget.x, value.ReadValue<float>());
    }

    public void OnLeftStickClick(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            ShipEditor.ToggleZoom(true);
        }
        if(value.canceled)
        {
            ShipEditor.ToggleZoom(false);
        }
    }

    public void OnDpadUp(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            ToolSelector.Instance.SetSelectedTool(0);
        }
    }

    public void OnDpadDown(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            ToolSelector.Instance.SetSelectedTool(1);
        }
    }

    public void OnDpadLeft(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            ToolSelector.Instance.SetSelectedTool(2);
        }
    }

    public void OnDpadRight(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            ToolSelector.Instance.SetSelectedTool(3);
        }
    }

    private bool rightStick_readyForFlick = true;
    private float rightStick_deadzone = 0.25f;
    public void OnRightstickHorizontal(InputAction.CallbackContext value)
    {
        var hValue = Maths.ApplyDeadzone(value.ReadValue<float>(), rightStick_deadzone);

        if(hValue == 0)
        {
            rightStick_readyForFlick = true;
            return;
        }

        if(rightStick_readyForFlick && hValue > 0)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RightStickHorizontal, InputData = 1f });
            rightStick_readyForFlick = false;
        }
        
        if(rightStick_readyForFlick && hValue < 0)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RightStickHorizontal, InputData = -1f });
            rightStick_readyForFlick = false;
        }
    }

    public void OnRB(InputAction.CallbackContext value)
    {
        if (value.performed && BuildToolView.Instance != null)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RBLB, InputData = 1f });
        }
    }
    public void OnLB(InputAction.CallbackContext value)
    {
        if (value.performed && BuildToolView.Instance != null)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RBLB, InputData = -1f });
        }
    }

    public void OnB(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.B, InputData = true });
        }
    }

    public void OnA(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.A, InputData = true });
        }
    }

    public void OnX(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.X, InputData = true });
        }
    }

    public void OnY(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.Y, InputData = true });
        }
    }

    public void OnRightStickClick(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RightStickClick, InputData = true });
        }
    }

    private float trigger_deadzone = 0.2f;
    public void OnRT(InputAction.CallbackContext value)
    {
        var triggerValue = Maths.ApplyDeadzone(value.ReadValue<float>(), trigger_deadzone);

        Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.RT, InputData = triggerValue });

    }
    public void OnLT(InputAction.CallbackContext value)
    {
        var triggerValue = Maths.ApplyDeadzone(value.ReadValue<float>(), trigger_deadzone);

        Messenger.Default.Publish(new ShipEditorToolInputPayload { InputType = ShipEditorToolInputPayload.ToolInputType.LT, InputData = triggerValue });
    }
}
