using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityClass : MonoBehaviour
{


    [Header("CHARACTER SPECIFIC")]
    public bool isUI = false;
    public ScriptableEntity scriptableEntity;

    [Header("ENEMY SPECIFIC")]

    [Header("COMMON")]
    public List<GameObject> listBuffDebuffClass;
    public int poisongDmg;
    public int attack = 0;
    public float attackDebuffPerc = 0;
    public float attackBuffPerc = 0;
    public int health;
    public int maxHealth;
    public int shield;
    public int maxShield;

    //combat
    public GameObject sliderBar;
    public Slider slider;
    public GameObject shieldIcon;
    public GameObject shieldText;
    public GameObject healthText;
    public GameObject fillBar;

    //ui
    public Image mainImage;
    public TMP_Text entityTitle;
    public Image uiPanelBorder;

    public Transform originalCombatPos;
    public float OriginXPos;

    [Header("ID")]
    public string entityID;


    public EntityClass()
    {

        //add an id to this scriptableCard, this is in order to identify it by comparisons
        entityID = System.Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        //if its ui element we dont wanna do anything
        if (isUI)
        {
            InitializeUIEntityPanel();
            return;
        }

        InititializeEntity();



    }

    public void InitializeUIEntityPanel()
    {
        mainImage = this.gameObject.transform.Find("MainImage").Find("Image").GetComponent<Image>();
        entityTitle = this.gameObject.transform.Find("Title").Find("Text").GetComponent<TMP_Text>();
        uiPanelBorder = this.gameObject.transform.Find("uiPanelBorder").GetComponent<Image>();

        if (scriptableEntity != null)
        {


            mainImage.sprite = scriptableEntity.entityImage;
            entityTitle.text = scriptableEntity.mainClass.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InititializeEntity()
    {
        //variables that can change during battle
        poisongDmg = scriptableEntity.poisonDmg;
        health = scriptableEntity.currHealth;
        maxHealth = scriptableEntity.maxHealth;
        attack = scriptableEntity.strength;

        InitUI();

        //save current position , so it can go back if needed
        OriginXPos = this.gameObject.transform.position.x;
    }

    public void InitUI()
    {
        sliderBar = this.gameObject.transform.Find("gameobjectUI").Find("Bars").Find("Health").gameObject;
        slider = sliderBar.GetComponent<Slider>();
        shieldIcon = sliderBar.transform.Find("ShieldIcon").gameObject;
        shieldText = shieldIcon.transform.Find("TEXT").gameObject;
        healthText = sliderBar.transform.Find("TEXT").gameObject;
        fillBar = sliderBar.transform.Find("Fill").gameObject;

        //make the healthbar red and hide shield
        shieldIcon.gameObject.SetActive(false);
        fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

        //update based on enemy hp and max hp
        healthText.GetComponent<TMP_Text>().text = health + " / " + maxHealth;

        //adjust the hp bar
        slider.value = (float)health / (float)maxHealth;

        if (shield > 0)
        {

            //update text on shield
            shieldText.GetComponent<TMP_Text>().text = shield.ToString();

            //make the bar blue
            fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);

            //make shield icon visible
            shieldIcon.SetActive(true);

        }
    }
}
