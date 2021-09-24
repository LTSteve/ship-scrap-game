using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipEditor : MonoBehaviour
{
    public PlayerController Player;

    [SerializeField]
    private Transform CameraPosition;
    private Transform CameraTarget;

    [SerializeField]
    private float ZoomSpeed = 10f;
    [SerializeField]
    private float RotateSpeed = 100f;

    [SerializeField]
    private Transform ForwardPoint;

    private bool active = false;

    private bool zooming = false;
    public Vector2 MoveTarget = Vector2.zero;

    private float minZoom = 1f;
    private float maxZoom = 10f;
    private float currentZoom = 3f;

    private Dictionary<string, ShipComponent> ShipComponentPrefabDatabase = new Dictionary<string, ShipComponent>();

    private int toolCount = 0;//PartCount { get; private set; } = 0;
    private int toolIndex = 0;
    private ITool CurrentTool;

    [SerializeField]
    private Transform shipBuilderUI;

    private void Start()
    {
        CameraTarget = Player.transform;
    }

    private void Update()
    {
        if (!active) return;

        _moveCamera();
    }

    public void ToggleZoom(bool active)
    {
        zooming = active;
    }

    public bool ToggleActivation()
    {
        active = !active;
        Player.ToggleControl();

        if (active)
        {
            //grab camera
            SmoothCam.Instance.SetReference(CameraPosition, CameraTarget, false, true);
                
            //set camera bounds & location
            minZoom = Player.GetSize();
            maxZoom = minZoom * 5f;

            currentZoom = minZoom * 3f;
            CameraPosition.position = Player.transform.position + (Vector3.one.normalized * currentZoom);

            //push notification
            NotificationScroller.Instance.PushNotification("Edit Mode");
        }
        else
        {
            zooming = false;
            MoveTarget = Vector2.zero;
        }

        _initializeUI(active);

        //toggle crosshairs
        ForwardPoint.gameObject.SetActive(active);

        return active;
    }

    private void _initializeUI(bool active)
    {
        shipBuilderUI.gameObject.SetActive(active);
        //Delete Tool
        /*
        var deleteTool = new DeleteTool(Player);
        ItemUI.Instance.AddToolSlot(deleteTool);
        toolCount++;
        */

        //Re-Root Tool
        //  TODO

        //Build Tools
        /*
        var parts = Resources.LoadAll("ShipParts", typeof(ShipComponent));
        foreach (var p in parts)
        {
            ShipComponentPrefabDatabase[p.name] = (ShipComponent)p;
            var buildTool = new BuildTool(100, (ShipComponent)p, Player.transform);
            ItemUI.Instance.AddToolSlot(buildTool);
            toolCount++;
        }
        */
    }

    private void _moveCamera()
    {
        var zoomSpeed = (maxZoom - minZoom) / 5f;

        var inOut = zooming ? (-MoveTarget.y) : 0f;

        currentZoom = Mathf.Clamp(currentZoom + zoomSpeed * inOut * ZoomSpeed * Time.deltaTime, minZoom, maxZoom);

        //update rotation target
        var rotateDist = RotateSpeed * Time.deltaTime;

        var desiredMovement = SmoothCam.Instance.transform.localToWorldMatrix * (zooming ? Vector3.zero : (Vector3)MoveTarget) * rotateDist;

        CameraPosition.position = Maths.ClampMovementToSphere(CameraTarget.position, currentZoom, CameraPosition.position, desiredMovement);
    }
}
