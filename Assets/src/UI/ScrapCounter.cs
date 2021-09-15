using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapCounter : MonoBehaviour
{
    public static ScrapCounter Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private Text scrapCount;
    [SerializeField]
    private RectTransform scrapBackground;

    [SerializeField]
    private float countRate = 1f;

    private float currentScrap = -0.01f;
    private float nextScrap = 0;
    private float diff = 0f;

    public void SetScrap(int newScrap)
    {
        nextScrap = newScrap;
        diff = 1f;
    }

    private void Update()
    {
        if (currentScrap == nextScrap) return;

        var diffScale = 4f - ((Mathf.Clamp(Mathf.Abs(diff), 1f, 25f) - 1f) / 24f) * 3f;

        diff -= Time.deltaTime * countRate * diffScale;

        currentScrap = Mathf.Lerp(currentScrap, nextScrap, 1f - diff);

        var textLength = _setText();

        var neededWidth = textLength * scrapCount.fontSize / 2f + 20f;

        scrapBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, neededWidth);
    }

    private int _setText()
    {
        scrapCount.text = ((int)currentScrap).ToString();
        return scrapCount.text.Length;
    }
}
