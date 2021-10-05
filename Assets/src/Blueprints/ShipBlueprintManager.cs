using SteveD.TJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShipBlueprintManager : MonoBehaviour
{
    public TJSON BlueprintTJSON;

    public bool AutoSave = true;

    private Transform modelParent;
    private ShipPart shipRoot;

    private void _init()
    {
        if(modelParent == null)
        {
            modelParent = transform.Find("Model");
        }
    }

    public void SaveBlueprint()
    {
        _init();

        if (shipRoot == null)
        {
            if (modelParent.childCount == 0)
            {
                return;
            }

            var shipPart = modelParent.GetChild(0).GetComponent<ShipPart>();
            while (shipPart.Parent != null)
            {
                shipPart = shipPart.Parent;
            }
            shipRoot = shipPart;
        }

        var genericBlueprint = Maths.CreateTreeNodeList(shipRoot);

        List<ShipPartModel> blueprint = new List<ShipPartModel>();
        for (var i = 0; i < genericBlueprint.Length; i++)
        {
            blueprint.Add(((ShipPart)genericBlueprint[i]).SavePropertiesToModel());
            blueprint[i].TreeAddress = genericBlueprint[i].GetTreeAddress();
        }

        BlueprintTJSON.Data = TJSONParser.Encode(new ShipBlueprintList() { Parts = blueprint });

        GUIUtility.systemCopyBuffer = BlueprintTJSON.Data;

        EditorUtility.SetDirty(BlueprintTJSON);
        AssetDatabase.SaveAssets();
    }

    public void RecalculateBlueprint()
    {
        _init();

        List<ShipPartModel> blueprint = null;
        try
        {
            blueprint = ((ShipBlueprintList)TJSONParser.Parse(BlueprintTJSON.Data)).Parts;
        }
        catch { }

        if (blueprint != null && blueprint.Count > 0)
        {
            _clearCurrentShip();

            shipRoot = _buildShip(blueprint);

            GetComponent<Ship>().ShipRoot = shipRoot;
        }
    }

    private void _clearCurrentShip()
    {
        var toDelete = new GameObject[modelParent.childCount];
        var index = 0;
        foreach(Transform child in modelParent)
        {
            toDelete[index] = child.gameObject;
            index++;
        }

        for(var i = 0; i < toDelete.Length;i++)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(toDelete[i]);
            }
            else
            {
                Destroy(toDelete[i]);
            }
        }
    }

    private ShipPart _buildShip(List<ShipPartModel> componentModels)
    {
        BlueprintTree tree = new BlueprintTree();

        for(var i = 0; i < componentModels.Count; i++)
        {
            _buildComponent(componentModels[i], tree);
        }

        return tree.Root;
    }

    private void _buildComponent(ShipPartModel model, BlueprintTree tree)
    {
        var component = Instantiate((ShipPart)Resources.Load(model.PrefabLocation, typeof(ShipPart)), modelParent);

        tree.AddToTree(component, model.TreeAddress);

        component.LoadPropertiesFromModel(model);
    }

    private class BlueprintTree
    {
        public ShipPart Root;

        private List<ShipPart> data = new List<ShipPart>();

        public void AddToTree(ShipPart component, string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                Root = component;
                data.Add(component);
                return;
            }

            var crawlerNode = Root;

            while (!string.IsNullOrEmpty(address))
            {
                var nextIndex = int.Parse(address.Substring(0,1));

                if(address.Length == 1)
                {
                    crawlerNode.Children.Add(component);
                    component.Parent = crawlerNode;
                    data.Add(component);
                    break;
                }
                else
                {
                    crawlerNode = (ShipPart) crawlerNode.Children[nextIndex];
                }

                address = address.Substring(1);
            }
        }
    }
}
