using SuperMaxim.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

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
    private float TiltSpeed = 30f;

    [SerializeField]
    private Transform ForwardPoint;

    private bool active = false;

    private bool zooming = false;
    private bool aiming = false;
    public Vector2 MoveTarget = Vector2.zero;
    private Vector2 aimTilt = Vector2.zero;

    private float minZoom = 1f;
    private float maxZoom = 15f;
    private float currentZoom = 6f;

    private Dictionary<string, ShipPart> ShipComponentPrefabDatabase = new Dictionary<string, ShipPart>();

    private int toolCount = 0;//PartCount { get; private set; } = 0;
    private int toolIndex = 0;
    private ITool CurrentTool;

    [SerializeField]
    private Transform shipBuilderUI;

    private void Start()
    {
        CameraTarget = Player.transform;
        Messenger.Default.Subscribe<NewShipBuildTargetPayload>(_onTargetPartChange);
        Messenger.Default.Subscribe<ShipEditorToolInputPayload>(_onInput);
    }

    private void Update()
    {
        if (!active) return;

        _moveCamera();
    }

    private void _onInput(ShipEditorToolInputPayload payload)
    {
        if(payload.InputType == ShipEditorToolInputPayload.ToolInputType.RT)
        {
            aiming = ((float)payload.InputData) > 0.25f;
        }
        if(payload.InputType == ShipEditorToolInputPayload.ToolInputType.LT)
        {
            ToggleZoom(((float)payload.InputData) > 0.5f);
        }
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
            maxZoom = minZoom * 15f;

            currentZoom = minZoom * 6f;
            CameraPosition.position = Player.transform.position + (Vector3.one.normalized * currentZoom);

            //push notification
            NotificationScroller.PushNotification("Edit Mode");
        }
        else
        {
            zooming = false;
            MoveTarget = Vector2.zero;

            var blueprintManager = Player.GetComponent<ShipBlueprintManager>();
            if(blueprintManager != null && blueprintManager.AutoSave)
            {
                blueprintManager.SaveBlueprint();
            }
        }

        _initializeUI(active);

        //toggle crosshairs
        ForwardPoint.gameObject.SetActive(active);

        return active;
    }

    private void _onTargetPartChange(NewShipBuildTargetPayload payload)
    {
        if(payload.Target == null)
        {
            CameraTarget = Player.transform;
        }
        else
        {
            CameraTarget = payload.Target.transform;
        }

        SmoothCam.Instance.SetReference(CameraPosition, CameraTarget, false, true);
    }

    private void _initializeUI(bool active)
    {
        shipBuilderUI.gameObject.SetActive(active);
    }

    private void _moveCamera()
    {
        var zoomSpeed = (maxZoom - minZoom) / 5f;

        var inOut = zooming ? (-MoveTarget.y) : 0f;

        currentZoom = Mathf.Clamp(currentZoom + zoomSpeed * inOut * ZoomSpeed * Time.deltaTime, minZoom, maxZoom);

        if (aiming)
        {
            var shipSize = PlayerController.Instance.GetSize() * 2;

            aimTilt += MoveTarget * TiltSpeed * shipSize * Time.deltaTime;
            aimTilt = new Vector2(Mathf.Clamp(aimTilt.x, -shipSize, shipSize), Mathf.Clamp(aimTilt.y, -shipSize, shipSize));

            SmoothCam.Instance.Tilt(aimTilt);

            var camTransform = SmoothCam.Instance.TiltOffset;

            //weird interaction, fix this later
            var colls = Physics.RaycastAll(camTransform.position, camTransform.forward, LayerMask.GetMask(new string[] { "ShipPart" })).Where(x => x.collider.gameObject.layer == LayerMask.NameToLayer("ShipPart"));

            if (colls.Any())
            {
                var hitInfo = colls.Where(x => x.distance == colls.Min(x => x.distance)).First();
                var shipComponent = hitInfo.collider.transform.parent.GetComponent<ShipPart>();

                Messenger.Default.Publish(new ShipEditorAimPayload { SelectedComponent = shipComponent, NearestBuildPoint = shipComponent.GetNearestBuildPoint(hitInfo.point) });
            }
        }
        else
        {
            aimTilt = Vector2.zero;
            SmoothCam.Instance.Tilt(aimTilt);

            //update rotation target
            var rotateDist = RotateSpeed * Time.deltaTime;

            var desiredMovement = SmoothCam.Instance.transform.localToWorldMatrix * (zooming ? Vector3.zero : (Vector3)MoveTarget) * rotateDist;

            CameraPosition.position = Maths.ClampMovementToSphere(CameraTarget.position, currentZoom, CameraPosition.position, desiredMovement);
        }
    }
}
