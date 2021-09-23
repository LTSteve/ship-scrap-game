using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipEditor))]
public class ShipEditorInputMap : MonoBehaviour
{
    private ShipEditor ShipEditor;

    private void Start()
    {
        ShipEditor = GetComponent<ShipEditor>();
    }

    public void OnSelect(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            //floop to shipeditor
            ShipEditor.ToggleActivation();
        }
    }
}
