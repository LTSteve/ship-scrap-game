using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [SerializeField]
    private Renderer ScrapRenderer;
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshedRenderer;

    [SerializeField]
    private float scaleUpperRange = 1.1f;
    [SerializeField]
    private float scaleLowerRange = 0.6f;

    [SerializeField]
    private float succForce = 2f;

    public float InitialVelocity = 1f;
    public int ScrapValue = 1;

    private float maxSuccRange = 1f;
    private Transform succTarget = null;

    private Rigidbody body;

    private void Start()
    {
        maxSuccRange = GetComponent<SphereCollider>().radius;

        //random rotation
        var randomRotation = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
        transform.rotation = Quaternion.LookRotation(randomRotation);

        //random color
        var randomGoodColor = Random.Range(0, 2);
        var randomColor = new Color(randomGoodColor == 0 ? 1f : Random.value, randomGoodColor == 1 ? 1f : Random.value, randomGoodColor == 2 ? 1f : Random.value);
        var material = ScrapRenderer.material;
        material.color = randomColor;

        //random scale
        var scale = Random.Range(scaleLowerRange, scaleUpperRange);
        if (ScrapValue > 10)
        {
            scale *= 1.5f;
        }
        if (ScrapValue > 100)
        {
            scale *= 2;
        }
        transform.Find("modelscale").localScale = new Vector3(scale, scale, scale);

        //random shape
        skinnedMeshedRenderer.SetBlendShapeWeight(0, Random.value * 100f);
        skinnedMeshedRenderer.SetBlendShapeWeight(1, Random.value * 100f);
        skinnedMeshedRenderer.SetBlendShapeWeight(2, Random.value * 100f);

        //random impulse & torque
        body = GetComponent<Rigidbody>();
        body.AddForce(new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * InitialVelocity, ForceMode.VelocityChange);
        body.AddTorque(new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f), ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        if(succTarget == null && body.velocity.magnitude > InitialVelocity)
        {
            body.velocity = body.velocity.normalized * InitialVelocity;
        }

        if (succTarget == null)
            return;

        //succ force

        var offset = succTarget.position - transform.position;
        var distanceScale = offset.magnitude / maxSuccRange;
        var direction = offset.normalized;
        var force = Mathf.Clamp(succForce / Mathf.Pow(distanceScale, 2.5f), 1f, float.MaxValue);

        body.AddForce(direction * force * Time.fixedDeltaTime, ForceMode.Force);

        if(distanceScale < 0.25f)
        {
            var ship = succTarget.GetComponent<PlayerController>();

            //succ
            ship.AddScrap(ScrapValue);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            var shipComponent = other.gameObject.GetComponentInParent<ShipComponent>();
            var ship = shipComponent.GetShip();

            if(ship == null || !(ship is PlayerController))
            {
                return;
            }

            //assign succ target
            succTarget = ship.transform;
        }
    }
}
