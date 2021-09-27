using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTool : ITool
{

    private Transform Icon;
    private Material DeleteMaterial;

    private ITreeNode[] currentlyPreviewing;
    private Material[][] savedMaterials;

    private PlayerController Player;

    public DeleteTool()
    {
        Player = PlayerController.Instance;

        Icon = (Transform)Resources.Load("ToolResources/DeleteIcon", typeof(Transform));
        DeleteMaterial = (Material)Resources.Load("ToolResources/BuildInProgressFail", typeof(Material));
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
        if(currentlyPreviewing != null)
        {
            _unHighlight(currentlyPreviewing);
        }
    }

    public void ShowPreview()
    {
        /*
        var shipComponent = BuildToolView.Instance.CurrentPartTarget;

        if(shipComponent != null)
        {
            if (currentlyPreviewing == null || currentlyPreviewing[0] != shipComponent)
            {
                _highlight(shipComponent.GetComponent<ShipComponent>());
            }
        }
        else
        {
            _unHighlight(currentlyPreviewing);
            currentlyPreviewing = null;
        }
        */
    }

    public void Use()
    {
        /*
        var shipComponent = BuildToolView.Instance.CurrentPartTarget;
        var nodeList = Maths.CreateTreeNodeList(shipComponent);

        currentlyPreviewing = null;
        savedMaterials = null;

        shipComponent.Parent.Children.Remove(shipComponent);

        for (var i = 0; i < nodeList.Length; i++)
        {
            ((ShipComponent)nodeList[i]).Explode(10f, true);
        }
        */
    }

    private void _highlight(ShipComponent toHighlight)
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
            savedMaterials[i] = ((ShipComponent)nodeList[i]).GetComponentInChildren<MeshRenderer>().materials;
        }

        var newMaterials = new Material[savedMaterials.Length];
        for(var i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = DeleteMaterial;
        }

        foreach(var previewee in currentlyPreviewing)
        {
            ((ShipComponent)previewee).GetComponentInChildren<MeshRenderer>().materials = newMaterials;
        }

    }

    private void _unHighlight(ITreeNode[] toUnHighlight)
    {
        if (toUnHighlight == null) return;

        for(var i = 0; i < toUnHighlight.Length; i++)
        {
            ((ShipComponent)toUnHighlight[i]).GetComponentInChildren<MeshRenderer>().materials = savedMaterials[i];
        }
    }
}
