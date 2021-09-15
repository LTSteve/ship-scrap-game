using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool
{
    public Transform GetModel();
    public void Deactivate();
    public void Activate();
    void ShowPreview(bool hit, RaycastHit hitInfo);
    void Use(bool hit, RaycastHit hitInfo);
    void HandleInputs();
}
