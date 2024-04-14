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

	public Animator animator;

	TMP_Text txt;
	Image icon;


	void Awake()
    {
		txt = GetComponentInChildren<TMP_Text>();
		icon = this.transform.Find("Icon").GetComponent<Image>();
		animator = this.GetComponent<Animator>();
	}

	void Start()
	{
		//icon.sprite = nonSelectedIcon;
		animator.SetBool("selectedButton", false);
	}

	void Update()
	{

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//icon.sprite = selectedIcon;
		animator.SetBool("selectedButton", true);
		AudioManager.Instance.PlaySfx("UI_Error");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		//icon.sprite = nonSelectedIcon;
		animator.SetBool("selectedButton", false);
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






}