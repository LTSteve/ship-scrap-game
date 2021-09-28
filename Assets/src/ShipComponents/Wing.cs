using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : ShipComponent
{
    private ShipState shipState;

    private Rigidbody shipBody;

    [SerializeField]
    private Transform thrustPoints;

    public float Thrust = 5f;
    public float EnergyConsumption = 0.1f;
    public float CounterThrustDeadzone = 0.01f;

    private WingThruster[] thrusters;

    public override void ApplyShipStats(ShipState shipState)
    {
        base.ApplyShipStats(shipState);

        this.shipState = shipState;

        shipBody = MyShip.GetComponent<Rigidbody>();

        thrusters = new WingThruster[thrustPoints.childCount];

        for(var i = 0; i < thrusters.Length; i++)
        {
            var thrustPoint = thrustPoints.GetChild(i);

            thrusters[i] = new WingThruster {
                ShipRelativeThrustDirection = _calculateRelativeThrustDirection(thrustPoint),
                ShipRelativePosition = thrustPoint.position - MyShip.transform.position,
                ShipBody = shipBody,
                MyShip = MyShip
            };
        }

    }

    private Vector3 _calculateRelativeThrustDirection(Transform thrustPoint)
    {
        return Maths.CardinalizeVector(Maths.WorldDirectionToLocalDirection(thrustPoint.forward, MyShip.transform.rotation));
    }

    private void FixedUpdate()
    {
        if (thrusters == null) return;

        foreach(var wingThruster in thrusters)
        {
            //calculate the velocity counter to the thruster's thrust direction
            var counterVelocity = wingThruster.CalculateCounterVelocity();

            //check if we should counterthrust
            if(counterVelocity > CounterThrustDeadzone)
            {
                if(shipState.CurrentPower > EnergyConsumption * Time.fixedDeltaTime)
                {
                    shipState.CurrentPower -= EnergyConsumption * Time.fixedDeltaTime;

                    shipBody.AddForceAtPosition(
                        MyShip.transform.rotation * -wingThruster.ShipRelativeThrustDirection * Thrust * Time.deltaTime, 
                        MyShip.transform.position + wingThruster.ShipRelativePosition, 
                        ForceMode.Force);
                }
            }
        }
    }

    public override List<ShipComponentData> GetData()
    {
        var toReturn = base.GetData();

        toReturn.Add(new ShipComponentData { Label = "E/s", Value = "" + EnergyConsumption * 2 });
        toReturn.Add(new ShipComponentData { Label = "Tr", Value = "" + Thrust * 2 });

        return toReturn;
    }

    private class WingThruster
    {
        public Vector3 ShipRelativeThrustDirection;
        public Vector3 ShipRelativePosition;
        public Rigidbody ShipBody;
        public Ship MyShip;

        public float CalculateCounterVelocity()
        {
            var worldVelocityAtMe = ShipBody.GetPointVelocity(MyShip.transform.position + ShipRelativePosition);

            var worldThrustDirection = MyShip.transform.rotation * ShipRelativeThrustDirection;

            return Vector3.Dot(worldThrustDirection, worldVelocityAtMe);
        }
    }
}
