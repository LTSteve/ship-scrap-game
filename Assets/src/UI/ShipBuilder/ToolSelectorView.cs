using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SuperMaxim.Messaging;

public class ToolSelectorView : MonoBehaviour
{
    public static ToolSelectorView Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private List<Sprite> Dpads;

    [SerializeField]
    private Image DpadImage;


    private void Start()
    {
        Messenger.Default.Subscribe<ToolChangedPayload>(_onSelectedToolChanged);
    }

    private void _onSelectedToolChanged(ToolChangedPayload payload)
    {
        DpadImage.sprite = Dpads[payload.Tool];
    }
}
