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
}
