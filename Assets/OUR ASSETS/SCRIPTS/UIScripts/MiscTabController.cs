using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiscTabController : MonoBehaviour
{
    public static MiscTabController Instance;

    public SwitchManager enableTutoralSwitch;

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

    public void Toggle_AllowTraining()
    {
        TutorialManager.Instance.allowTutorials = !TutorialManager.Instance.allowTutorials;
    }

    public void ModalOpenResetTutorials()
    {
        UIManager.Instance.OpenModal("RESET TUTORIAL", "Are you sure you want to reset tutorials?", MiscTabController.Instance.ResetTutorials);
    }

    public void ResetTutorials()
    {
        ES3.DeleteKey("tutorialsSeen");
        DataPersistenceManager.Instance.tutorialsSeen.Clear();
    }

}
