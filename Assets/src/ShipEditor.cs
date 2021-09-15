using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int inOut = 0;
    private Vector2 mouseDrag = Vector2.zero;

    private float minZoom = 1f;
    private float maxZoom = 10f;
    private float currentZoom = 3f;

    private Dictionary<string, ShipComponent> ShipComponentPrefabDatabase = new Dictionary<string, ShipComponent>();

    private int toolCount = 0;//PartCount { get; private set; } = 0;
    private int toolIndex = 0;
    private ITool CurrentTool;

    private void Start()
    {
        CameraTarget = Player.transform;

        //build the itemUI & load all parts
        _generateItemUITools();
    }

    private void _generateItemUITools()
    {
        //Delete Tool
        var deleteTool = new DeleteTool(Player);
        ItemUI.Instance.AddToolSlot(deleteTool);
        toolCount++;

        //Re-Root Tool
        //  TODO

        //Build Tools
        var parts = Resources.LoadAll("ShipParts", typeof(ShipComponent));
        foreach (var p in parts)
        {
            ShipComponentPrefabDatabase[p.name] = (ShipComponent)p;
            var buildTool = new BuildTool(100, (ShipComponent)p, Player.transform);
            ItemUI.Instance.AddToolSlot(buildTool);
            toolCount++;
        }
    }

    private void Update()
    {
        _handleActivation();

        if (!active) return;

        _handleCameraControls();

        _handleToolActions();

        _moveCamera();
    }

    private void _handleActivation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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

                ItemUI.Instance.SetItemUIOpen(true);
                CurrentTool = ItemUI.Instance.ChangeSelectedItem(toolIndex);
            }
            else
            {
                inOut = 0;
                mouseDrag = Vector2.zero;
                if(CurrentTool != null)
                {
                    CurrentTool.Deactivate();
                }
                ItemUI.Instance.SetItemUIOpen(false);
                toolIndex = 0;
            }

            //toggle crosshairs
            ForwardPoint.gameObject.SetActive(active);
        }
    }

    private void _handleToolActions()
    {
        CurrentTool = ItemUI.Instance.ChangeSelectedItem(toolIndex);

        CurrentTool.HandleInputs();

        var hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo);

        CurrentTool.ShowPreview(hit, hitInfo);

        if(Input.GetMouseButtonDown(0))
        {
            CurrentTool.Use(hit, hitInfo);
        }
    }

    private void _handleCameraControls()
    {
        inOut = 0;
        if (Input.GetKey(KeyCode.W))
        {
            inOut += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inOut -= 1;
        }

        //mouse scroll wheel
        var partChange = (int)Input.mouseScrollDelta.y;
        if(toolCount == 0)
        {
            Debug.LogError("No Tools!");
            return;
        }
        toolIndex = Maths.RollingModulo(toolIndex + partChange, toolCount);

        //dragging right click for rotate
        if (Input.GetMouseButton(1))
        {
            mouseDrag = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        else
        {
            mouseDrag = Vector2.Lerp(mouseDrag, Vector2.zero, 7f * Time.deltaTime);
            //the smoothcam stop code can catch the tail of this lerp
        }
    }

    private void _moveCamera()
    {
        var zoomSpeed = (maxZoom - minZoom) / 5f;

        currentZoom = Mathf.Clamp(currentZoom + zoomSpeed * -inOut * ZoomSpeed * Time.deltaTime, minZoom, maxZoom);

        //update rotation target
        var nextPosition = CameraPosition.position + SmoothCam.Instance.transform.rotation * new Vector3(-mouseDrag.x * RotateSpeed * Time.deltaTime, -mouseDrag.y * RotateSpeed * Time.deltaTime, 0);
        var nextDirection = (nextPosition - CameraTarget.position).normalized;

        CameraPosition.position = CameraTarget.position + nextDirection * currentZoom;
    }
}
