using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShop : MonoBehaviour
{
    public static ItemShop Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<string> partSubdirectories;

    public float zPlacement = 5f;

    public Transform ItemsList;

    private Dictionary<ShipPart, Transform> itemsRef = new Dictionary<ShipPart, Transform>();

    private void Start()
    {
        for(var i = 0; i < partSubdirectories.Count; i++)
        {
            var partSubdirectory = partSubdirectories[i];
            var parts = Resources.LoadAll("ShipParts/" + partSubdirectory, typeof(ShipPart));

            for (var j = 0; j < parts.Length; j++)
            {
                var p = (ShipPart)parts[j];

                itemsRef[p] = _spawnModel(p, new Vector2Int(i, j));
            }
        }

        foreach(var transform in ItemsList.GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = 10;
        }
    }

    private Transform _spawnModel(ShipPart p, Vector2Int position)
    {
        var model = Instantiate(p.transform.Find("modelscale"), ItemsList);
        model.localScale = Vector3.one * 0.125f;
        model.localPosition = new Vector3(position.x, position.y, zPlacement);
        return model;
    }

    public Transform GetModel(ShipPart component)
    {
        return itemsRef[component];
    }
}
