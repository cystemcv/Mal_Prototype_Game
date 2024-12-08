using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public List<ScriptableEntity> characterList;

    public List<ScriptableCompanion> companionList;

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


    public void ProceedWithAnimationAndSound(GameObject entityPlayAnimation, ScriptableCard scriptableCard)
    {

        float m_CurrentClipLength = 0;
        string m_ClipName = "";

        Animator entityAnimator = entityPlayAnimation.transform.Find("model").GetComponent<Animator>();

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(scriptableCard.entityAnimation.ToString());
        }


        // Play the sound
        if (scriptableCard.cardSoundEffect != null)
        {
            AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
        }
       
    }


}
