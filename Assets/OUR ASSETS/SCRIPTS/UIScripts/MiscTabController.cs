using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiscTabController : MonoBehaviour
{
    public static MiscTabController Instance;

    public SwitchManager enableTutoralSwitch;
    public SwitchManager enableLensDistortion;
    public SwitchManager enableChromaticAberration;

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

    public void Toggle_AllowLensDistortion()
    {
        SystemManager.Instance.options_allow_lens_distortion = !SystemManager.Instance.options_allow_lens_distortion;
    }

    public void Toggle_AllowChromaticAberration()
    {
        SystemManager.Instance.options_allow_chromatic_aberration = !SystemManager.Instance.options_allow_chromatic_aberration;
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
