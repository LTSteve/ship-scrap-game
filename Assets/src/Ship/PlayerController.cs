using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Ship
{
    public static PlayerController Instance;

    [SerializeField]
    private Transform CameraPosition;
    [SerializeField]
    private Transform CameraTarget;

    private int Scrap = 0;

    private void Awake()
    {
        Instance = this;
    }

    protected override void LateStart()
    {
        base.LateStart();
        AddScrap(10);
    }

    protected override void Update()
    {
        if (!control) return;

        base.Update();

        if(Input.GetButtonDown("Toggle Gyro"))
        {
            InputState.GyrosActive = !InputState.GyrosActive;
        }

        InputState.Pitch = Input.GetAxisRaw("Pitch");
        InputState.Roll = Input.GetAxisRaw("Roll");
        InputState.Yaw = Input.GetAxisRaw("Yaw");

        InputState.Thrust = Input.GetAxisRaw("Thrust");
        InputState.VerticalThrust = Input.GetAxisRaw("Vertical Thrust");
        InputState.HorizontalThrust = Input.GetAxisRaw("Horizontal Thrust");

        InputState.Fire1 = Input.GetButton("Fire2"); //joystick 'b' rn

        PowerMeter.Instance.SetState(myState.MaxPower == 0 ? 0f : (myState.CurrentPower / myState.MaxPower));
    }

    public void AddScrap(int value)
    {
        Scrap += value;
        ScrapCounter.Instance.SetScrap(Scrap);
    }

    public bool PayScrap(int value)
    {
        if(Scrap < value)
        {
            return false;
        }

        Scrap -= value;
        ScrapCounter.Instance.SetScrap(Scrap);

        return true;
    }

    public bool HasScrap(int value)
    {
        return Scrap >= value;
    }

    public override void ToggleControl()
    {
        base.ToggleControl();

        if(control) 
        {
            //set my current camera position at an angle
            CameraPosition.localPosition = new Vector3(0, 3, -7).normalized * GetSize() * 4f;

            //grab camera
            SmoothCam.Instance.SetReference(CameraPosition, CameraTarget);

            //activate UI
            PowerMeter.Instance.SetState(myState.MaxPower == 0 ? 0f : (myState.CurrentPower / myState.MaxPower));
            PowerDrawIconBar.Instance.Activate();
            PowerMeter.Instance.Activate();

            //turn on gun crosshairs
            _toggleCrosshairs(true);
        }
        else
        {
            //deactivate UI
            PowerDrawIconBar.Instance.Deactivate();
            PowerMeter.Instance.Deactivate();

            //turn off gun crosshairs
            _toggleCrosshairs(false);
        }
    }

    private void _toggleCrosshairs(bool value)
    {
        if (ShipRoot != null)
        {
            var guns = transform.Find("Model").gameObject.GetComponentsInChildren<FixedLaserGun>();

            foreach (var g in guns)
            {
                var crosshairs = g.transform.Find("crosshairs");
                foreach (Transform c in crosshairs)
                {
                    c.gameObject.SetActive(value);
                }
            }
        }

    }
}
