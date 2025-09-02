using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    //public Sprite selectedIcon;
    //public Sprite nonSelectedIcon;
    public LTDescr scaleTween;
    public float hoverScale = 1.2f;
    private float transitionTime = 0.1f;
    public bool playFeedbacks = true;
    private Vector3 originalScale;

    public Animator animator;

    TMP_Text txt;
    Image icon;

    public string hoverSoundName = "UI_Hover";
    public bool randomPitch = false;

    void Awake()
    {
        //txt = GetComponentInChildren<TMP_Text>();
        //icon = this.transform.Find("Icon").GetComponent<Image>();
        //animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        originalScale = transform.localScale;
        //icon.sprite = nonSelectedIcon;
        //animator.SetBool("selectedButton", false);
    }

    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playFeedbacks)
        {
            //FeedbackManager.Instance.PlayOnTarget(this.gameObject.transform, FeedbackManager.Instance.mm_HoverUI_Prefab);
        }

        // Scale up the hovered card
        if (hoverScale != 1)
        {
            scaleTween = LeanTween.scale(this.gameObject, originalScale * hoverScale, transitionTime);
        }
        //icon.sprite = selectedIcon;
        //animator.SetBool("selectedButton", true);
        AudioManager.Instance.PlaySfx(hoverSoundName, randomPitch);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale up the hovered card
        scaleTween = LeanTween.scale(this.gameObject, originalScale, transitionTime);

        //icon.sprite = nonSelectedIcon;
        //animator.SetBool("selectedButton", false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //when selecting go back to initial 

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void DisableButton()
    {



    }

    public void BackToOriginalState()
    {
        this.gameObject.transform.localScale = originalScale;
    }






}