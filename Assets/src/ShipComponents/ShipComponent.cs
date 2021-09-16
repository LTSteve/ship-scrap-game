using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour, ITreeNode
{
    private Transform buildPoints;

    //Tree Properties
    [SerializeField]
    private ShipComponent parent;
    [SerializeField]
    public List<ShipComponent> children = new List<ShipComponent>();

    public ShipComponent Parent { get { return parent; } set { parent = value; } }
    public List<ShipComponent> Children { get { return children; } set { children = value; } }

    [HideInInspector]
    public Ship MyShip;

    public float Mass = 1f;
    public int Price = 10;
    [SerializeField]
    protected string prefabLocation = "ShipParts/structural block";

    private Transform scrapPrefab;

    private void Awake()
    {
        buildPoints = transform.Find("connectionpoints");
        scrapPrefab = (Transform)Resources.Load("Scrap", typeof(Transform));
    }

    public Transform GetNearestBuildPoint(Vector3 point)
    {
        var nearestDistance = float.MaxValue;
        Transform nearest = null;

        foreach(Transform buildPoint in buildPoints)
        {
            var nextdist = Vector3.Distance(buildPoint.position, point);
            if (nextdist < nearestDistance)
            {
                nearestDistance = nextdist;
                nearest = buildPoint;
            }
        }

        return nearest;
    }

    public Transform GetBuildPoint(int index)
    {
        return buildPoints.GetChild(index % buildPoints.childCount);
    }

    public virtual void ApplyShipStats(ShipState shipState)
    {
        var workingCenterOfMass = shipState.CenterOfMass * shipState.Mass + transform.localPosition * Mass;

        shipState.Mass += Mass;

        shipState.CenterOfMass = workingCenterOfMass / shipState.Mass;
    }

    public void Explode(float force = 1f, bool fullRefund = false)
    {
        transform.Find("collider").gameObject.SetActive(false);

        var workingValue = fullRefund ? Price : Mathf.Clamp((int)(Price * 0.5f), 0, int.MaxValue);
        var maxIterations = Mathf.Clamp((int)Mathf.Sqrt(workingValue), 5, 100);
        var iterations = 0;
        while (workingValue > 5)
        {
            int nextPiece = 0;

            if (workingValue > 125 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(100, workingValue);
            }
            else if (workingValue > 50 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(50, workingValue);
            }
            else if (workingValue > 10 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(10, workingValue);
            }
            else
            {
                nextPiece = Random.Range(1, workingValue);
            }

            _spawnScrap(force, nextPiece);
            workingValue -= nextPiece;

            iterations++;
            if (iterations > maxIterations)
            {
                break;
            }
        }

        if (workingValue > 0)
        {
            _spawnScrap(0f, workingValue);
        }

        Destroy(this.gameObject);
    }

    public Ship GetShip()
    {
        return MyShip;
    }

    public virtual void LoadPropertiesFromModel(ShipComponentModel model)
    {
        prefabLocation = model.PrefabLocation;

        transform.localPosition = model.Offset;
        transform.localRotation = model.Rotation;
    }

    public virtual ShipComponentModel SavePropertiesToModel()
    {
        var model = new ShipComponentModel();

        model.PrefabLocation = prefabLocation;

        model.Offset = transform.localPosition;
        model.Rotation = transform.localRotation;

        return model;
    }

    public string GetTreeAddress()
    {
        var workingAddress = "";

        var workingParent = Parent;

        var workingChild = this;

        while(workingParent != null)
        {
            workingAddress = "" + workingParent.Children.IndexOf(workingChild) + workingAddress;
            workingChild = workingParent;
            workingParent = workingParent.Parent;
        }

        return workingAddress;
    }

    private void _spawnScrap(float velocity, int value)
    {
        Instantiate(scrapPrefab, transform.position, Quaternion.identity);
    }

    public ShipComponent GetParent()
    {
        return Parent;
    }

    public void SetParent(ShipComponent parent)
    {
        Parent = parent;
    }

    public List<ShipComponent> GetChildren()
    {
        return Children;
    }

    public void SetChildren(List<ShipComponent> children)
    {
        Children = children;
    }
}
