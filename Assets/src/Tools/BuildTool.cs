using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTool : ITool
{
    public ShipComponent ComponentPrefab;

    private ShipComponent GhostPart;

    private Transform playerPartContainer;

    private Vector3 currentColliderCenter;
    private Vector3 currentColliderExtents;
    private Material[] currentPartMaterials;

    private int connectionPointIndex;
    private int rotationIndex;

    private Material BuildInProgressFail;
    private Material BuildInProgressSuccess;

    private Transform PartConnector;

    public BuildTool(int cost, ShipComponent componentPrefab, Transform playerTransform)
    {
        ComponentPrefab = componentPrefab;
        this.playerPartContainer = playerTransform.Find("Model");

        //load resources
        BuildInProgressFail = (Material)Resources.Load("ToolResources/BuildInProgressFail", typeof(Material));
        BuildInProgressSuccess = (Material)Resources.Load("ToolResources/BuildInProgressSuccess", typeof(Material));
        PartConnector = (Transform)Resources.Load("ToolResources/connection ring", typeof(Transform));
    }

    public void Activate()
    {
        //build new Ghostpart
        _buildGhostPart();
    }

    public void Deactivate()
    {
        if(GhostPart != null)
            GameObject.Destroy(GhostPart.gameObject);
        GhostPart = null;
    }

    public Transform GetModel()
    {
        return ComponentPrefab.transform.Find("modelscale");
    }

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotationIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            connectionPointIndex++;
        }
    }

    public void ShowPreview(bool hit, RaycastHit hitInfo)
    {
        if (hit && hitInfo.collider.gameObject.layer == 6) //ship part = 6
        {
            var shipComponent = hitInfo.collider.GetComponentInParent<ShipComponent>();
            var buildPoint = shipComponent.GetNearestBuildPoint(hitInfo.point);

            _placeGhostPart(buildPoint);
        }
        else
        {
            GhostPart.gameObject.SetActive(false);
        }
    }

    public void Use(bool hit, RaycastHit hitInfo)
    {
        if (hit && hitInfo.collider.gameObject.layer == 6) //ship part = 6
        {
            var shipComponent = hitInfo.collider.GetComponentInParent<ShipComponent>();
            var buildPoint = shipComponent.GetNearestBuildPoint(hitInfo.point);

            //make sure we can build
            if (Physics.CheckBox(currentColliderCenter + GhostPart.transform.position, currentColliderExtents, GhostPart.transform.rotation))
            {
                return;
            }

            //reset GhostPart's materials & collider
            _setCurrentPartMaterial(currentPartMaterials);
            //Player.IgnoreCollisionsWithPart(GhostPart);
            GhostPart.GetComponentInChildren<Collider>().enabled = true;

            //move ghost block into ship structure
            GhostPart.transform.parent = playerPartContainer;

            //place Connector
            GameObject.Instantiate(PartConnector, buildPoint.position, buildPoint.rotation, GhostPart.transform);

            shipComponent.Children.Add(GhostPart);
            GhostPart.Parent = shipComponent;

            //reset GhostPart
            _buildGhostPart();
        }
    }

    private void _buildGhostPart()
    {
        GhostPart = GameObject.Instantiate(ComponentPrefab, playerPartContainer);
        var collider = GhostPart.GetComponentInChildren<BoxCollider>();
        currentColliderCenter = collider.bounds.center;
        currentColliderExtents = collider.bounds.extents;
        GhostPart.GetComponentInChildren<Collider>().enabled = false;
        currentPartMaterials = GhostPart.GetComponentInChildren<Renderer>().materials;
    }

    private void _placeGhostPart(Transform buildPoint)
    {
        GhostPart.gameObject.SetActive(true);
        var otherBuildPoint = GhostPart.GetBuildPoint(connectionPointIndex);

        //these are both kinda dumb, and i still don't fully understand them
        GhostPart.transform.rotation = buildPoint.rotation * otherBuildPoint.localRotation * Quaternion.Euler(0, 180, 0);
        GhostPart.transform.position = buildPoint.position + GhostPart.transform.rotation * -new Vector3(-otherBuildPoint.localPosition.x, otherBuildPoint.localPosition.y, otherBuildPoint.localPosition.z);

        GhostPart.transform.rotation *= Quaternion.Euler(0, 0, 90f * rotationIndex);

        //assign material based on collision

        var collisions = Physics.OverlapBox(currentColliderCenter + GhostPart.transform.position, currentColliderExtents, GhostPart.transform.rotation);
        if (collisions != null && collisions.Length > 0)
        {
            _setCurrentPartMaterial(BuildInProgressFail);
        }
        else
        {
            _setCurrentPartMaterial(BuildInProgressSuccess);
        }
    }

    private void _setCurrentPartMaterial(Material m)
    {
        var partRenderer = GhostPart.GetComponentInChildren<Renderer>();
        var materialsArray = new Material[partRenderer.materials.Length];
        for (var i = 0; i < partRenderer.materials.Length; i++)
        {
            materialsArray[i] = m;
        }
        _setCurrentPartMaterial(materialsArray);
    }
    private void _setCurrentPartMaterial(Material[] m)
    {
        var partRenderer = GhostPart.GetComponentInChildren<Renderer>();
        partRenderer.materials = m;
    }
}
