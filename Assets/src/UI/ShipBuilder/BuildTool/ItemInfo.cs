using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public Transform DataPrefab;
    public Text Name;

    public void Data(ShipComponent component)
    {
        Name.text = component.Name;

        var datas = component.GetData();

        foreach (var data in datas)
        {
            var newInfo = Instantiate(DataPrefab, transform);

            var textScale = newInfo.Find("TextScale");

            textScale.Find("Label").GetComponent<Text>().text = data.Label;
            textScale.Find("Value").GetComponent<Text>().text = data.Value;

            newInfo.gameObject.SetActive(true);
        }

        DataPrefab.gameObject.SetActive(false);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public bool Toggle()
    {
        var active = !gameObject.activeSelf;

        gameObject.SetActive(active);

        return active;
    }
}
