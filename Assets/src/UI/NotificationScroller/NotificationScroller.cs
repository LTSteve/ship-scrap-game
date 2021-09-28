using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationScroller : MonoBehaviour
{
    private static NotificationScroller Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum NotificationType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    [SerializeField]
    private Notification notificationPrefab;

    [SerializeField]
    private float notificationTime = 4f;

    [SerializeField]
    private List<Color> notificationColors;

    public static void PushNotification(string message, NotificationType type = NotificationType.Info, float time = -1f)
    {
        if (Instance == null) return;

        if (time < 0f)
            time = Instance.notificationTime;

        var newNotification = Instantiate(Instance.notificationPrefab, Instance.transform);

        newNotification.Initialize(message, Instance.notificationColors[(int)type], time);
    }
}
