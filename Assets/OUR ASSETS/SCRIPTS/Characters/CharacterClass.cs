using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClass : MonoBehaviour
{
    public bool isUI = false;
    public ScriptablePlayer scriptablePlayer;

    public List<GameObject> listBuffDebuffClass;

    public int poisongDmg;

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
    public TMP_Text characterTitle;
    public Image uiPanelBorder;


    [Header("ID")]
    public string characterID;

    

    public CharacterClass()
    {

        //add an id to this scriptableCard, this is in order to identify it by comparisons
        characterID = System.Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        //if its ui element we dont wanna do anything
        if (isUI)
        {
            InitializeUICharacterPanel();
            return;
        }

        InititializeCharacter();



    }

    public void InitializeUICharacterPanel()
    {
        mainImage = this.gameObject.transform.Find("MainImage").Find("Image").GetComponent<Image>();
        characterTitle = this.gameObject.transform.Find("Title").Find("Text").GetComponent<TMP_Text>();
        uiPanelBorder = this.gameObject.transform.Find("uiPanelBorder").GetComponent<Image>();

        if (scriptablePlayer != null)
        {


            mainImage.sprite = scriptablePlayer.characterImage;
            characterTitle.text = scriptablePlayer.mainClass.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InititializeCharacter()
    {
        //variables that can change during battle
        poisongDmg = scriptablePlayer.poisonDmg;
        health = scriptablePlayer.currHealth;
        maxHealth = scriptablePlayer.maxHealth;


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
