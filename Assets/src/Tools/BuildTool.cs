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

            _placeGhostPart(shipComponent, buildPoint);
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
            if (Physics.CheckBox(currentColliderCenter + GhostPart.transform.position, currentColliderExtents, GhostPart.transform.rotation, LayerMask.GetMask(new string[] { "ShipPart" })))
            {
                return;
            }
            var player = PlayerController.Instance;
            if(player == null || !player.PayScrap(GhostPart.Price))
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
        currentPartMaterials = GhostPart.GetComponentInChildren<MeshRenderer>().materials;
    }

    private void _placeGhostPart(ShipComponent parentComponent, Transform buildPoint)
    {
        GhostPart.gameObject.SetActive(true);
        var otherBuildPoint = GhostPart.GetBuildPoint(connectionPointIndex); //the build point on our current ghost part

        //rotate the ghost part so the build points align (opposite directions)
        GhostPart.transform.rotation = 
            buildPoint.rotation //set the point we're building on as the reference rotation
            * Quaternion.Inverse(otherBuildPoint.localRotation) //turn relative to where my build point is
            * Quaternion.Euler(180, 0, 0); //then turn 180

        //rotate the ghost part around the connection axis based on rotation index
        GhostPart.transform.rotation *= Quaternion.AngleAxis(90f * rotationIndex, Quaternion.Inverse(GhostPart.transform.rotation) * buildPoint.forward);

        //move the ghost part so buildpoint.position == otherbuildpoint.position
        var shipRelativeBuildPointPosition = buildPoint.position - playerPartContainer.position;
        var buildPointRelativeGhostPartCenter = GhostPart.transform.position - otherBuildPoint.position;
        GhostPart.transform.position = playerPartContainer.position + shipRelativeBuildPointPosition + buildPointRelativeGhostPartCenter;

        //assign material based on collision & affordability

        var player = PlayerController.Instance;

        var collisions = Physics.OverlapBox(GhostPart.transform.TransformDirection(currentColliderCenter) + GhostPart.transform.position, currentColliderExtents, GhostPart.transform.rotation, LayerMask.GetMask(new string[] { "ShipPart" }));

        if (player == null || !player.HasScrap(GhostPart.Price) || (collisions == null || collisions.Length > 0))
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
        var partRenderer = GhostPart.GetComponentInChildren<MeshRenderer>();
        var materialsArray = new Material[partRenderer.materials.Length];
        for (var i = 0; i < partRenderer.materials.Length; i++)
        {
            materialsArray[i] = m;
        }
        _setCurrentPartMaterial(materialsArray);
    }
    private void _setCurrentPartMaterial(Material[] m)
    {
        var partRenderer = GhostPart.GetComponentInChildren<MeshRenderer>();
        partRenderer.materials = m;
    }
}
