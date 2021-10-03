using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeleteTool : ITool
{
    private Material DeleteMaterial;

    private ITreeNode[] currentlyPreviewing;
    private Material[][] savedMaterials;

    private ShipPart currentPartTarget;

    private bool active;

    public DeleteTool()
    {
        DeleteMaterial = (Material)Resources.Load("ToolResources/BuildInProgressFail", typeof(Material));
    }

    public void Activate()
    {
        if (active) return;

        //subcribe to events
        Messenger.Default.Subscribe<ShipEditorToolInputPayload>(_onInput);
        Messenger.Default.Subscribe<ShipEditorAimPayload>(_onPartSelect);

        //target first part
        _aquireTarget();

        //show Ghostpart
        _showPreview();

        active = true;
    }

    public void Deactivate()
    {
        if (!active) return;

        //unsubscribe from events
        Messenger.Default.Unsubscribe<ShipEditorToolInputPayload>(_onInput);
        Messenger.Default.Unsubscribe<ShipEditorAimPayload>(_onPartSelect);

        if (currentlyPreviewing != null)
        {
            _unHighlight(currentlyPreviewing);
        }

        SmoothCam.Instance.TemporaryFocus(null);

        active = false;
    }

    private void _onInput(ShipEditorToolInputPayload payload)
    {
        switch (payload.InputType)
        {
            case ShipEditorToolInputPayload.ToolInputType.A:
                Use();
                _aquireTarget();
                _showPreview();
                break;
        }
    }

    private void _onPartSelect(ShipEditorAimPayload payload)
    {
        if (payload.SelectedComponent == PlayerController.Instance.ShipRoot) return;

        currentPartTarget = payload.SelectedComponent;

        _showPreview();

        if(currentPartTarget != null)
            SmoothCam.Instance.TemporaryFocus(currentPartTarget.transform);
    }


    private void _aquireTarget()
    {
        var camTransform = SmoothCam.Instance.TiltOffset;

        var aimPayload = new ShipEditorAimPayload();

        //weird interaction, fix this later
        var colls = Physics.RaycastAll(camTransform.position, camTransform.forward, LayerMask.GetMask(new string[] { "ShipPart" })).Where(x => x.collider.gameObject.layer == LayerMask.NameToLayer("ShipPart"));

        if (colls.Any())
        {
            var hitInfo = colls.Where(x => x.distance == colls.Min(x => x.distance)).First();
            var shipComponent = hitInfo.collider.transform.parent.GetComponent<ShipPart>();
            aimPayload.SelectedComponent = shipComponent;
        }

        _onPartSelect(aimPayload);
    }


    public void _showPreview()
    {
        if(currentPartTarget != null)
        {
            if (currentlyPreviewing == null || currentlyPreviewing[0] != currentPartTarget)
            {
                _highlight(currentPartTarget.GetComponent<ShipPart>());
            }
        }
        else
        {
            _unHighlight(currentlyPreviewing);
            currentlyPreviewing = null;
        }
    }

    public void Use()
    {
        if (currentPartTarget == null) return;

        var nodeList = Maths.CreateTreeNodeList(currentPartTarget);

        currentlyPreviewing = null;
        savedMaterials = null;

        currentPartTarget.Parent.Children.Remove(currentPartTarget);

        for (var i = 0; i < nodeList.Length; i++)
        {
            ((ShipPart)nodeList[i]).Explode(10f, true);
        }

        currentPartTarget = null;
    }

    private void _highlight(ShipPart toHighlight)
    {
        if (currentlyPreviewing != null && currentlyPreviewing[0] != (ITreeNode)toHighlight)
        {
            _unHighlight(currentlyPreviewing);
        }

        var nodeList = Maths.CreateTreeNodeList(toHighlight);

        currentlyPreviewing = nodeList;
        savedMaterials = new Material[nodeList.Length][];
        for(var i = 0; i < savedMaterials.Length; i++)
        {
            savedMaterials[i] = ((ShipPart)nodeList[i]).GetComponentInChildren<MeshRenderer>().materials;
        }

        var newMaterials = new Material[savedMaterials.Length];
        for(var i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = DeleteMaterial;
        }

        foreach(var previewee in currentlyPreviewing)
        {
            ((ShipPart)previewee).GetComponentInChildren<MeshRenderer>().materials = newMaterials;
        }

    }

    private void _unHighlight(ITreeNode[] toUnHighlight)
    {
        if (toUnHighlight == null) return;

        for(var i = 0; i < toUnHighlight.Length; i++)
        {
            ((ShipPart)toUnHighlight[i]).GetComponentInChildren<MeshRenderer>().materials = savedMaterials[i];
        }
    }
}
