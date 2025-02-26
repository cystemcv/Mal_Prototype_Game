using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DoTweenAnimController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public GameObject objectAnimation;

    private DOTweenAnimation[] objectDoAnimations;

    public bool allowHover = false;
    public bool allowClick = false;
    public bool allowEnable = false;

    void Start()
    {



    }

    void OnEnable()
    {
        if (allowEnable && objectDoAnimations != null)
        {
            PlayAnimation();
        }
    }

    void OnDisable()
    {
        if (allowEnable && objectDoAnimations != null)
        {
            PlayAnimationBackward();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (allowHover && objectDoAnimations != null)
        {
            PlayAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (allowHover && objectDoAnimations != null)
        {
            PlayAnimationBackward();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (allowClick && objectDoAnimations != null)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        objectDoAnimations = this.gameObject.GetComponents<DOTweenAnimation>(); // Get animations from assigned GameObject
        foreach (var anim in objectDoAnimations)
        {
            anim.DORestart(); // Restart all animations
        }
    }

    public void PlayAnimationBackward()
    {
        objectDoAnimations = this.gameObject.GetComponents<DOTweenAnimation>();

        float longestDuration = 0f;

        foreach (var anim in objectDoAnimations)
        {
            if (anim.tween != null && anim.tween.IsActive())
            {
                float animDuration = anim.tween.Duration(false);
                if (animDuration > longestDuration)
                {
                    longestDuration = animDuration;
                }

                anim.tween.PlayBackwards();
            }
        }

        if (longestDuration > 0f)
        {
            DOVirtual.DelayedCall(longestDuration, () => gameObject.SetActive(false));
        }
    }




}
