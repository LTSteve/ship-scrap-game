using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Ship
{
    [SerializeField]
    private Transform CameraPosition;
    [SerializeField]
    private Transform CameraTarget;

    private int Scrap = 0;

    private bool initialCrosshairsSpawnBecauseSomethingIsWack = false;

    protected override void Update()
    {
        if (!control) return;

        if (!initialCrosshairsSpawnBecauseSomethingIsWack) _toggleCrosshairs(true);

        base.Update();

        if(Input.GetButtonDown("Toggle Gyro"))
        {
            GyrosActive = !GyrosActive;
        }

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

    public override void ToggleControl()
    {
        base.ToggleControl();

        if(control) 
        {
            //grab camera
            SmoothCam.Instance.SetReference(CameraPosition, CameraTarget);

            //activate UI
            PowerMeter.Instance.SetState(myState.MaxPower == 0 ? 0f : (myState.CurrentPower / myState.MaxPower));
            PowerDrawIconBar.Instance.Activate();
            PowerMeter.Instance.Activate();

            //turn on gun crosshairs
            initialCrosshairsSpawnBecauseSomethingIsWack = false;
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
            initialCrosshairsSpawnBecauseSomethingIsWack = true;
        }

    }
}
