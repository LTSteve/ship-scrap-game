using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private static TargetIndicator Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private Transform target;

    public static void SetActive(bool active)
    {
        Instance.gameObject.SetActive(active);
    }

    public static Vector3? GetTargetedPosition()
    {
        var layerMask = new LayerMask() | (1 << LayerMask.NameToLayer("ShipPart"));

        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        var targetingSomething = Physics.Raycast(ray, out var targetingHit, 1000f, layerMask);

        Instance.target.gameObject.SetActive(targetingSomething);

        if (!targetingSomething)
            return null;

        Instance.target.position = Camera.main.transform.position + (targetingHit.point - Camera.main.transform.position).normalized * 2f;
        Instance.target.localRotation *= Quaternion.Euler(0, 0, 170f * Time.fixedDeltaTime);

        return targetingHit.point;
    }
}
