using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SystemManager;
using Michsky.MUIP; // MUIP namespace

public class NotificationSystemManager : MonoBehaviour
{

    public static NotificationSystemManager Instance;

    [SerializeField] private GameObject notificationParent;
    [SerializeField] private NotificationManager notificationWarning;
    [SerializeField] private NotificationManager notificationError;
    [SerializeField] private NotificationManager notificationSuccess;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void ShowNotification(NotificationOperation notificationOperation, string title, string description)
    {
        GameObject prefabToUse = notificationWarning.gameObject;

        if (notificationOperation == NotificationOperation.ERROR)
            prefabToUse = notificationError.gameObject;
        else if (notificationOperation == NotificationOperation.SUCCESS)
            prefabToUse = notificationSuccess.gameObject;

        GameObject notificationGO = Instantiate(prefabToUse);
        notificationGO.transform.SetParent(notificationParent.transform, false); // Keep local layout
        notificationGO.SetActive(true);

        NotificationManager notificationManager = notificationGO.GetComponent<NotificationManager>();
        notificationManager.startBehaviour = NotificationManager.StartBehaviour.None;
        notificationManager.title = title;
        notificationManager.description = description;
        notificationManager.UpdateUI(); // CRITICAL for the UI to refresh
        //notificationManager.Open();
      
    }


    public void TestRandom()
    {
        int random = UnityEngine.Random.Range(0, 3);

        NotificationOperation notificationOperation = NotificationOperation.WARNING;

        if (random == 1)
        {
            notificationOperation = NotificationOperation.ERROR;
        }
        else if (random == 2)
        {
            notificationOperation = NotificationOperation.SUCCESS;
        }

        ShowNotification(notificationOperation,"Test", "LOTS OF BLA BLA BLA BLA BLA!");
    }

}
