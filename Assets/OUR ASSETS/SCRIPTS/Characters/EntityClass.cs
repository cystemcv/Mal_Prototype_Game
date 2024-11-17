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
    
    //stats
    public int poisongDmg;
    public int attack = 0;
    public float attackDebuffPerc = 0;
    public float attackBuffPerc = 0;
    public int health;
    public int maxHealth;
    public int shield;
    public int maxShield;
    public int counterDamage = 0;

    float rotationSpeed = -2000f;  // Degrees per second
    public bool allowEntityRotation = false;

    public int summonTurns = 1;

    //combat
    public GameObject sliderBar;
    public Slider slider;
    public GameObject shieldIcon;
    public GameObject shieldText;
    public GameObject healthText;
    public GameObject fillBar;
    public TMP_Text summonTurnsText;

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
    public Vector3 spawnGameObjectUI = new Vector3(0, 0, 0);
    private GameObject gameobjectUI;
    private GameObject model;

    public bool spawnSummonTurnsObject = false;

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

        StartCoroutine(InititializeEntity());


    }

    public IEnumerator SpawnUI()
    {






        yield return null;
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


    public IEnumerator InititializeEntity()
    {

        if (this.gameObject.tag == "PlayerSummon" || this.gameObject.tag == "EnemySummon")
        {
            spawnSummonTurnsObject = true;
        }

        yield return SpawnUI();

        //variables that can change during battle
        poisongDmg = scriptableEntity.poisonDmg;
        health = scriptableEntity.currHealth;
        maxHealth = scriptableEntity.maxHealth;
        attack = scriptableEntity.strength;

        summonTurns = scriptableEntity.summonTurns;

        if (spawnSummonTurnsObject)
        {
            summonTurnsText = this.gameObject.transform.Find("gameobjectUI").Find("Bars").Find("SummonTurnsObject").Find("Text").GetComponent<TMP_Text>();
            summonTurnsText.text = summonTurns.ToString();
        }
        else
        {
            this.gameObject.transform.Find("gameobjectUI").Find("Bars").Find("SummonTurnsObject").gameObject.SetActive(false);
        }

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

        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity, this.gameObject);
        SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorVeryLightBlue, this.gameObject);

        AIBrain aIBrain = this.GetComponent<AIBrain>();

        //if no ai brain the get continue to the next or deasd
        if (aIBrain == null)
        {
            return;
        }

        //create gameobject on scene and spawn it on the discard spawner
        UI_Combat.Instance.CheckEnemyCard.SetActive(true);
        GameObject cardPrefab = UI_Combat.Instance.CheckEnemyCard.transform.GetChild(0).gameObject;
        cardPrefab.GetComponent<Canvas>().sortingOrder = 1000;

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

    public void OnPointerExit(PointerEventData eventData)
    {
        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, this.gameObject);
        SystemManager.Instance.ChangeTargetEntityColor(SystemManager.Instance.colorWhite, this.gameObject);

        AIBrain aIBrain = this.GetComponent<AIBrain>();

        //if no ai brain the get continue to the next or deasd
        if (aIBrain == null)
        {
            return;
        }

        UI_Combat.Instance.CheckEnemyCard.SetActive(false);

    }

    public void AdjustSummonTurns(int value)
    {
        if (spawnSummonTurnsObject)
        {
            summonTurns += value;
            summonTurnsText.text = summonTurns.ToString();

            if (summonTurns >= 99)
            {
                summonTurns = 99;
            }

            if (summonTurns <= 0)
            {
                summonTurns = 0;
            }

            //if the target reach to 0
            Combat.Instance.CheckIfEntityIsDead(this);

        }
    }
}
