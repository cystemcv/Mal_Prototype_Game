using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntityClass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    float rotationSpeed = -2000f;  // Degrees per second
    public bool allowEntityRotation = false;

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

    public Vector3 originalCombatPos;
    public float OriginXPos;

    [Header("ID")]
    public string entityID;

    public SystemManager.EntityMode entityMode = SystemManager.EntityMode.NORMAL;

    //spawn prefabs on entity
    [Header("PREFABS")]
    public bool spawnBuffDebuffObject;
    public Vector3 spawnBuffDebuffObjectAdjustment = new Vector3(0,0,0);
    public bool spawnHealthBarObject;
    public Vector3 spawnHealthBarObjectAdjustment = new Vector3(0, 0, 0);
    public bool spawnIntendObject;
    public Vector3 spawnIntendObjectAdjustment = new Vector3(0, 0, 0);

    [Header("PREFABS LIVE ADJUSTMENTS")]
    public bool allowPrefabLiveAdjustments = true;
    private GameObject bdO;
    private GameObject hpO;
    private GameObject intentO;
    private GameObject gameobjectUI;

    private GameObject model;

    public EntityClass()
    {

        //add an id to this scriptableCard, this is in order to identify it by comparisons
        entityID = System.Guid.NewGuid().ToString();
    }

    void Update()
    {

        if (allowEntityRotation)
        {
            RotateEntity();
        }

        if (allowPrefabLiveAdjustments == false)
        {
            return;
        }

        if (hpO != null)
        {
            hpO.transform.position = new Vector3(gameobjectUI.transform.position.x + spawnHealthBarObjectAdjustment.x, gameobjectUI.transform.position.y + spawnHealthBarObjectAdjustment.y, gameobjectUI.transform.position.z + spawnHealthBarObjectAdjustment.z);
        }

        if (bdO != null)
        {
            bdO.transform.position = new Vector3(hpO.transform.position.x + spawnBuffDebuffObjectAdjustment.x, hpO.transform.position.y + spawnBuffDebuffObjectAdjustment.y, hpO.transform.position.z + spawnBuffDebuffObjectAdjustment.z);
        }

        if (intentO != null)
        {
            intentO.transform.position = new Vector3(gameobjectUI.transform.position.x + spawnIntendObjectAdjustment.x, gameobjectUI.transform.position.y + spawnIntendObjectAdjustment.y, gameobjectUI.transform.position.z + spawnIntendObjectAdjustment.z);
        }

    


    }


    private void Awake()
    {


    }

    // Start is called before the first frame update
    void Start()
    {

        //SpawnUI();
        model = this.gameObject.transform.Find("model").gameObject;

        //if its ui element we dont wanna do anything
        if (isUI)
        {
            InitializeUIEntityPanel();
            return;
        }

        InititializeEntity();


    }

    public void SpawnUI()
    {

     

        //get the entity ui spawner
        gameobjectUI = this.gameObject.transform.Find("gameobjectUI").gameObject;

        if (spawnHealthBarObject)
        {
            //if already exist then keep it
            if (gameobjectUI.transform.Find("Bars") != null)
            {

            }
            else
            {
                //spawn new ui
                Vector3 spawnPos = new Vector3(gameobjectUI.transform.position.x + spawnHealthBarObjectAdjustment.x, gameobjectUI.transform.position.y + spawnHealthBarObjectAdjustment.y, gameobjectUI.transform.position.z + spawnHealthBarObjectAdjustment.z);
                GameObject newUI = Instantiate(SystemManager.Instance.entity_healthBarObject, spawnPos, Quaternion.identity);
                newUI.name = "Bars";
                hpO = newUI;
                //parent it
                hpO.transform.SetParent(gameobjectUI.transform);


            }


        }

        //spawn ui for entities
        if (spawnBuffDebuffObject)
        {
            //if already exist then destroy it
            if (gameobjectUI.transform.Find("BuffDebuffList") != null)
            {

            }
            else
            {
                //spawn new ui
                Vector3 spawnPos = new Vector3(hpO.transform.position.x + spawnBuffDebuffObjectAdjustment.x, hpO.transform.position.y + spawnBuffDebuffObjectAdjustment.y - 0.1f, hpO.transform.position.z + spawnBuffDebuffObjectAdjustment.z);
                GameObject newUI = Instantiate(SystemManager.Instance.entity_buffDebuffObject, spawnPos, Quaternion.identity);
                newUI.name = "BuffDebuffList";
                bdO = newUI;
                //parent it
                bdO.transform.SetParent(gameobjectUI.transform);
            }
        }



        if (spawnIntendObject)
        {
            //if already exist then destroy it
            if (gameobjectUI.transform.Find("intendList") != null)
            {

            }
            else
            {
                //spawn new ui
                Vector3 spawnPos = new Vector3(gameobjectUI.transform.position.x + spawnIntendObjectAdjustment.x, gameobjectUI.transform.position.y + spawnIntendObjectAdjustment.y, gameobjectUI.transform.position.z + spawnIntendObjectAdjustment.y);
                GameObject newUI = Instantiate(SystemManager.Instance.entity_intendObject, spawnPos, Quaternion.identity);
                newUI.name = "intendList";
                intentO = newUI;
                //parent it
                intentO.transform.SetParent(gameobjectUI.transform);
            }
        }
    }

    private IEnumerator InitializeHealthBar(GameObject newUI)
    {
        // Wait until the end of the frame to ensure the object is fully instantiated and set up
        yield return new WaitForEndOfFrame();

        // Call InitUIBar now that the object is fully set up
        InitUIBar();
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


    public void InititializeEntity()
    {
        SpawnUI();

        //variables that can change during battle
        poisongDmg = scriptableEntity.poisonDmg;
        health = scriptableEntity.currHealth;
        maxHealth = scriptableEntity.maxHealth;
        attack = scriptableEntity.strength;

        InitUIBar();

        //save current position , so it can go back if needed
        OriginXPos = this.gameObject.transform.position.x;
    }

    public void InitUIBar()
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

    public void RotateEntity()
    {
        model.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void DestroyEntityInCombat()
    {
        //launch enemy and then kaboom
        allowEntityRotation = true;

        // Get the Rigidbody2D component of the enemy
        Rigidbody2D enemyRb = this.gameObject.GetComponent<Rigidbody2D>();

        // Launch the enemy up and to the right
        Vector2 launchDirection = new Vector2(Combat.Instance.sideForce, Combat.Instance.upwardForce);
        enemyRb.AddForce(launchDirection);

        StartCoroutine(DestroyEntityInCombatIE());
    }

    public IEnumerator DestroyEntityInCombatIE()
    {

        yield return new WaitForSeconds(Combat.Instance.deathLaunchTimer);

        GameObject deathExplostion = Instantiate(Combat.Instance.deathExplosion, this.gameObject.transform.Find("model").Find("SpawnEffect").position, Quaternion.identity);
        Destroy(deathExplostion, 0.6f);
        Destroy(this.gameObject);
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("IN???");
        if (this.gameObject.tag == "Enemy")
        {

            Debug.Log("IN???");

            //create gameobject on scene and spawn it on the discard spawner
            UI_Combat.Instance.CheckEnemyCard.SetActive(true);
            GameObject cardPrefab = UI_Combat.Instance.CheckEnemyCard.transform.GetChild(0).gameObject;
            cardPrefab.GetComponent<Canvas>().sortingOrder = 1000;
            AIBrain aIBrain = this.GetComponent<AIBrain>();

    
            ScriptableCard scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep];

            //add the scriptable card object to the prefab class to reference
            cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;


            //make the local scale 1,1,1
            cardPrefab.transform.localScale = new Vector3(1, 1, 1);

            //update the information on the card prefab
            DeckManager.Instance.UpdateCardUI(cardPrefab);

            //deactivate events
            cardPrefab.GetComponent<CardEvents>().enabled = false;


        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (this.gameObject.tag == "Enemy")
        {
            UI_Combat.Instance.CheckEnemyCard.SetActive(false);
        }

    }
}
