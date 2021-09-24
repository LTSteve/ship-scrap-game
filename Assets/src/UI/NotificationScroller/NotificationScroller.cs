using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationScroller : MonoBehaviour
{
    public static NotificationScroller Instance;

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

    public void PushNotification(string message, NotificationType type = NotificationType.Info, float time = -1f)
    {
        if (time < 0f)
            time = notificationTime;

        var newNotification = Instantiate(notificationPrefab, transform);

        newNotification.Initialize(message, notificationColors[(int)type], time);
    }
}
