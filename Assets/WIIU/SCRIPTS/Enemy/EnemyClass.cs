using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyClass : MonoBehaviour
{

    public List<GameObject> listBuffDebuffClass;

    public int health;
    public int maxHealth;

    public int shield;
    public int maxShield;

    public GameObject sliderBar;
    public Slider slider;
    public GameObject shieldIcon;
    public GameObject shieldText;
    public GameObject healthText;
    public GameObject fillBar;

    public void Start()
    {
        sliderBar = this.gameObject.transform.Find("gameobjectUI").Find("Bars").Find("Health").gameObject;
        slider = sliderBar.GetComponent<Slider>();
        shieldIcon = sliderBar.transform.Find("ShieldIcon").gameObject;
        shieldText = shieldIcon.transform.Find("TEXT").gameObject;
        healthText = sliderBar.transform.Find("TEXT").gameObject;
        fillBar = sliderBar.transform.Find("Fill").gameObject;

        //make the healthbar red and hide shield
        shieldIcon.gameObject.SetActive(false);
        fillBar.GetComponent<Image>().color = CombatManager.Instance.redColor;

        //update based on enemy hp and max hp
        healthText.GetComponent<TMP_Text>().text = health + " / " + maxHealth;

        //adjust the hp bar
        slider.value = (float)health / (float)maxHealth;

        if (shield > 0)
        {

            //update text on shield
            shieldText.GetComponent<TMP_Text>().text = shield.ToString();

            //make the bar blue
            fillBar.GetComponent<Image>().color = CombatManager.Instance.blueColor;

            //make shield icon visible
            shieldIcon.SetActive(true);

        }
    }

}
