using SteveD.TJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShipBlueprintManager : MonoBehaviour
{
    [TextArea]
    public string BlueprintJSON;

    public bool RecalculateBlueprint;
    public bool SaveBlueprint;

    private Transform modelParent;
    [SerializeField]
    private ShipComponent shipRoot;

    private void Awake()
    {
        modelParent = transform.Find("Model");
    }

    private void Start()
    {
        RecalculateBlueprint = true;
    }

    private void Update()
    {
        if (RecalculateBlueprint)
        {
            List<ShipComponentModel> blueprint;
            try
            {
                blueprint = (List<ShipComponentModel>)TJSONParser.Parse(BlueprintJSON);
            }
            catch
            {
                return;
            }

            if (blueprint != null && blueprint.Count > 0)
            {
                _clearCurrentShip();

                shipRoot = _buildShip(blueprint);

                GetComponent<Ship>().ShipRoot = shipRoot;
            }

            RecalculateBlueprint = false;
        }

        if (SaveBlueprint)
        {
            if (shipRoot == null) return;

            var genericBlueprint = Maths.CreateTreeNodeList(shipRoot);

            List<ShipComponentModel> blueprint = new List<ShipComponentModel>();
            for(var i = 0; i < genericBlueprint.Length; i++)
            {
                blueprint.Add(((ShipComponent)genericBlueprint[i]).SavePropertiesToModel());
                blueprint[i].TreeAddress = genericBlueprint[i].GetTreeAddress();
            }

            BlueprintJSON = TJSONParser.Encode(blueprint);

            GUIUtility.systemCopyBuffer = BlueprintJSON;

            SaveBlueprint = false;
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

    private ShipComponent _buildShip(List<ShipComponentModel> componentModels)
    {
        BlueprintTree tree = new BlueprintTree();

        for(var i = 0; i < componentModels.Count; i++)
        {
            _buildComponent(componentModels[i], tree);
        }

        return tree.Root;
    }

    private void _buildComponent(ShipComponentModel model, BlueprintTree tree)
    {
        var component = Instantiate((ShipComponent)Resources.Load(model.PrefabLocation, typeof(ShipComponent)), modelParent);

        tree.AddToTree(component, model.TreeAddress);

        component.LoadPropertiesFromModel(model);
    }

    private class BlueprintTree
    {
        public ShipComponent Root;

        private List<ShipComponent> data = new List<ShipComponent>();

        public void AddToTree(ShipComponent component, string address)
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
                    crawlerNode = (ShipComponent) crawlerNode.Children[nextIndex];
                }

                address = address.Substring(1);
            }
        }
    }
}
