using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    public GameObject mm_Hit_Prefab;
    public GameObject mm_Death_Prefab;
    public GameObject mm_HoverCard_Prefab;
    public GameObject mm_ClickCard_Prefab;
    public GameObject mm_ActivatedUI_Prefab;
    public GameObject mm_MovingFast_Prefab;
    public GameObject mm_OpenPanel_Prefab;
    public GameObject mm_ClosePanel_Prefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayOnTarget(Transform target, GameObject mmPrefab)
    {
        if (!target || !mmPrefab) return;

        // Look for existing
        Transform existingChild = target.Find(mmPrefab.name);
        MMF_Player mmInstance;

        if (existingChild == null)
        {
            GameObject newInstance = Instantiate(mmPrefab, target);
            newInstance.name = mmPrefab.name;
            newInstance.transform.localPosition = Vector3.zero;
            newInstance.transform.localRotation = Quaternion.identity;

            mmInstance = newInstance.GetComponent<MMF_Player>();
        }
        else
        {
            mmInstance = existingChild.GetComponent<MMF_Player>();
        }

        if (!mmInstance) return;

        // Always assign targets fresh
        foreach (MMF_Feedback feedback in mmInstance.FeedbacksList)
        {

            if (feedback.Label == "Squash and Stretch")
            {

                var squash = feedback as MMF_SquashAndStretch;
                if (squash != null)
                {
                    squash.SquashAndStretchTarget = target;
                }

            }
            else if (feedback.Label == "Scale")
            {

                var scale = feedback as MMF_Scale;
                if (scale != null)
                {
                    scale.AnimateScaleTarget = target;
                }

            }
            else if (feedback.Label == "Chromatic Aberration")
            {
                    feedback.Active = SystemManager.Instance.options_allow_chromatic_aberration;
            }
            else if (feedback.Label == "Lens Distortion")
            {
                    feedback.Active = SystemManager.Instance.options_allow_lens_distortion;
            }
        }

        // This is the critical part: re-initialize AFTER assigning
        mmInstance.Initialization();

        mmInstance.PlayFeedbacks();
    }

}
