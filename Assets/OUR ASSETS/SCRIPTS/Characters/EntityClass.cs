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
    public int defence = 0;
    public float defenceDebuffPerc = 0;
    public float defenceBuffPerc = 0;
    public int resistance = 0;
    public float resistanceDebuffPerc = 0;
    public float resistanceBuffPerc = 0;


    public int health;
    public int maxHealth;
    public int shield;
    public int maxShield = 999;
    public int armor;
    public int maxArmor = 999;
    public int counterDamage = 0;

    float rotationSpeed = -2000f;  // Degrees per second
    public bool allowEntityRotation = false;

    public bool modifiedSummon = false;


    //combat
    public GameObject sliderBar;
    public Slider slider;
    public GameObject shieldIcon;
    public GameObject shieldText;
    public GameObject armorIcon;
    public GameObject armorText;
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

    //public bool spawnSummonTurnsObject = false;

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

        // StartCoroutine(InititializeEntity());


    }

    public IEnumerator SpawnUI()
    {

        AIBrain aIBrain = this.GetComponent<AIBrain>();
        GameObject gameObjectUI = this.gameObject.transform.Find("gameobjectUI").gameObject;

        //Destroy the gameobject UI
        if (gameObjectUI != null)
        {
            yield return StartCoroutine(SystemManager.Instance.DestroyObjectIE(gameObjectUI, 0));

        }

        //Add the appropriate entity UI
        if (aIBrain == null)
        {
            yield return StartCoroutine(SystemManager.Instance.SpawnPrefabIE(UI_Combat.Instance.commonGameobjectUI, this.gameObject, 0, "gameobjectUI", spawnGameObjectUI));

        }
        else
        {
            yield return StartCoroutine(SystemManager.Instance.SpawnPrefabIE(UI_Combat.Instance.commonGameobjectUI, this.gameObject, 0, "gameobjectUI", spawnGameObjectUI));

        }

        yield return null;
    }

    //private IEnumerator InitializeHealthBar(GameObject newUI)
    //{
    //    // Wait until the end of the frame to ensure the object is fully instantiated and set up
    //    yield return new WaitForEndOfFrame();

    //    // Call InitUIBar now that the object is fully set up
    //    InitUIBar();
    //}

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

    public void InititalizeStatsOnly()
    {
        health = scriptableEntity.currHealth;
        maxHealth = scriptableEntity.maxHealth;
        shield = scriptableEntity.shield;
        armor = scriptableEntity.armor;
        maxShield = 999;
        maxArmor = 999;
        attack = scriptableEntity.strength;
        defence = scriptableEntity.defence;
    }

    public IEnumerator InititializeEntity()
    {

        bool allowScaling = false;

        List<string> tagsAllowedForScaling = SystemManager.Instance.GetEnemyTagsList();

        if (tagsAllowedForScaling.Contains(this.gameObject.tag))
        {
            allowScaling = true;
        }


        yield return StartCoroutine(SpawnUI());

        //initialize from SO
        InititalizeStatsOnly();

        InitUIBar();
        //save current position , so it can go back if needed
        OriginXPos = this.gameObject.transform.position.x;
    }


    public void ModifyStatsFromCustomClass(EntityClass entityClassModify)
    {

        if (entityClassModify.health != this.health)
        {
            this.health = entityClassModify.health;
        }

        if (entityClassModify.maxHealth != this.maxHealth)
        {
            this.maxHealth = entityClassModify.maxHealth;
        }

        if (entityClassModify.shield != this.shield)
        {
            this.shield = entityClassModify.shield;
        }

        if (entityClassModify.maxShield != this.maxShield)
        {
            this.maxShield = entityClassModify.maxShield;
        }

        if (entityClassModify.armor != this.armor)
        {
            this.armor = entityClassModify.armor;
        }

        if (entityClassModify.maxArmor != this.maxArmor)
        {
            this.maxArmor = entityClassModify.maxArmor;
        }

        InitUIBar();

    }


    public void InitUIBar()
    {

        sliderBar = this.gameObject.transform.Find("gameobjectUI").Find("Bars").Find("Health").gameObject;
        slider = sliderBar.GetComponent<Slider>();
        shieldIcon = sliderBar.transform.Find("ShieldIcon").gameObject;
        shieldText = shieldIcon.transform.Find("TEXT").gameObject;
        armorIcon = sliderBar.transform.Find("ArmorIcon").gameObject;
        armorText = armorIcon.transform.Find("TEXT").gameObject;
        healthText = sliderBar.transform.Find("TEXT").gameObject;
        fillBar = sliderBar.transform.Find("Fill").gameObject;

        //make the healthbar red and hide shield
        //shieldIcon.gameObject.SetActive(false);
        //armorIcon.gameObject.SetActive(false);
        //fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

        //update based on enemy hp and max hp
        healthText.GetComponent<TMP_Text>().text = health + "/" + maxHealth;

        //adjust the hp bar
        slider.value = (float)health / (float)maxHealth;

        //armor = 10;
        //shield = 10;

        if (ItemManager.Instance.artifactTestPoolList.Count != 0 && this.gameObject.tag == "Enemy")
        {

            health = 1;
        }

        Combat.Instance.UpdateEntityDamageVisuals(this);

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

        // Check if the ray intersects with any colliders

        if (SystemManager.Instance.abilityMode != SystemManager.AbilityModes.NONE){
            return;
        }

        AIBrain aIBrain = this.GetComponent<AIBrain>();

        //if no ai brain the get continue to the next or deasd
        if (aIBrain == null)
        {
            return;
        }

        if (aIBrain.scriptableEntity.aICommands.Count == 0)
        {
            return;
        }

        if (this.gameObject.GetComponent<AIBrain>().targetForCard != null)
        {
            List<CombatPosition> combatPositionList = Combat.Instance.CheckCardTargets(this.gameObject.GetComponent<AIBrain>().targetForCard, this.gameObject.GetComponent<AIBrain>().scriptableCardToUse);

            foreach (CombatPosition combatPosition in combatPositionList)
            {
                combatPosition.position.transform.Find("TargetingPrefab").gameObject.SetActive(true);
            }
        }

        CardScriptData cardScriptData = new CardScriptData();
        cardScriptData.scriptableCard = this.gameObject.GetComponent<AIBrain>().aICommandCardToUse.aiScriptableCard;

        GameObject aiCardParent = GameObject.Find("ROOT").transform.Find("UICOMBAT").Find("CANVAS").Find("AI CARD").gameObject;

        //SystemManager.Instance.DestroyAllChildren(aiCardParent);

        aiCardParent.transform.localScale = new Vector3(1f, 1f, 1);

        cardScriptData.scalingLevelValue = aIBrain.getCardLevel;

        GameObject aiCard = DeckManager.Instance.InitializeCardPrefab(cardScriptData, aiCardParent, false,false);
        aiCard.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        aiCardParent.transform.position = new Vector3(this.gameObject.transform.position.x,
            aiCardParent.transform.position.y, 
            aiCardParent.transform.position.z);

        aiCard.GetComponent<CardScript>().enabled = false;
        aiCard.GetComponent<CardEvents>().enabled = false;

        // Get reference to the CanvasGroup
        aiCard.AddComponent<CanvasGroup>();
        CanvasGroup canvasGroup = aiCard.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 
        // Fade in (to visible)
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.25f); // fade to full alpha in 0.5 seconds

        ////create gameobject on scene and spawn it on the discard spawner
        //UI_Combat.Instance.CheckEnemyCard.SetActive(true);
        //GameObject cardPrefab = UI_Combat.Instance.CheckEnemyCard.transform.GetChild(0).gameObject;
        //cardPrefab.GetComponent<Canvas>().sortingOrder = 1000;

        //ScriptableCard scriptableCard = aIBrain.scriptableCardToUse;

        ////add the scriptable card object to the prefab class to reference
        //cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;

        ////make the local scale 1,1,1
        //cardPrefab.transform.localScale = new Vector3(1, 1, 1);

        ////update the information on the card prefab
        //DeckManager.Instance.UpdateCardUI(cardPrefab);

        ////deactivate events
        //cardPrefab.GetComponent<CardEvents>().enabled = false;

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

        if (aIBrain.scriptableEntity.aICommands.Count == 0)
        {
            return;
        }

        GameObject aiCardParent = GameObject.Find("ROOT").transform.Find("UICOMBAT").Find("CANVAS").Find("AI CARD").gameObject;

        //SystemManager.Instance.DestroyAllChildren(aiCardParent);

        GameObject aiCard = aiCardParent.transform.GetChild(0).gameObject;
        aiCard.transform.parent = GameObject.Find("ROOT").transform.Find("UICOMBAT").Find("CANVAS");
        Destroy(aiCard,1f);

        CanvasGroup canvasGroup = aiCard.GetComponent<CanvasGroup>();
        // Fade out (to invisible)
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.25f); // fade to 0 alpha in 0.5 seconds

        CombatCardHandler.Instance.ResetAllTargeting();

    }

    //public void AdjustSummonTurns(int value)
    //{
    //    if (spawnSummonTurnsObject)
    //    {
    //        //summonTurns += value;
    //        //summonTurnsText.text = summonTurns.ToString();

    //        //if (summonTurns >= 99)
    //        //{
    //        //    summonTurns = 99;
    //        //}

    //        //if (summonTurns <= 0)
    //        //{
    //        //    summonTurns = 0;
    //        //}

    //        //if the target reach to 0
    //        Combat.Instance.CheckIfEntityIsDead(this);

    //    }
    //}
}


