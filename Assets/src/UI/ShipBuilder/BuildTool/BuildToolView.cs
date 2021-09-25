using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolView : AbstractToolView
{
    public static BuildToolView Instance;

    private void Awake()
    {
        Instance = this;
    }

    private int currentPanel = 0;

    private List<BuildCategoryPanel> panels = new List<BuildCategoryPanel>();

    public float MoveRate = 200f;

    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform shipView;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        var buildCategoryPanels = transform.GetComponentsInChildren<BuildCategoryPanel>();
        foreach (var panel in buildCategoryPanels)
        {
            panels.Add(panel);
        }
    }

    public override void Enable()
    {
        ThingSlider.DoMeASlide(shipView, shipView.anchoredPosition, shipView.anchoredPosition + new Vector2(0f, 128f), MoveRate);
        ThingSlider.DoMeASlide(rectTransform, rectTransform.anchoredPosition, new Vector2(rectTransform.anchoredPosition.x, 128f), MoveRate);
    }

    public override void Disable()
    {
        ThingSlider.DoMeASlide(shipView, shipView.anchoredPosition, shipView.anchoredPosition + new Vector2(0f, -128f), MoveRate);
        ThingSlider.DoMeASlide(rectTransform, rectTransform.anchoredPosition, new Vector2(rectTransform.anchoredPosition.x, -128f), MoveRate);
    }

    public void NextItem()
    {

    }

    public void PreviousItem()
    {

    }

    public void NextPanel()
    {
        if(currentPanel == (panels.Count - 1))
        {
            _setPanel(0);
        }
        else
        {
            _setPanel(currentPanel + 1);
        }
    }

    public void PreviousPanel()
    {
        if(currentPanel == 0)
        {
            _setPanel(panels.Count - 1);
        }
        else
        {
            _setPanel(currentPanel - 1);
        }
    }

    private void _setPanel(int nextPanel)
    {
        currentPanel = nextPanel;

        for(int i = 0; i < panels.Count; i++)
        {
            var panelX = i * 30f + (i > currentPanel ? 450f : 0f);

            panels[i].MoveToAndActivate(panelX, i == currentPanel);
        }
    }
}
