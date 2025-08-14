using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Michsky.MUIP;
using static UIManager;
using static CardListManager;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;



    public ScriptablePlanets scriptablePlanet;
    public ScriptablePlanets scriptableFakeEventPlanet;
    public ScriptableEvent ScriptableEvent;

    public GameObject planetClicked;

    public bool trainingMode = false;
    public bool trainingModeUIOpen = false;
    public GameObject trainingOptionsUI;

    public GameObject manaSliderUI;
    public GameObject trainingModeText;
    public GameObject turnSliderUI;

    public GameObject enemyDropDown1;
    public GameObject enemyDropDown2;
    public GameObject enemyDropDown3;
    public GameObject enemyDropDown4;
    public GameObject enemyDropDown5;

    public GameObject heroDropDown1;
    public GameObject heroDropDown2;
    public GameObject heroDropDown3;
    public GameObject heroDropDown4;
    public GameObject heroDropDown5;

    public GameObject battlegroundsDropDown;
    public GameObject heroDropDown;

    public Sprite fillBox;
    public Sprite emptyBox;

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


    public void Start()
    {



        // Add int32 (dynamic) events
        var cd1 = enemyDropDown1.GetComponent<CustomDropdown>();
        var cd2 = enemyDropDown2.GetComponent<CustomDropdown>();
        var cd3 = enemyDropDown3.GetComponent<CustomDropdown>();
        var cd4 = enemyDropDown4.GetComponent<CustomDropdown>();
        var cd5 = enemyDropDown5.GetComponent<CustomDropdown>();

        cd1.onValueChanged.AddListener(value => Training_SpawnEnemy(value, 0));
        cd2.onValueChanged.AddListener(value => Training_SpawnEnemy(value, 1));
        cd3.onValueChanged.AddListener(value => Training_SpawnEnemy(value, 2));
        cd4.onValueChanged.AddListener(value => Training_SpawnEnemy(value, 3));
        cd5.onValueChanged.AddListener(value => Training_SpawnEnemy(value, 4));

        var cd1h = heroDropDown1.GetComponent<CustomDropdown>();
        var cd2h = heroDropDown2.GetComponent<CustomDropdown>();
        var cd3h = heroDropDown3.GetComponent<CustomDropdown>();
        var cd4h = heroDropDown4.GetComponent<CustomDropdown>();
        var cd5h = heroDropDown5.GetComponent<CustomDropdown>();

        cd1h.onValueChanged.AddListener(value => Training_SpawnAlly(value, 0));
        cd2h.onValueChanged.AddListener(value => Training_SpawnAlly(value, 1));
        cd3h.onValueChanged.AddListener(value => Training_SpawnAlly(value, 2));
        cd4h.onValueChanged.AddListener(value => Training_SpawnAlly(value, 3));
        cd5h.onValueChanged.AddListener(value => Training_SpawnAlly(value, 4));

        var cdBattleground = battlegroundsDropDown.GetComponent<CustomDropdown>();
        cdBattleground.onValueChanged.AddListener(value => Training_ChangeBattleGround(value, 0));

        var cdHero = heroDropDown.GetComponent<CustomDropdown>();
        cdHero.onValueChanged.AddListener(value => Training_ChangeHero(value, 0));

        InitializeOptions();
    }

    public void UiTrainingButton()
    {
        if (CombatManager.Instance.trainingMode)
        {
            UIManager.Instance.topPanelCombat.transform.Find("ShowTrainingButton").gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.topPanelCombat.transform.Find("ShowTrainingButton").gameObject.SetActive(false);
        }
    }

    public void SpawnEffectPrefab(GameObject target, ScriptableCard scriptableCard)
    {
        //missing prefab vfx
        if (scriptableCard.abilityEffect == null || target == null)
        {
            return;
        }

        //spawn prefab
        GameObject abilityEffect = Instantiate(scriptableCard.abilityEffect, target.transform.Find("model").Find("SpawnEffect").position, Quaternion.identity);
        //abilityEffect.transform.SetParent(target.transform.Find("model").Find("SpawnEffect"));

        abilityEffect.transform.position = new Vector3(abilityEffect.transform.position.x,
            abilityEffect.transform.position.y + scriptableCard.abilityEffectYaxis,
            abilityEffect.transform.position.z);

        //if the target is player then we wanna rotate 
        if (target.tag == "Player")
        {
            abilityEffect.transform.Rotate(new Vector3(0, 180, 0));
        }

        Destroy(abilityEffect, scriptableCard.abilityEffectLifetime);
    }

    public int GetScaleNumber(List<int> scalingList)
    {

        int num = 0;

        int galaxyLevel = CustomDungeonGenerator.Instance.scalingLevel;

        // Ensure galaxyLevel doesn't exceed the list's count to avoid exceptions
        num = scalingList.Take(galaxyLevel).Sum();

        return num;

    }


    //TRAINING MODE
    public void EnableTrainingMode()
    {
        trainingMode = true;

        //show training menu
    }

    public void DisableTrainingMode()
    {
        trainingMode = false;
    }

    public void ShowTrainingOptions()
    {
        InitializeOptions();
        trainingModeUIOpen = true;
        trainingOptionsUI.SetActive(true);
    }

    public void HideTrainingOptions()
    {
        trainingModeUIOpen = false;
        trainingOptionsUI.SetActive(false);
    }

    public void InitializeOptions()
    {

        if (Combat.Instance == null)
        {
            return;
        }

        manaSliderUI.GetComponent<Slider>().value = Combat.Instance.ManaAvailable;
        turnSliderUI.GetComponent<Slider>().value = Combat.Instance.turns;

        if (trainingMode)
        {
            trainingModeText.GetComponent<TMP_Text>().text = "Training : On";
        }
        else
        {
            trainingModeText.GetComponent<TMP_Text>().text = "Training : Off";
        }

        CustomDropdown enemyCustomDropDown1 = enemyDropDown1.GetComponent<CustomDropdown>();
        CustomDropdown enemyCustomDropDown2 = enemyDropDown2.GetComponent<CustomDropdown>();
        CustomDropdown enemyCustomDropDown3 = enemyDropDown3.GetComponent<CustomDropdown>();
        CustomDropdown enemyCustomDropDown4 = enemyDropDown4.GetComponent<CustomDropdown>();
        CustomDropdown enemyCustomDropDown5 = enemyDropDown5.GetComponent<CustomDropdown>();

        CustomDropdown heroCustomDropDown1 = heroDropDown1.GetComponent<CustomDropdown>();
        CustomDropdown heroCustomDropDown2 = heroDropDown2.GetComponent<CustomDropdown>();
        CustomDropdown heroCustomDropDown3 = heroDropDown3.GetComponent<CustomDropdown>();
        CustomDropdown heroCustomDropDown4 = heroDropDown4.GetComponent<CustomDropdown>();
        CustomDropdown heroCustomDropDown5 = heroDropDown5.GetComponent<CustomDropdown>();

        CustomDropdown battlegroundsCustomDropDown = battlegroundsDropDown.GetComponent<CustomDropdown>();
        CustomDropdown heroCustomDropDown = heroDropDown.GetComponent<CustomDropdown>();


        enemyCustomDropDown1.ClearList();
        enemyCustomDropDown2.ClearList();
        enemyCustomDropDown3.ClearList();
        enemyCustomDropDown4.ClearList();
        enemyCustomDropDown5.ClearList();

        heroCustomDropDown1.ClearList();
        heroCustomDropDown2.ClearList();
        heroCustomDropDown3.ClearList();
        heroCustomDropDown4.ClearList();
        heroCustomDropDown5.ClearList();

        battlegroundsCustomDropDown.ClearList();
        heroCustomDropDown.ClearList();

        enemyCustomDropDown1.CreateNewItem("EMPTY", null, false);
        enemyCustomDropDown2.CreateNewItem("EMPTY", null, false);
        enemyCustomDropDown3.CreateNewItem("EMPTY", null, false);
        enemyCustomDropDown4.CreateNewItem("EMPTY", null, false);
        enemyCustomDropDown5.CreateNewItem("EMPTY", null, false);

        heroCustomDropDown1.CreateNewItem("EMPTY", null, false);
        heroCustomDropDown2.CreateNewItem("EMPTY", null, false);
        heroCustomDropDown3.CreateNewItem("EMPTY", null, false);
        heroCustomDropDown4.CreateNewItem("EMPTY", null, false);
        heroCustomDropDown5.CreateNewItem("EMPTY", null, false);

        foreach (ScriptableEntity scriptableEntity in AIManager.Instance.scriptableEntities)
        {
            enemyCustomDropDown1.CreateNewItem(scriptableEntity.entityName, null, false);
            enemyCustomDropDown2.CreateNewItem(scriptableEntity.entityName, null, false);
            enemyCustomDropDown3.CreateNewItem(scriptableEntity.entityName, null, false);
            enemyCustomDropDown4.CreateNewItem(scriptableEntity.entityName, null, false);
            enemyCustomDropDown5.CreateNewItem(scriptableEntity.entityName, null, false);

            heroCustomDropDown1.CreateNewItem(scriptableEntity.entityName, null, false);
            heroCustomDropDown2.CreateNewItem(scriptableEntity.entityName, null, false);
            heroCustomDropDown3.CreateNewItem(scriptableEntity.entityName, null, false);
            heroCustomDropDown4.CreateNewItem(scriptableEntity.entityName, null, false);
            heroCustomDropDown5.CreateNewItem(scriptableEntity.entityName, null, false);
        }

        foreach (ScriptableBattleGrounds scriptableBattleGround in CustomDungeonGenerator.Instance.scriptableBattleGrounds)
        {
            battlegroundsCustomDropDown.CreateNewItem(scriptableBattleGround.battleGroundType.ToString(), null, false);
        }

        foreach (ScriptableEntity scriptableEntityHero in CharacterManager.Instance.characterList)
        {
            heroCustomDropDown.CreateNewItem(scriptableEntityHero.mainClass.ToString(), null, false);
        }


        // Initialize the new items
        enemyCustomDropDown1.SetupDropdown();
        enemyCustomDropDown2.SetupDropdown();
        enemyCustomDropDown3.SetupDropdown();
        enemyCustomDropDown4.SetupDropdown();
        enemyCustomDropDown5.SetupDropdown();

        heroCustomDropDown1.SetupDropdown();
        heroCustomDropDown2.SetupDropdown();
        heroCustomDropDown3.SetupDropdown();
        heroCustomDropDown4.SetupDropdown();
        heroCustomDropDown5.SetupDropdown();

        battlegroundsCustomDropDown.SetupDropdown();
        heroCustomDropDown.SetupDropdown();

        if (CombatManager.Instance.scriptablePlanet != null)
        {
            SystemManager.Instance.SetDropdownByName(battlegroundsCustomDropDown, CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGroundType.ToString());
        }

        if (StaticData.staticCharacter != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown, StaticData.staticCharacter.mainClass.ToString());
        }

        //set drop down values
        if (Combat.Instance.enemiesCombatPositions[0].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(enemyCustomDropDown1, Combat.Instance.enemiesCombatPositions[0].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.enemiesCombatPositions[1].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(enemyCustomDropDown2, Combat.Instance.enemiesCombatPositions[1].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.enemiesCombatPositions[2].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(enemyCustomDropDown3, Combat.Instance.enemiesCombatPositions[2].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.enemiesCombatPositions[3].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(enemyCustomDropDown4, Combat.Instance.enemiesCombatPositions[3].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.enemiesCombatPositions[4].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(enemyCustomDropDown5, Combat.Instance.enemiesCombatPositions[4].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }

        //set drop down values
        if (Combat.Instance.characterCombatPositions[0].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown1, Combat.Instance.characterCombatPositions[0].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.characterCombatPositions[1].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown2, Combat.Instance.characterCombatPositions[1].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.characterCombatPositions[2].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown3, Combat.Instance.characterCombatPositions[2].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.characterCombatPositions[3].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown4, Combat.Instance.characterCombatPositions[3].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }
        if (Combat.Instance.characterCombatPositions[4].entityOccupiedPos != null)
        {
            SystemManager.Instance.SetDropdownByName(heroCustomDropDown5, Combat.Instance.characterCombatPositions[4].entityOccupiedPos.GetComponent<EntityClass>().scriptableEntity.entityName);
        }


    }

    public void Training_SetMana()
    {
        Combat.Instance.ManaAvailable = int.Parse(manaSliderUI.GetComponent<Slider>().value.ToString());
    }

    public void Training_SetTurns()
    {
        Combat.Instance.Turns = int.Parse(turnSliderUI.GetComponent<Slider>().value.ToString());
        UIManager.Instance.AnimateTextTypeWriter("Turn:" + Combat.Instance.Turns, UIManager.Instance.turnText.GetComponent<TMP_Text>(), 4f);
    }

    public void Training_OpenArtifactPanel()
    {
        ItemManager.Instance.OpenArtifactPanel(true);
    }

    public void Training_DestroyAllEnemies()
    {
        GameObject combatscene = GameObject.Find("COMBATSCENE");
        SystemManager.Instance.DestroyAllChildren(combatscene.transform.Find("Enemies").gameObject);

        SystemManager.Instance.SetDropdownByName(enemyDropDown1.GetComponent<CustomDropdown>(), "EMPTY");
        SystemManager.Instance.SetDropdownByName(enemyDropDown2.GetComponent<CustomDropdown>(), "EMPTY");
        SystemManager.Instance.SetDropdownByName(enemyDropDown3.GetComponent<CustomDropdown>(), "EMPTY");
        SystemManager.Instance.SetDropdownByName(enemyDropDown4.GetComponent<CustomDropdown>(), "EMPTY");
        SystemManager.Instance.SetDropdownByName(enemyDropDown5.GetComponent<CustomDropdown>(), "EMPTY");

    }

    public void Training_ResetCombat()
    {
        SystemManager.Instance.DestroyAllChildren(GameObject.Find("HAND MANAGER").transform.Find("Canvas").Find("HAND").gameObject);
        HandManager.Instance.cardsInHandList.Clear();
        StartCoroutine(Combat.Instance.InitializeCombat());
    }

    public void Training_ToggleTraining()
    {
        trainingMode = !trainingMode;

        if (trainingMode)
        {
            trainingModeText.GetComponent<TMP_Text>().text = "Training : On";
        }
        else
        {
            trainingModeText.GetComponent<TMP_Text>().text = "Training : Off";
        }
    }

    public void Training_DrawCard()
    {
        DeckManager.Instance.DrawCardFromDeck();
    }

    public void Training_SetHPOne()
    {
        List<GameObject> gameObjects = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetAllTagsList());

        foreach (GameObject gameObject in gameObjects)
        {
            EntityClass entityClass = gameObject.GetComponent<EntityClass>();

            if (entityClass == null)
            {
                continue;
            }

            entityClass.health = 1;
            entityClass.InitUIBar();
        }

    }

    public void Training_SetHPFull()
    {
        List<GameObject> gameObjects = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetAllTagsList());

        foreach (GameObject gameObject in gameObjects)
        {
            EntityClass entityClass = gameObject.GetComponent<EntityClass>();

            if (entityClass == null)
            {
                continue;
            }

            entityClass.health = entityClass.maxHealth;
            entityClass.InitUIBar();
        }

    }


    public void Training_RemoveAllCardsFromDeck()
    {
        StaticData.staticMainDeck.Clear();
        StartCoroutine(Combat.Instance.InitializeCombat());
    }

    public void Training_RemoveCardsFromDeck()
    {
        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = StaticData.staticMainDeck;
        optionsSettings.cardListMode = CardListMode.EDIT;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = StaticData.staticMainDeck.Count;
        optionsSettings.title = "Remove Cards From Deck";
        optionsSettings.onConfirmAction = CombatManager.Instance.CardList_RemoveCardsFromDeck;
        optionsSettings.allowClassButtons = false;
        UIManager.Instance.ShowCardList(optionsSettings);
    }

    public void CardList_RemoveCardsFromDeck()
    {

        Debug.Log("CardList_RemoveCardsFromDeck called");

        if (UIManager.Instance.selectedCardList.Count == 0)
        {
            return;
        }

        //otherwise remove card

        foreach (CardScriptData cardScriptData in UIManager.Instance.selectedCardList)
        {
            DeckManager.Instance.RemoveCardFromList(cardScriptData, StaticData.staticMainDeck);
        }

        StartCoroutine(Combat.Instance.InitializeCombat());

    }

    public void Training_AddCardsToDeck()
    {
        List<CardScriptData> cardScriptDataList = new List<CardScriptData>();
        cardScriptDataList = CardListManager.Instance.GetAllCardsFromLibrary();

        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = cardScriptDataList;
        optionsSettings.cardListMode = CardListMode.EDIT;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 100;
        optionsSettings.title = "Add Cards To Deck";
        optionsSettings.onConfirmAction = CombatManager.Instance.CardList_AddCardsToDeck;
        optionsSettings.allowClassButtons = true;
        optionsSettings.allowDuplicates = true;
        UIManager.Instance.ShowCardList(optionsSettings);
    }

    public void CardList_AddCardsToDeck()
    {

        Debug.Log("CardList_AddCardsToDeck called");

        if (UIManager.Instance.selectedCardList.Count == 0)
        {
            return;
        }

        //otherwise remove card

        foreach (CardScriptData cardScriptData in UIManager.Instance.selectedCardList)
        {
            for (int i = 0; i < cardScriptData.copiesOfCard; i++)
            {
                DeckManager.Instance.AddCardOnDeck(cardScriptData.scriptableCard, 0);
            }

        }

        StartCoroutine(Combat.Instance.InitializeCombat());

    }

    public void Training_SpawnEnemy(int value, int combatPosIndex)
    {
        StartCoroutine(Training_SpawnEnemyIE(value, combatPosIndex));
    }

    public IEnumerator Training_SpawnEnemyIE(int value, int combatPosIndex)
    {
        CombatPosition combatPosition = Combat.Instance.enemiesCombatPositions[combatPosIndex];
        //THEN IT IS EMPTY
        if (value == 0)
        {
            if (combatPosition.entityOccupiedPos != null)
            {
                Destroy(combatPosition.entityOccupiedPos);
                yield return null;
            }
        }
        else
        {
            //reduce by 1 to remove the empty
            value = value - 1;
            ScriptableEntity scriptableEntity = AIManager.Instance.scriptableEntities[value];


            if (combatPosition.entityOccupiedPos != null)
            {
                Destroy(combatPosition.entityOccupiedPos);
                yield return null;
            }

            CardScriptData cardScriptData = new CardScriptData();
            yield return StartCoroutine(Combat.Instance.InstantiateEntity(null, scriptableEntity, null, "Enemy", null, cardScriptData, combatPosition));
        }


    }

    public void Training_SpawnAlly(int value, int combatPosIndex)
    {
        StartCoroutine(Training_SpawnAllyIE(value, combatPosIndex));
    }

    public IEnumerator Training_SpawnAllyIE(int value, int combatPosIndex)
    {
        CombatPosition combatPosition = Combat.Instance.characterCombatPositions[combatPosIndex];

        //cannot remove player
        if (combatPosition.entityOccupiedPos != null && combatPosition.entityOccupiedPos.tag == "Player")
        {
            if (combatPosIndex == 0)
            {
                SystemManager.Instance.SetDropdownByName(heroDropDown1.GetComponent<CustomDropdown>(), "EMPTY");
            }
            else if (combatPosIndex == 1)
            {
                SystemManager.Instance.SetDropdownByName(heroDropDown2.GetComponent<CustomDropdown>(), "EMPTY");
            }
            else if (combatPosIndex == 2)
            {
                SystemManager.Instance.SetDropdownByName(heroDropDown3.GetComponent<CustomDropdown>(), "EMPTY");
            }
            else if (combatPosIndex == 3)
            {
                SystemManager.Instance.SetDropdownByName(heroDropDown4.GetComponent<CustomDropdown>(), "EMPTY");
            }
            else if (combatPosIndex == 4)
            {
                SystemManager.Instance.SetDropdownByName(heroDropDown5.GetComponent<CustomDropdown>(), "EMPTY");
            }
            NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.WARNING, "WARNING!", "Cannot remove hero! Please change the hero position first!");
            yield return null;
        }
        else
        {
            //THEN IT IS EMPTY
            if (value == 0)
            {
                if (combatPosition.entityOccupiedPos != null)
                {
                    Destroy(combatPosition.entityOccupiedPos);
                    yield return null;
                }
            }
            else
            {
                //reduce by 1 to remove the empty
                value = value - 1;
                ScriptableEntity scriptableEntity = AIManager.Instance.scriptableEntities[value];


                if (combatPosition.entityOccupiedPos != null)
                {
                    Destroy(combatPosition.entityOccupiedPos);
                    yield return null;
                }

                CardScriptData cardScriptData = new CardScriptData();
                yield return StartCoroutine(Combat.Instance.InstantiateEntity(null, scriptableEntity, null, "PlayerSummon", null, cardScriptData, combatPosition));
            }

        }


    }

    public void Training_ChangeBattleGround(int value, int combatPosIndex)
    {
        StartCoroutine(Training_ChangeBattleGroundIE(value, combatPosIndex));
    }

    public IEnumerator Training_ChangeBattleGroundIE(int value, int combatPosIndex)
    {
        GameObject battleground = GameObject.FindGameObjectWithTag("BattleGround");
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(battleground.transform.gameObject));

        ScriptableBattleGrounds scriptableBattleGround = CustomDungeonGenerator.Instance.scriptableBattleGrounds[value];

        GameObject bg = Instantiate(scriptableBattleGround.battleGround, battleground.transform.position, Quaternion.identity);
        bg.transform.SetParent(battleground.transform);

        Combat.Instance.battleGroundType = CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGroundType;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (scriptableBattleGround.isSpaceShip)
        {
            player.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.spaceShipAnimator;
        }
        else
        {
            player.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.entityAnimator;
        }


        yield return null; // Wait for a frame 

    }

    public void Training_ChangeHero(int value, int combatPosIndex)
    {
        StartCoroutine(Training_ChangeHeroIE(value, combatPosIndex));
    }

    public IEnumerator Training_ChangeHeroIE(int value, int combatPosIndex)
    {
        GameObject hero = GameObject.FindGameObjectWithTag("Player");
        CombatPosition combatPosition = Combat.Instance.GetCombatPosition(hero);
        Destroy(hero);

        ScriptableEntity scriptableEntity = CharacterManager.Instance.characterList[value];
        CardScriptData cardScriptData = new CardScriptData();
        EntityResult entityResult = new EntityResult();
        yield return StartCoroutine(Combat.Instance.InstantiateEntity(entityResult, scriptableEntity, null, "Player", null, cardScriptData, combatPosition));

        StaticData.staticCharacter = scriptableEntity;

        hero = entityResult.entity;

        int valueBg = battlegroundsDropDown.GetComponent<CustomDropdown>().selectedItemIndex;
        ScriptableBattleGrounds scriptableBattleGround = CustomDungeonGenerator.Instance.scriptableBattleGrounds[valueBg];


        if (scriptableBattleGround.isSpaceShip)
        {
            hero.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.spaceShipAnimator;
        }
        else
        {
            hero.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.entityAnimator;
        }


        yield return null; // Wait for a frame 

    }




}
