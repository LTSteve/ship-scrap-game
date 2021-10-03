using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildTool : ITool
{
    //resources
    private Material BuildInProgressFail;
    private Material BuildInProgressSuccess;
    private Transform PartConnectorPrefab;

    //target references
    private Transform BuildPoint;
    private Transform playerPartContainer;

    //build data
    private int connectionPointIndex; //the build point on my new part that i want to connect to the build point target on the parttarget
    private int rotationIndex;
    private ShipPart currentPartTarget;
    private Transform currentBuildPointTarget;

    //ghost part swap stuff
    private Material[] currentPartMaterials;
    private ShipPart GhostPart;
    private ShipPart newComponentPrefab;

    //collisions
    private Vector3 currentColliderCenter;
    private Vector3 currentColliderExtents;

    private bool active = false;

    public BuildTool()
    {
        this.playerPartContainer = PlayerController.Instance.transform.Find("Model");

        //load resources
        BuildInProgressFail = (Material)Resources.Load("ToolResources/BuildInProgressFail", typeof(Material));
        BuildInProgressSuccess = (Material)Resources.Load("ToolResources/BuildInProgressSuccess", typeof(Material));
        PartConnectorPrefab = (Transform)Resources.Load("ToolResources/connection ring", typeof(Transform));
        BuildPoint = GameObject.Instantiate((Transform)Resources.Load("ToolResources/BuildPointIndicator", typeof(Transform)));

        BuildPoint.gameObject.SetActive(false);
    }
    public void Activate()
    {
        if (active) return;

        //subcribe to events
        Messenger.Default.Subscribe<ShipEditorToolInputPayload>(_onInput);
        Messenger.Default.Subscribe<BuildItemSelectedPayload>(_onNewItemSelected);
        Messenger.Default.Subscribe<ShipEditorAimPayload>(_onBuildPointSelect);

        //show UI
        BuildToolView.Instance.Enable();

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
        Messenger.Default.Unsubscribe<BuildItemSelectedPayload>(_onNewItemSelected);
        Messenger.Default.Unsubscribe<ShipEditorAimPayload>(_onBuildPointSelect);

        //hide UI
        BuildToolView.Instance.Disable();
        if(BuildPoint != null)
            BuildPoint.gameObject.SetActive(false);

        //destroy Ghostpart
        if (GhostPart != null)
            GameObject.Destroy(GhostPart.gameObject);
        GhostPart = null;

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
            case ShipEditorToolInputPayload.ToolInputType.X:
                rotationIndex++;
                _showPreview();
                break;
            case ShipEditorToolInputPayload.ToolInputType.Y:
                connectionPointIndex++;
                _showPreview();
                break;
        }
    }

    private void _onNewItemSelected(BuildItemSelectedPayload payload)
    {
        newComponentPrefab = payload.SelectedComponent;

        if(GhostPart != null)
        {
            GameObject.Destroy(GhostPart.gameObject);
            GhostPart = null;
        }

        _buildGhostPart();
        _showPreview();
    }


    private void _aquireTarget()
    {
        var camTransform = SmoothCam.Instance.TiltOffset;

        var aimPayload = new ShipEditorAimPayload();

        //weird interaction, fix this later
        var colls = Physics.RaycastAll(camTransform.position, camTransform.forward, LayerMask.GetMask(new string[] { "ShipPart" })).Where(x => x.collider.gameObject.layer == LayerMask.NameToLayer("ShipPart"));

        if (colls.Any())
        {
            var hitInfo = colls.Where(x => x.distance == colls.Min(x=>x.distance)).First();
            var shipComponent = hitInfo.collider.transform.parent.GetComponent<ShipPart>();
            aimPayload.SelectedComponent = shipComponent;
            aimPayload.NearestBuildPoint = shipComponent.GetNearestBuildPoint(hitInfo.point);
        }

        _onBuildPointSelect(aimPayload);
    }


    private void _onBuildPointSelect(ShipEditorAimPayload payload)
    {
        currentPartTarget = payload.SelectedComponent;
        currentBuildPointTarget = payload.NearestBuildPoint;

        _assignBuildPoint(payload.NearestBuildPoint);

        _showPreview();
            
        SmoothCam.Instance.TemporaryFocus(currentBuildPointTarget);
    }

    private void _showPreview()
    {
        if(GhostPart == null)
        {
            //build new Ghostpart
            _buildGhostPart();
        }

        if(currentBuildPointTarget == null)
        {
            GhostPart.gameObject.SetActive(false);
        }
        else
        {
            _placeGhostPart(newComponentPrefab, currentBuildPointTarget);
        }
    }

    public void Use()
    {
        //make sure we can build
        if (Physics.CheckBox(currentColliderCenter + GhostPart.transform.position, currentColliderExtents, GhostPart.transform.rotation, LayerMask.GetMask(new string[] { "ShipPart" })))
        {
            NotificationScroller.PushNotification("Part blocked!", NotificationScroller.NotificationType.Error);
            return;
        }
        var player = PlayerController.Instance;
        if (player == null || !player.PayScrap(GhostPart.Price))
        {
            NotificationScroller.PushNotification("Can't pay " + GhostPart.Price + " scrap!", NotificationScroller.NotificationType.Error);
            return;
        }

        //reset GhostPart's materials & collider
        _setCurrentPartMaterial(currentPartMaterials);
        //Player.IgnoreCollisionsWithPart(GhostPart);
        GhostPart.GetComponentInChildren<Collider>().enabled = true;

        //move ghost block into ship structure
        GhostPart.transform.parent = playerPartContainer;

        //place Connector
        GameObject.Instantiate(PartConnectorPrefab, currentBuildPointTarget.position, currentBuildPointTarget.rotation, GhostPart.transform);

        currentPartTarget.Children.Add(GhostPart);
        GhostPart.Parent = currentPartTarget;

        //reset GhostPart
        _buildGhostPart();
    }

    private void _buildGhostPart()
    {
        if (newComponentPrefab == null) return;
        
        GhostPart = GameObject.Instantiate(newComponentPrefab, playerPartContainer);
        var collider = GhostPart.GetComponentInChildren<BoxCollider>();
        currentColliderCenter = collider.bounds.center;
        currentColliderExtents = collider.bounds.extents;
        GhostPart.GetComponentInChildren<Collider>().enabled = false;
        currentPartMaterials = GhostPart.GetComponentInChildren<MeshRenderer>().materials;
    }

    private void _placeGhostPart(ShipPart parentComponent, Transform buildPoint)
    {
        GhostPart.gameObject.SetActive(true);
        var otherBuildPoint = GhostPart.GetBuildPoint(connectionPointIndex); //the build point on our current ghost part

        //rotate the ghost part so the build points align (opposite directions)
        GhostPart.transform.rotation =
            buildPoint.rotation * Quaternion.Euler(0f,180f,0f) //set the point we're building on as the reference rotation
            * Quaternion.Inverse(otherBuildPoint.localRotation); //turn relative to where my build point is

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

    private void _assignBuildPoint(Transform newBuildPoint)
    {
        var position = newBuildPoint?.position;
        if (!position.HasValue)
        {
            BuildPoint.gameObject.SetActive(false);
            return;
        }

        BuildPoint.position = position.Value;
        BuildPoint.gameObject.SetActive(true);
    }
}
