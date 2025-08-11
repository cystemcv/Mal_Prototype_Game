using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScriptableCard;
using static ScriptableScaling;

public class PlayedCard
{

    public bool isPlaying = false;
    public float timer;
    public GameObject target;
    public ScriptableCard scriptableCard;
    public CardScript cardScript;
    public CardScriptData cardScriptData;
    public GameObject cardObject;
    public GameObject playedCardUI;
}

[Serializable]
public class CombatPosition
{
    public GameObject position;
    public GameObject entityOccupiedPos;
    public ScriptableHazard hazard;
}

public class Combat : MonoBehaviour
{
    public static Combat Instance;


    [Header("SPAWNS")]
    public GameObject characterStartSpawn;
    public GameObject enemyStartSpawn;

    //public List<CombatPosition> companionCombatPositions;
    public List<CombatPosition> characterCombatPositions;
    public List<CombatPosition> enemiesCombatPositions;



    [Header("COMBAT COMMON VARIABLES")]
    public int turns = 0;
    public float deathLaunchSpeed = 2f;
    public float deathLaunchTimer = 0.4f;
    public float upwardForce = 800f;  // Force applied upwards
    public float sideForce = 400f;    // Force applied to the right
    public GameObject deathExplosion;

    [Header("LEADER MECHANIC")]
    public GameObject leaderCharacter;

    [Header("CONDITIONS")]
    public int charactersAlive = 0;
    public int enemiesAlive = 0;
    public bool conditionsEnabled = false;
    public bool combatEnded = false;


    public int tempBoostAttack = 0;
    public int tempBoostDefence = 0;
    public int tempBoostResistance = 0;
    public int tempMonsterAttackBoost = 0;

    public LTDescr moveEntityTween;

    GameObject battleground;

    public int reduceCompanionStartingCd = 0;
    public int reduceCompanionAbilityCd = 0;

    [Header("GLOBAL VARIABLES")]
    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;

    public delegate void OnManaChange();
    public event OnManaChange onManaChange;

    public List<PlayedCard> playedCardList = new List<PlayedCard>();
    public List<CardScriptData> discardEffectCardList = new List<CardScriptData>();

    public SystemManager.BattleGroundType battleGroundType;

    private string codeMode = "";
    public int ManaAvailable
    {
        get { return manaAvailable; }
        set
        {
            if (manaAvailable == value) return;
            manaAvailable = value;
            if (onManaChange != null)
                onManaChange();
        }
    }



    //onCombatDeckChange += CombatDeckEvent;
    //    onDiscardedPileChange += DiscardedPileEvent;
    //    onBanishedPileChange += BanishedPileEvent;


    public int Turns
    {
        get => turns;
        set
        {
            if (turns != value)
            {
                turns = value;
                TurnsChanged?.Invoke(turns); // Trigger the event
            }
        }
    }

    public event System.Action<int> TurnsChanged;

    private void OnEnable()
    {
        onManaChange += ManaDetectEvent;

    }

    private void OnDisable()
    {
        onManaChange -= ManaDetectEvent;

    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //at the start of combat

        if (CombatManager.Instance.ScriptableEvent == null)
        {
            StartCombat();
        }
        else
        {
            StartEvent();
        }


    }

    // Start is called before the first frame update
    void Start()
    {

        //CombatManager.Instance.EnableTrainingMode();

        //StaticData.companionItemList.Add(ItemManager.Instance.SOItemToClass(StaticData.staticScriptableCompanion.companionItemList[0]));

    }


    public void Update()
    {

        if (conditionsEnabled == true &&
            (CheckIfSpawnPosAreEmpty("Player") || CheckIfSpawnPosAreEmpty("Enemy")))
        {

            if (!CombatManager.Instance.trainingMode)
            {
                CombatOver();
            }


        }

        //check the queue
        if (playedCardList.Count > 0)
        {

            //check if the card is played
            //play the card
            if (playedCardList[0].isPlaying == false)
            {

                //save the cardscript
                DeckManager.Instance.savedPlayedCard = playedCardList[0].cardScript;

                DeckManager.Instance.PlayerPlayedCard(playedCardList[0].cardScriptData);

                playedCardList[0].isPlaying = true;




            }

            //if the card is playing then start decreasing the timer
            if (playedCardList[0].timer > 0 && playedCardList[0].isPlaying == true)
            {
                playedCardList[0].timer -= Time.deltaTime;
                playedCardList[0].timer = Mathf.Max(playedCardList[0].timer, 0);
                playedCardList[0].playedCardUI.transform.Find("CardTimer").GetComponent<TMP_Text>().text = playedCardList[0].timer.ToString("F1");
            }
            else if (playedCardList[0].timer <= 0 && playedCardList[0].isPlaying == true)
            {
                //remove it
                UI_Combat.Instance.RemovePlayedCardUI(playedCardList[0]);

                playedCardList.RemoveAt(0);




            }


        }

    }

    public void FixedUpdate()
    {

        //UIManager.Instance.turnText.GetComponent<TMP_Text>().text = "Turn:" + Turns;
    }

    //public void CardQueueNumbering()
    //{
    //    int counter = 0;
    //    foreach (PlayedCard playedCard in playedCardList)
    //    {
    //        if (playedCard.cardObject == null)
    //        {
    //            continue;
    //        }
    //        playedCard.cardObject.GetComponent<CardEvents>().enabled = false;
    //        playedCard.cardObject.GetComponent<CardScript>().cardQueue.SetActive(true);
    //        playedCard.cardObject.GetComponent<CardScript>().cardQueue.transform.Find("Text").GetComponent<TMP_Text>().text = counter.ToString();
    //        counter++;
    //    }

    //}

    public void ManaDetectEvent()
    {

        //show the mana on UI
        //UI_Combat.Instance.manaInfo.transform.Find("Text").GetComponent<TMP_Text>().text = manaAvailable.ToString();
        UI_Combat.Instance.manaInfo.GetComponent<ButtonManager>().SetText("MANA [" + manaAvailable.ToString() + "]");

        //go throught each card in the hand and update them
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            UpdateCardAfterManaChange(cardPrefab);
        }

    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().cardScriptData.scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        CardScriptData cardScriptData = cardScript.cardScriptData;
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("Info").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (manaAvailable < cardScriptData.primaryManaCost)
        {
            //cannot be played
            cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }
        else
        {
            //can be played
            cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
        }
    }

    public void CombatOver()
    {
        if (combatEnded)
        {
            return;
        }

        //make mana back to full
        StartCoroutine(RefillMana());

        //yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnCombatEnd));
        combatEnded = true;

        UI_Combat.Instance.DisableCombatUI();

        //always check characters first, if both lost = game over
        if (CheckIfSpawnPosAreEmpty("Player"))
        {

            //remove the dungeon
            StaticData.staticDungeonParent = null;

            //lose
            UIManager.Instance.resultsWindow.SetActive(true);
        }
        else if (CheckIfSpawnPosAreEmpty("Enemy"))
        {

            //get the player
            GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
            EntityClass entityClass = playerCharacter.GetComponent<EntityClass>();

            //save character
            StaticData.staticCharacter.maxHealth = entityClass.maxHealth;
            StaticData.staticCharacter.currHealth = entityClass.health;

            StaticData.lootItemList.Clear();

            CalculateLootRewards();

            ItemManager.Instance.ShowLoot();

            //win
            //UIManager.Instance.resultsWindow.SetActive(true);

            //UIManager.Instance.resultsWindow_Text.GetComponent<TMP_Text>().text = StaticData.FormatStatsForText(StaticData.combatStats);
            //UIManager.Instance.resultsWindow_ScoringText.GetComponent<TMP_Text>().text = "Total Score : " + StaticData.GetTotalJsonScore(StaticData.combatStats);
            //ItemManager.Instance.ShowInventory();

        }

    }

    public void CalculateLootRewards()
    {

        foreach (ScriptablePlanets.ItemClassPlanet itemClassPlanet in CombatManager.Instance.scriptablePlanet.itemClassPlanet)
        {

            //based on min and max value
            int quantity = UnityEngine.Random.Range(itemClassPlanet.minQuantity, itemClassPlanet.maxQuantity);

            int percQuantity = 0;

            if (itemClassPlanet.percentage != 0)
            {


                //then give value based on range
                for (int i = 0; i < quantity; i++)
                {

                    int randomChance = UnityEngine.Random.Range(0, 100);

                    if (randomChance <= itemClassPlanet.percentage)
                    {
                        percQuantity++; //add quantity by 1
                    }

                }

                quantity = percQuantity;

            }



            //if 0 then nothing should be added on loot
            if (quantity == 0)
            {
                continue;
            }

            //create the classItem
            ClassItemData classItem = new ClassItemData(itemClassPlanet.scriptableItem, quantity);

            //then add item to loot
            StaticData.lootItemList.Add(classItem);

            //ItemManager.Instance.AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);
            // yield return new WaitForSeconds(0.1f);
        }


        foreach (ScriptablePlanets.ItemClassPlanet itemClassPlanet in CombatManager.Instance.scriptablePlanet.planetBattleGround.itemClassPlanet)
        {

            //based on min and max value
            int quantity = UnityEngine.Random.Range(itemClassPlanet.minQuantity, itemClassPlanet.maxQuantity);

            int percQuantity = 0;

            if (itemClassPlanet.percentage != 0)
            {


                //then give value based on range
                for (int i = 0; i < quantity; i++)
                {

                    int randomChance = UnityEngine.Random.Range(0, 100);

                    if (randomChance <= itemClassPlanet.percentage)
                    {
                        percQuantity++; //add quantity by 1
                    }

                }

                quantity = percQuantity;

            }



            //if 0 then nothing should be added on loot
            if (quantity == 0)
            {
                continue;
            }

            //create the classItem
            ClassItemData classItem = new ClassItemData(itemClassPlanet.scriptableItem, quantity);

            //then add item to loot
            StaticData.lootItemList.Add(classItem);

            //ItemManager.Instance.AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);
            // yield return new WaitForSeconds(0.1f);
        }

        //yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnLoot));

    }

    public IEnumerator InitializeCombat(string code = "")
    {
        codeMode = code;
        UI_Combat.Instance.DisableCombatUI();

        //hide the dungeon generato
        if (StaticData.staticDungeonParent != null)
        {
            StaticData.staticDungeonParent.SetActive(false);
        }
        else
        {
            StaticData.staticDungeonParent = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("DungeonParent").gameObject;
            StaticData.staticDungeonParent.SetActive(false);
        }

        GameObject spaceship = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("spaceship").gameObject;
        GameObject spaceshipLine = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("spaceshipLine").gameObject;
        if (spaceship)
        {
            spaceship.SetActive(false);
            spaceshipLine.SetActive(false);
        }

        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

        //Debug.Log("Test dsck"  + DeckManager.Instance.scriptableDeck.mainDeck[0].scriptableCard.cardName);

        //reset turns
        Turns = 0;

        //play music
        AudioManager.Instance.PlayMusic("Combat_1");



        //destroy any previous characters
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Characters").gameObject));

        //destroy any previous enemies
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Enemies").gameObject));

        //destroy battleground
        battleground = GameObject.FindGameObjectWithTag("BattleGround");
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(battleground.transform.gameObject));

        yield return StartCoroutine(HandManager.Instance.DestroyAllHandCards());

        //spawn the companion
        //yield return StartCoroutine(SpawnCompanion());

        //spawn the characters
        yield return StartCoroutine(SpawnCharacters());

        //spawn the enemies
        yield return StartCoroutine(SpawnEnemies(false));


        //spawn battleground
        yield return StartCoroutine(SpawnBattleground());



        yield return new WaitForSeconds(0.5f);

        //initialize all things card related, deck, combat deck, discard pile, etc
        yield return StartCoroutine(InitializeCardsAndDecks());


        //check the characters that are alive
        yield return StartCoroutine(CheckCharactersAlive());

        //check the enemies that are alive
        //yield return StartCoroutine(CheckEnemiesAlive());



        if (code != "EVENT")
        {
            yield return StartCoroutine(SystemManager.Instance.TriggerLoadEndAnimation());
        }


        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(UI_Combat.Instance.OnNotification("COMBAT START", 1));

        yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnCombatStart, null));

        StaticData.staticScriptableCompanion.InitializeButton();

        //do the ai logic for each enemy
        yield return StartCoroutine(AIManager.Instance.AiActInitialize(SystemManager.Instance.GetEnemyTagsList()));

        //start win conditions
        conditionsEnabled = true;

        //start the player
        yield return StartCoroutine(WaitPlayerTurns());
    }


    public IEnumerator WaitEnemyTurns()
    {

        //UI_Combat.Instance.endTurnButton.GetComponent<Animator>().SetTrigger("PlayerEnd");
        UI_Combat.Instance.DisableCombatUI();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return StartCoroutine(PlayerTurnEnd());

        if (combatEnded)
        {
            yield return null; //stops function
        }

   

        yield return StartCoroutine(EnemyTurnStart());

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return StartCoroutine(EnemyTurn());

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return StartCoroutine(EnemyTurnEnd());

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return StartCoroutine(PlayerTurnStart());

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return StartCoroutine(PlayerTurn());

        if (combatEnded)
        {
            yield return null; //stops function
        }
    }



    public IEnumerator PlayerTurnStart()
    {



        if (combatEnded)
        {
            yield return null; //stops function
        }


        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;
        //start player turn
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

        //increase turn;
        Turns += 1;
        //animate the text
        UIManager.Instance.AnimateTextTypeWriter("Turn:" + Turns, UIManager.Instance.turnText.GetComponent<TMP_Text>(), 4f);


        yield return StartCoroutine(ActivateHazardsPlayerTurnStart());

        //mana should go back to full
        yield return StartCoroutine(RefillMana());

        //remove mana
        yield return StartCoroutine(RemoveShieldFromEntitiesBasedOnTag("Player"));


        //do the ai logic for each enemy
        yield return StartCoroutine(AIManager.Instance.AiAct(SystemManager.Instance.GetPlayerTagsList()));

        //////generate intend for all players
        yield return StartCoroutine(AIManager.Instance.GenerateIntends(SystemManager.Instance.GetPlayerTagsList()));
        yield return StartCoroutine(AIManager.Instance.GenerateIntends(SystemManager.Instance.GetEnemyTagsList()));

        //yield return StartCoroutine(LowerSummonTurns(SystemManager.Instance.GetPlayerTagsList()));

        //loop for all buffs and debuffs
        yield return StartCoroutine(BuffSystemManager.Instance.ActivateAllBuffsDebuffs());

        //draw cards
        yield return StartCoroutine(DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit, 0));

        yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerTurnStart, null));

        yield return StartCoroutine(ActivateDelayedCardEffects());


        yield return null; // Wait for a frame 
    }

    public IEnumerator ActivateDelayedCardEffects()
    {

        foreach (CardScriptData cardScriptData in discardEffectCardList)
        {
            cardScriptData.scriptableCard.OnDelayEffect(cardScriptData);

        }

        //empty the delay effect list
        discardEffectCardList.Clear();

        yield return null; // Wait for a frame 
    }

    //public IEnumerator LowerSummonTurns(List<string> tags)
    //{
    //    List<GameObject> entities = SystemManager.Instance.FindGameObjectsWithTags(tags);

    //    foreach (GameObject entity in entities)
    //    {
    //        if (entity.tag == "PlayerSummon" || entity.tag == "EnemySummon")
    //        {
    //            EntityClass entityClass = entity.GetComponent<EntityClass>();
    //            //lower every turn by 1
    //            entityClass.AdjustSummonTurns(-1);
    //        }

    //    }

    //    yield return null;
    //}

    public int GetLastNonNull(List<GameObject> array)
    {
        for (int i = array.Count - 1; i >= 0; i--)
        {
            if (array[i] != null)
            {
                return i;
            }
        }
        return 0; // If no non-null elements are found
    }

    public IEnumerator PlayerTurn()
    {

        //if (turns != 1)
        //{
        //UI_Combat.Instance.endTurnButton.GetComponent<Animator>().SetTrigger("EnemyEnd");


        //}

        yield return new WaitForSeconds(0.4f);
        UI_Combat.Instance.EnableCombatUI();
        yield return StartCoroutine(UI_Combat.Instance.OnNotification("PLAYER TURN", 1));

        if (combatEnded)
        {
            yield return null; //stops function
        }

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerTurn;


        yield return null; //wait for frame

    }

    public IEnumerator PlayerTurnEnd()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerEndTurn;

        //discard cards
        DeckManager.Instance.DiscardWholeHand();

        //loop for all buffs and debuffs
        yield return StartCoroutine(BuffSystemManager.Instance.ActivateAllBuffsDebuffs());

        yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerTurnEnd, null));

        yield return StartCoroutine(ActivateHazardsPlayerTurnEnd());

        yield return new WaitForSeconds(1f);

        yield return null; //skip frame

    }

    public IEnumerator ActivateHazardsPlayerTurnEnd()
    {

        foreach (CombatPosition combatPosition in characterCombatPositions)
        {
            if (combatPosition.hazard != null)
            {
                StartCoroutine(combatPosition.hazard.OnTurnEnd(combatPosition));
            }

            yield return null; //skip frame
        }

    }

    public IEnumerator ActivateHazardsEnemyTurnEnd()
    {

        foreach (CombatPosition combatPosition in enemiesCombatPositions)
        {
            if (combatPosition.hazard != null)
            {
                StartCoroutine(combatPosition.hazard.OnTurnEnd(combatPosition));
            }

            yield return null; //skip frame
        }

    }

    public IEnumerator ActivateHazardsPlayerTurnStart()
    {

        foreach (CombatPosition combatPosition in characterCombatPositions)
        {
            if (combatPosition.hazard != null)
            {
                StartCoroutine(combatPosition.hazard.OnTurnStart(combatPosition));
            }

            yield return null; //skip frame
        }

    }

    public IEnumerator ActivateHazardsEnemyTurnStart()
    {

        foreach (CombatPosition combatPosition in enemiesCombatPositions)
        {
            if (combatPosition.hazard != null)
            {
                StartCoroutine(combatPosition.hazard.OnTurnStart(combatPosition));
            }

            yield return null; //skip frame
        }

    }

    public IEnumerator EnemyTurnStart()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }
        //TutorialManager.Instance.StartTutorial(TutorialManager.Instance.tutorialDataList[0]);
        TutorialManager.Instance.StartTutorial(TutorialManager.Instance.tutorialDataList[0]);
        yield return new WaitUntil(() => !TutorialManager.Instance.IsTutorialActive);

        TutorialManager.Instance.StartTutorial(TutorialManager.Instance.tutorialDataList[1]);
        yield return new WaitUntil(() => !TutorialManager.Instance.IsTutorialActive);


        yield return StartCoroutine(ActivateHazardsEnemyTurnStart());
        //TutorialManager.Instance.StartTutorial(TutorialManager.Instance.tutorialDataList[0]);

        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
        yield return StartCoroutine(UI_Combat.Instance.OnNotification("ENEMY TURN", 1));


        //loop for all buffs and debuffs
        yield return StartCoroutine(BuffSystemManager.Instance.ActivateAllBuffsDebuffs());

        //remove shield from all enemies
        RemoveShieldFromEntitiesBasedOnTag("Enemy");

        // yield return StartCoroutine(LowerSummonTurns(SystemManager.Instance.GetEnemyTagsList()));

        yield return null;
    }

    public IEnumerator EnemyTurn()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyTurn;

        //do the ai logic for each enemy
        yield return StartCoroutine(AIManager.Instance.AiAct(SystemManager.Instance.GetEnemyTagsList()));



        yield return null;
    }

    public IEnumerator EnemyTurnEnd()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyEndTurn;

        //loop for all buffs and debuffs
        yield return StartCoroutine(BuffSystemManager.Instance.ActivateAllBuffsDebuffs());

        yield return StartCoroutine(ActivateHazardsEnemyTurnEnd());

        yield return null;
    }

    public IEnumerator SpawnCompanion()
    {

        if (StaticData.staticScriptableCompanion == null)
        {
            StaticData.staticScriptableCompanion = CharacterManager.Instance.companionList[0].Clone();
        }
        CardScriptData cardScriptData = new CardScriptData();
        StartCoroutine(InstantiateEntity(null, StaticData.staticScriptableCompanion, "Companion", null, cardScriptData, null));

        yield return null; // Wait for a frame 


    }


    public IEnumerator SpawnCharacters()
    {

        //check character
        if (StaticData.staticCharacter == null)
        {
            //assign a temporary character (this is mostly to test battle)
            StaticData.staticCharacter = CharacterManager.Instance.characterList[0].Clone();
        }

        if (StaticData.staticScriptableCompanion == null)
        {
            StaticData.staticScriptableCompanion = CharacterManager.Instance.companionList[0].Clone();
        }

        CardScriptData cardScriptData = new CardScriptData();
        StartCoroutine(InstantiateEntity(StaticData.staticCharacter, null, "Player", null, cardScriptData, null));

        yield return null; // Wait for a frame 


    }



    public IEnumerator InstantiateEntity(ScriptableEntity scriptableEntity, ScriptableCompanion scriptableCompanion, string tag, EntityClass modifiedEntityClass, CardScriptData cardScriptData, CombatPosition combatPositionChosen)
    {

        //check if available spots
        if (CheckIfSpawnPosAreFull(tag))
        {
            Debug.Log("FULL LIST! " + tag);
            yield break;
        }

        CombatPosition combatPosition;

        if (combatPositionChosen != null)
        {
            combatPosition = combatPositionChosen;
        }
        else
        {


            //get the postition in combat
            if (CombatCardHandler.Instance.posClicked == null || CombatCardHandler.Instance.posClicked.position == null)
            {
                combatPosition = GetSpawnPosition(tag);
            }
            else
            {
                combatPosition = CombatCardHandler.Instance.posClicked;
                CombatCardHandler.Instance.posClicked = null;
            }
        }

        GameObject entity;
        //instantiate the character prefab based on the spawnPosition
        if (scriptableEntity != null)
        {
            entity = Instantiate(scriptableEntity.entityPrefab, combatPosition.position.transform.position, Quaternion.identity);
            //pass it the scriptable object to the class
            entity.GetComponent<EntityClass>().scriptableEntity = scriptableEntity;
        }
        else
        {
            entity = Instantiate(scriptableCompanion.companionPrefab, combatPosition.position.transform.position, Quaternion.identity);
        }

        entity.tag = tag;

        combatPosition.entityOccupiedPos = entity;



        if (SystemManager.Instance.GetPlayerTagsList().Contains(tag))
        {
            entity.transform.SetParent(this.gameObject.transform.Find("Characters"));
            //entity.AddComponent<EntityExtras>();

            if (entity.GetComponent<AIBrain>() != null)
            {

                FlipSprite(entity);
            }
            else
            {
                //entity.transform.Find("gameobjectUI").Find("DisplayCardName").gameObject.SetActive(false);
            }
        }
        else if (SystemManager.Instance.GetEnemyTagsList().Contains(tag))
        {
            entity.transform.SetParent(this.gameObject.transform.Find("Enemies"));
        }
        else
        {

        }


        if (entity.GetComponent<AIBrain>() != null)
        {

                if (cardScriptData.scalingLevelValue != 0)
                {
                    entity.GetComponent<AIBrain>().entityLevel = cardScriptData.scalingLevelValue;
                }

                entity.GetComponent<AIBrain>().justSpawned = true;
                entity.GetComponent<AIBrain>().GenerateIntend();
        
        }

        if (entity.GetComponent<EntityClass>() != null)
        {
            yield return StartCoroutine(entity.GetComponent<EntityClass>().InititializeEntity());
        }

        //add scaling buff/debuffs
        for (int i = 0; i < CustomDungeonGenerator.Instance.scalingLevel; i++)
        {

            if (i >= CustomDungeonGenerator.Instance.scriptableScaling.scalingLevelBuffDebuffs.Count)
            {
                break;
            }

            ScalingLevelBuffDebuff scalingLevelBuffDebuffs = CustomDungeonGenerator.Instance.scriptableScaling.scalingLevelBuffDebuffs[i];

            if (scalingLevelBuffDebuffs.targetEnemySide && (entity.tag == "Enemy" || entity.tag == "EnemySummon"))
            {
                BuffSystemManager.Instance.AddBuffDebuff(entity, scalingLevelBuffDebuffs.scriptableBuffDebuff, scalingLevelBuffDebuffs.scalingValue);
            }
            else if (!scalingLevelBuffDebuffs.targetEnemySide && (entity.tag == "Player" || entity.tag == "PlayerSummon"))
            {
                BuffSystemManager.Instance.AddBuffDebuff(entity, scalingLevelBuffDebuffs.scriptableBuffDebuff, scalingLevelBuffDebuffs.scalingValue);
            }


        }


        yield return null;

    }

    public ScriptablePlanets GetScriptablePlanet(bool isFakeEventPlanet)
    {

        ScriptablePlanets scriptablePlanet;

        if (CombatManager.Instance.scriptableFakeEventPlanet != null && isFakeEventPlanet == true)
        {
            scriptablePlanet = CombatManager.Instance.scriptableFakeEventPlanet;
        }
        else
        {
            scriptablePlanet = CombatManager.Instance.scriptablePlanet;
        }


        return scriptablePlanet;

    }

    public IEnumerator SpawnEnemies(bool isFakeEventPlanet)
    {

        ScriptablePlanets scriptablePlanet = GetScriptablePlanet(isFakeEventPlanet);

        List<ScriptableEntity> scriptableEntities = scriptablePlanet.scriptableEntities;

        //if its not fake planet
        if (!isFakeEventPlanet)
        {
            //check if starting battle
            if (CustomDungeonGenerator.Instance.basicFightsFought < CustomDungeonGenerator.Instance.basicFightsFoughtMax && codeMode != "EVENT")
            {
                scriptableEntities = scriptablePlanet.scriptableBasicEntities;
                CustomDungeonGenerator.Instance.basicFightsFought++;
            }

        }

        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in scriptableEntities)
        {
            CardScriptData cardScriptData = new CardScriptData();
            //instantiate our character or characters
            yield return StartCoroutine(InstantiateEntity(scriptableEntity, null, "Enemy", null, cardScriptData, null));

            yield return null; // Wait for a frame 
        }

    }

    public IEnumerator SpawnBattleground()
    {

        GameObject bg = Instantiate(CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGround, battleground.transform.position, Quaternion.identity);
        bg.transform.SetParent(battleground.transform);

        battleGroundType = CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGroundType;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (CombatManager.Instance.scriptablePlanet.planetBattleGround.isSpaceShip)
        {
            player.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.spaceShipAnimator;
        }
        else
        {
            player.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = StaticData.staticCharacter.entityAnimator;
        }


        yield return null; // Wait for a frame 

    }

    public List<GameObject> FindPosTargeting(List<string> tags)
    {
        // If no tags are provided, return an empty list
        if (tags == null || tags.Count == 0)
        {
            return new List<GameObject>();
        }



        List<GameObject> listOfGameobjects = new List<GameObject>();
        List<CombatPosition> availablePositions = new List<CombatPosition>();

        foreach (string tag in tags)
        {

            if (tag != "PlayerPos" && tag != "EnemyPos")
            {
                //continue to the next tag
                continue;
            }

            //if tag is then we need to enable them

            if (Combat.Instance.CheckIfSpawnPosAreFull(tag))
            {
                //continue to the next tag
                continue;
            }

            if (tag == "PlayerPos")
            {
                availablePositions = characterCombatPositions
                   .Where(pos => pos.entityOccupiedPos == null)
                   .ToList();


            }
            else if (tag == "EnemyPos")
            {
                availablePositions = enemiesCombatPositions
                 .Where(pos => pos.entityOccupiedPos == null)
                 .ToList();


            }

            foreach (CombatPosition combatPosition in availablePositions)
            {
                combatPosition.position.transform.Find("Visual").gameObject.SetActive(true);
                listOfGameobjects.Add(combatPosition.entityOccupiedPos);
            }

        }



        return listOfGameobjects;
    }

    public void HideAllCombatPosVisuals()
    {
        foreach (CombatPosition combatPosition in characterCombatPositions)
        {
            combatPosition.position.transform.Find("Visual").gameObject.SetActive(false);
        }

        foreach (CombatPosition combatPosition in enemiesCombatPositions)
        {
            combatPosition.position.transform.Find("Visual").gameObject.SetActive(false);
        }


    }



    public CombatPosition FindCombatPositionByEntity(GameObject clickedEntity)
    {

        // Search character positions
        CombatPosition match = characterCombatPositions
            .FirstOrDefault(pos => pos.entityOccupiedPos == clickedEntity);
        if (match != null) return match;

        // Search enemy positions
        match = enemiesCombatPositions
            .FirstOrDefault(pos => pos.entityOccupiedPos == clickedEntity);
        if (match != null) return match;

        // If nothing is found
        return null;
    }

    public CombatPosition GetCombatPosition(GameObject target)
    {
        CombatPosition combatPosition;

        if (target.name == "Visual")
        {
            combatPosition = Combat.Instance.FindCombatPositionByGameObject(target.transform.parent.gameObject);
        }
        else
        {
            combatPosition = Combat.Instance.FindCombatPositionByEntity(target.transform.gameObject);
        }

        return combatPosition;
    }

    public CombatPosition FindCombatPositionByGameObject(GameObject clickedPosition)
    {

        // Search character positions
        CombatPosition match = characterCombatPositions
            .FirstOrDefault(pos => pos.position == clickedPosition);
        if (match != null) return match;

        // Search enemy positions
        match = enemiesCombatPositions
            .FirstOrDefault(pos => pos.position == clickedPosition);
        if (match != null) return match;

        // If nothing is found
        return null;
    }

    public CombatPosition GetSpawnPosition(string tag)
    {
        CombatPosition combatPosition = new CombatPosition();

        //last place is reserved for comp
        if (SystemManager.Instance.GetPlayerTagsList().Contains(tag) || tag == "PlayerPos")
        {

            List<CombatPosition> availablePositions = characterCombatPositions
            .Where(pos => pos.entityOccupiedPos == null)
            .ToList();

            int random = UnityEngine.Random.Range(0, availablePositions.Count);
            combatPosition = availablePositions[random];
        }
        else if (SystemManager.Instance.GetEnemyTagsList().Contains(tag) || tag == "EnemyPos")
        {
            List<CombatPosition> availablePositions = enemiesCombatPositions
            .Where(pos => pos.entityOccupiedPos == null)
            .ToList();

            int random = UnityEngine.Random.Range(0, availablePositions.Count);
            combatPosition = availablePositions[random];
        }
        else
        {
            //companion is on last place and he should always spawn first
            combatPosition = characterCombatPositions[characterCombatPositions.Count - 1];
        }

        return combatPosition;
    }

    public bool CheckIfSpawnPosAreFull(string tag)
    {
        bool isFull = false;

        if (SystemManager.Instance.GetPlayerTagsList().Contains(tag) || tag == "PlayerPos")
        {
            isFull = characterCombatPositions.All(pos => pos.entityOccupiedPos != null);
        }
        else if (SystemManager.Instance.GetEnemyTagsList().Contains(tag) || tag == "EnemyPos")
        {
            isFull = enemiesCombatPositions.All(pos => pos.entityOccupiedPos != null);
        }


        return isFull;
    }


    public bool CheckIfSpawnPosAreEmpty(string tag)
    {
        bool isFull = false;

        if (SystemManager.Instance.GetPlayerTagsList().Contains(tag) || tag == "PlayerPos")
        {
            isFull = characterCombatPositions.All(pos => pos.entityOccupiedPos == null);
        }
        else if (SystemManager.Instance.GetEnemyTagsList().Contains(tag) || tag == "EnemyPos")
        {
            isFull = enemiesCombatPositions.All(pos => pos.entityOccupiedPos == null);
        }


        return isFull;
    }


    public int GetStepsBetweenPositions(CombatPosition from, CombatPosition to)
    {
        int startIndex = characterCombatPositions.IndexOf(from);
        int endIndex = characterCombatPositions.IndexOf(to);

        if (startIndex == -1 || endIndex == -1)
        {
            Debug.LogWarning("One or both CombatPosition instances not found in the list.");
            return -1;
        }

        return Mathf.Abs(endIndex - startIndex);
    }

    public void FlipSprite(GameObject spriteObject)
    {
        Vector3 scale = spriteObject.transform.Find("model").localScale;
        scale.x *= -1; // Reverse the y-axis scale
        spriteObject.transform.Find("model").localScale = scale;
    }

    public IEnumerator RearrangeFormation(List<GameObject> formation)
    {

        bool nullPosFound = false;

        for (int i = 0; i < formation.Count; i++)
        {
            float distance = 0;
            Vector3 newPosition = new Vector3(0, 0, 0);

            //then get the distance
            if (i != 0)
            {
                distance = GetDistanceOfFormation(formation, i);
            }


            if (SystemManager.Instance.GetPlayerTagsList().Contains(formation[i].tag))
            {
                newPosition = new Vector3(characterStartSpawn.transform.position.x + distance, characterStartSpawn.transform.position.y, characterStartSpawn.transform.position.z);
            }
            else
            {
                newPosition = new Vector3(enemyStartSpawn.transform.position.x - distance, enemyStartSpawn.transform.position.y, enemyStartSpawn.transform.position.z);
            }

            LeanTween.moveX(formation[i], newPosition.x, 0.2f);

            yield return new WaitForSeconds(0.2f);

        }

        yield return null; // Wait for a frame 

    }



    public int GetFormationIndex(List<GameObject> formation)
    {

        int index = 0;

        for (int i = 0; i < formation.Count; i++)
        {

            //check if there is a gameobject to calculate distance. if there is none break the loop
            if (formation[i] == null)
            {
                index = i;
                break;
            }


        }

        return index;

    }

    public float GetDistanceOfFormation(List<GameObject> formation, int currentIndex)
    {

        float distance = 0f;

        for (int i = 0; i < formation.Count; i++)
        {

            //check if there is a gameobject to calculate distance. if there is none break the loop
            if (i == currentIndex)
            {
                break;
            }

            ScriptableEntity scriptableEntity = formation[i].GetComponent<EntityClass>().scriptableEntity;
            distance += scriptableEntity.distanceFromAnotherUnit;

        }

        return distance;

    }

    public void StartCombat(string code = "")
    {
        StartCoroutine(InitializeCombat(code));

    }

    public void StartEvent()
    {
        StartCoroutine(InitializeEvent());
    }

    public IEnumerator CheckCharactersAlive()
    {

        //initialize dead count
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

        charactersAlive = 0;
        foreach (GameObject character in characters)
        {
            //count how many alive
            if (character.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {
                charactersAlive += 1;
            }
        }

        yield return null;

    }

    //public IEnumerator CheckEnemiesAlive()
    //{
    //    //initialize dead count
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    //    enemiesAlive = 0;
    //    foreach (GameObject enemy in enemies)
    //    {
    //        //count how many alive
    //        if (enemy.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
    //        {
    //            enemiesAlive += 1;
    //        }
    //    }

    //    yield return null;
    //}

    public IEnumerator InitializeCardsAndDecks()
    {

        if (StaticData.staticMainDeck.Count == 0)
        {
            DeckManager.Instance.BuildStartingDeck();
        }

        //clean up discard pile
        DeckManager.Instance.discardedPile.Clear();

        //clean up banished pile
        DeckManager.Instance.banishedPile.Clear();

        //clean up hand
        DeckManager.Instance.handCards.Clear();

        //initialize the combat deck
        DeckManager.Instance.combatDeck.Clear();
        DeckManager.Instance.combatDeck = new List<CardScriptData>(StaticData.staticMainDeck);
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);



        yield return null; // Wait for a frame 
    }



    public IEnumerator RefillMana()
    {
        //initialize mana and UI
        ManaAvailable = manaMaxAvailable;

        yield return null;

    }



    public IEnumerator WaitPlayerTurns()
    {
        //yield return new WaitForSeconds(2f);
        //player turn start
        yield return StartCoroutine(PlayerTurnStart());
        //yield return new WaitForSeconds(2f);
        //player turn
        yield return StartCoroutine(PlayerTurn());

    }



    public IEnumerator RemoveShieldFromEntitiesBasedOnTag(string tag)
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject character in characters)
        {
            RemoveShieldFromEntity(character);
        }

        yield return null;

    }



    public void RemoveShieldFromEntity(GameObject entity)
    {

        EntityClass entityClass = entity.GetComponent<EntityClass>();

        //characterClass.InititializeCharacter();

        entityClass.shield = 0;

        //dont show the icon
        entityClass.shieldIcon.SetActive(false);

        //update text on shield
        entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();

        if (entityClass.armor > 0)
        {
            entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorOrange);
        }
        else
        {
            entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }


    }



    public void CalculateReceivedEntityDamage(EntityClass entityClass, int adjustNumber, bool bypassShield)
    {
        //logic
        int remainingAmount = adjustNumber;

        //shield
        if (entityClass.shield > 0 && remainingAmount > 0 && bypassShield == false)
        {
            entityClass.shield = entityClass.shield - remainingAmount;

            if (entityClass.shield >= 0)
            {
                remainingAmount = 0; //shield fully protected the damage
            }
            else
            {
                remainingAmount = -1 * entityClass.shield; //the remaining damage should go to the next calculations
                entityClass.shield = 0; //shield should not be negative
            }
        }

        //armor
        if (entityClass.armor > 0 && remainingAmount > 0)
        {
            entityClass.armor = entityClass.armor - remainingAmount;

            if (entityClass.armor >= 0)
            {
                remainingAmount = 0; //armor fully protected the damage
            }
            else
            {
                remainingAmount = -1 * entityClass.armor; //the remaining damage should go to the next calculations
                entityClass.armor = 0; //armor should not be negative
            }
        }

        //hp
        if (entityClass.health > 0 && remainingAmount > 0)
        {
            entityClass.health = entityClass.health - remainingAmount;

            if (entityClass.health <= 0)
            {
                entityClass.health = 0;
            }
        }
    }

    public void UpdateEntityDamageVisuals(EntityClass entityClass)
    {
        //update visuals

        //hp
        //update text on hp
        entityClass.healthText.GetComponent<TMP_Text>().text = entityClass.health + "/" + entityClass.maxHealth;

        //adjust the hp bar
        UI_Combat.Instance.UpdateHealthBarSmoothly(entityClass.health, entityClass.maxHealth, entityClass.slider);

        //flash enemy when directly hit
        StartCoroutine(FlashEntityAfterHit(entityClass.gameObject));

        FeedbackManager.Instance.PlayOnTarget(entityClass.transform, FeedbackManager.Instance.mm_Hit_Prefab);

        //make the bar red
        entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

        //armor
        if (entityClass.armor > 0)
        {
            //show icon
            entityClass.armorIcon.SetActive(true);
            //update text on shield
            entityClass.armorText.GetComponent<TMP_Text>().text = entityClass.armor.ToString();
            //make the bar blue
            entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorOrange);
        }
        else
        {
            entityClass.armorText.GetComponent<TMP_Text>().text = entityClass.armor.ToString();
            //hide the icon
            entityClass.armorIcon.SetActive(false);
        }

        //shield
        if (entityClass.shield > 0)
        {
            //show icon
            entityClass.shieldIcon.SetActive(true);
            //update text on shield
            entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();
            //make the bar blue
            entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
        }
        else
        {
            entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();
            //hide the icon
            entityClass.shieldIcon.SetActive(false);
        }
    }

    public IEnumerator AdjustTargetHealth(GameObject caster, GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {

        if (target == null)
        {
            yield return null;
        }

        EntityClass entityClass = target.GetComponent<EntityClass>();

        if (entityClass.entityMode == SystemManager.EntityMode.DEAD)
        {
            yield return null;
        }

        if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
        {

            CalculateReceivedEntityDamage(entityClass, adjustNumber, bypassShield);

            UpdateEntityDamageVisuals(entityClass);

            yield return StartCoroutine(BuffSystemManager.Instance.ActivateBuffsDebuffs_OnGettingHit(caster, target));


        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.COUNTER)
        {
            Debug.Log("COUNTER????");
            CalculateReceivedEntityDamage(entityClass, adjustNumber, bypassShield);

            UpdateEntityDamageVisuals(entityClass);


        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.HEAL)
        {
            //increase the hp

            entityClass.health = entityClass.health + adjustNumber;

            //check if max from hp
            if (entityClass.health > entityClass.maxHealth)
            {
                entityClass.health = entityClass.maxHealth;
            }

            //update text on hp
            entityClass.healthText.GetComponent<TMP_Text>().text = entityClass.health + "/" + entityClass.maxHealth;

            //adjust the hp bar
            UI_Combat.Instance.UpdateHealthBarSmoothly(entityClass.health, entityClass.maxHealth, entityClass.slider);

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
        {
            //increase the shield

            entityClass.shield = entityClass.shield + adjustNumber;

            //check if max from hp
            if (entityClass.shield > entityClass.maxShield)
            {
                entityClass.shield = entityClass.maxShield;
            }

            if (entityClass.shield > 0)
            {
                //show the icon
                entityClass.shieldIcon.SetActive(true);

                //update text on shield
                entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();

                //make the bar blue
                entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
            }

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.ARMOR)
        {
            //increase the shield

            entityClass.armor = entityClass.armor + adjustNumber;

            //check if max from hp
            if (entityClass.armor > 999)
            {
                entityClass.armor = 999;
            }


            if (entityClass.armor > 0)
            {
                //show the icon
                entityClass.armorIcon.SetActive(true);

                //update text on shield
                entityClass.armorText.GetComponent<TMP_Text>().text = entityClass.armor.ToString();

                //make the bar blue
                entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorOrange);
            }

            if (entityClass.shield > 0)
            {
                //show the icon
                entityClass.shieldIcon.SetActive(true);

                //update text on shield
                entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();

                //make the bar blue
                entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
            }


        }

        //spawn numberOn screen
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject numberOnScreenPrefab = Instantiate(UI_Combat.Instance.numberOnScreen, target.transform.position, Quaternion.identity);
        numberOnScreenPrefab.transform.SetParent(target.transform);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().color = AdjustNumberModeColor(adjustNumberMode);

        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().text = adjustNumber.ToString();

        Destroy(numberOnScreenPrefab, 1f);


        yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnEntityGetHit, null));

        //if the target reach to 0
        yield return StartCoroutine(CheckIfEntityIsDead(entityClass));


    }



    public IEnumerator CheckIfEntityIsDead(EntityClass entityClass)
    {


        if (CombatManager.Instance.trainingMode)
        {
            if (entityClass.gameObject.tag == "Player")
            {
                entityClass.health = 1; //player cannot die in training mode
            }
        }

        if (entityClass.health <= 0)
        {

            entityClass.entityMode = SystemManager.EntityMode.DEAD;
            Animator animator = entityClass.transform.Find("model").GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Dead");
            }

            if (entityClass.gameObject.tag == "Player")
            {
                yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerDeath, null));
                charactersAlive -= 1;
            }
            else if (entityClass.gameObject.tag == "Enemy")
            {
                yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnEnemyDeath, null));
                //enemiesAlive -= 1;
            }

            //if (SystemManager.Instance.GetPlayerTagsList().Contains(entityClass.gameObject.tag))
            //{
            //    characterFormation.Remove(entityClass.gameObject);
            //}
            //else
            //{
            //    enemyFormation.Remove(entityClass.gameObject);
            //}

            EntityDeadDestroy(entityClass.gameObject);

        }

    }

    public void EntityDeadDestroy(GameObject entity)
    {

        entity.GetComponent<EntityClass>().DestroyEntityInCombat();


    }

    public Color32 AdjustNumberModeColor(SystemManager.AdjustNumberModes adjustNumberMode)
    {
        Color32 colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

        if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.HEAL)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.ARMOR)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorOrange);
        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.COUNTER)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }

        return colorToChange;
    }

    public int CalculateEntityShield(int startingShield, GameObject entity, GameObject target, ScriptableCard scriptableCard = null)
    {

        if (entity == null)
        {
            return startingShield; //stops function
        }

        if (combatEnded)
        {
            return startingShield;
        }

        try
        {
            // Get character attack, debuff, and buff percentages
            int increaseStatValueInt = 0;
            float increaseStatValueFloat = 0;


            //get all buffs
            List<BuffDebuffClass> buffDebuffClassList = BuffSystemManager.Instance.GetAllBuffDebuffFromTarget(entity);

            //then loop
            foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassList)
            {

                //check if the buff has value to return
                var result = buffDebuffClass.scriptableBuffDebuff.OnModifyStats(entity, target, scriptableCard);

                //check if it has value then it modifies stats
                if (result.HasValue)
                {
                    var value = result.Value;
                    // use value

                    if (value.StatModifiedAttribute == SystemManager.StatModifiedAttribute.DEFENCE)
                    {

                        if (value.statModifiedType == SystemManager.StatModifiedType.NORMAL)
                        {
                            increaseStatValueInt += value.statIncreaseInt;
                        }
                        else if (value.statModifiedType == SystemManager.StatModifiedType.PERCENTAGE)
                        {
                            increaseStatValueFloat += value.statIncreaseFloat;
                        }

                    }
                }

            }

            // Calculate combined attack
            int combinedVar = startingShield + increaseStatValueInt;
            int resultVar = combinedVar;
            resultVar = Mathf.Max(0, Mathf.FloorToInt(resultVar + (combinedVar * (increaseStatValueFloat / 100))));


            return resultVar;
        }
        catch (Exception ex)
        {
            // Debug.LogError("Error Calculating Entity Damage : " + " : ERROR MSG : " + ex.Message);

            return 0;
        }
    }

    public int CalculateEntityArmor(int startingArmor, GameObject entity, GameObject target, ScriptableCard scriptableCard = null)
    {

        if (entity == null)
        {
            return startingArmor; //stops function
        }

        if (combatEnded)
        {
            return startingArmor;
        }

        try
        {
            // Get character attack, debuff, and buff percentages
            int increaseStatValueInt = 0;
            float increaseStatValueFloat = 0;


            //get all buffs
            List<BuffDebuffClass> buffDebuffClassList = BuffSystemManager.Instance.GetAllBuffDebuffFromTarget(entity);

            //then loop
            foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassList)
            {

                //check if the buff has value to return
                var result = buffDebuffClass.scriptableBuffDebuff.OnModifyStats(entity, target, scriptableCard);

                //check if it has value then it modifies stats
                if (result.HasValue)
                {
                    var value = result.Value;
                    // use value

                    if (value.StatModifiedAttribute == SystemManager.StatModifiedAttribute.ARMOR)
                    {

                        if (value.statModifiedType == SystemManager.StatModifiedType.NORMAL)
                        {
                            increaseStatValueInt += value.statIncreaseInt;
                        }
                        else if (value.statModifiedType == SystemManager.StatModifiedType.PERCENTAGE)
                        {
                            increaseStatValueFloat += value.statIncreaseFloat;
                        }

                    }
                }

            }

            // Calculate combined attack
            int combinedVar = startingArmor + increaseStatValueInt;
            int resultVar = combinedVar;
            resultVar = Mathf.Max(0, Mathf.FloorToInt(resultVar + (combinedVar * (increaseStatValueFloat / 100))));


            return resultVar;
        }
        catch (Exception ex)
        {
            // Debug.LogError("Error Calculating Entity Damage : " + " : ERROR MSG : " + ex.Message);

            return 0;
        }
    }


    public int CalculateEntityDmg(int startingDmg, GameObject entity, GameObject target, ScriptableCard scriptableCard = null)
    {

        if (entity == null)
        {
            return startingDmg; //stops function
        }

        if (combatEnded)
        {
            return startingDmg;
        }


        // Get character attack, debuff, and buff percentages
        int increaseStatValueInt = 0;
        float increaseStatValueFloat = 0;
        int fixedAmount = -1;


        //get all buffs
        List<BuffDebuffClass> buffDebuffClassList = BuffSystemManager.Instance.GetAllBuffDebuffFromTarget(entity);

        //then loop
        foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassList)
        {

            //check if the buff has value to return
            var result = buffDebuffClass.scriptableBuffDebuff.OnModifyStats(entity, target, scriptableCard);

            //check if it has value then it modifies stats
            if (result.HasValue)
            {
                var value = result.Value;
                // use value

                if (value.StatModifiedAttribute == SystemManager.StatModifiedAttribute.ATTACK)
                {

                    if (value.statModifiedType == SystemManager.StatModifiedType.NORMAL)
                    {
                        increaseStatValueInt += value.statIncreaseInt;
                    }
                    else if (value.statModifiedType == SystemManager.StatModifiedType.PERCENTAGE)
                    {
                        increaseStatValueFloat += value.statIncreaseFloat;
                    }

                }
            }

        }

        if (target != null)
        {
            //get all buffs
            List<BuffDebuffClass> buffDebuffClassListTarget = BuffSystemManager.Instance.GetAllBuffDebuffFromTarget(target);

            //then loop
            foreach (BuffDebuffClass buffDebuffClass in buffDebuffClassListTarget)
            {

                //check if the buff has value to return
                var result = buffDebuffClass.scriptableBuffDebuff.OnModifyStats_Target(entity, target, scriptableCard);

                //check if it has value then it modifies stats
                if (result.HasValue)
                {
                    var value = result.Value;
                    // use value

                    if (value.StatModifiedAttribute == SystemManager.StatModifiedAttribute.ATTACK)
                    {

                        if (value.statModifiedType == SystemManager.StatModifiedType.TARGETNORMAL)
                        {
                            increaseStatValueInt += value.statIncreaseInt;
                        }
                        else if (value.statModifiedType == SystemManager.StatModifiedType.TARGETPERCENTAGE)
                        {
                            increaseStatValueFloat += value.statIncreaseFloat;
                        }
                        else if (value.statModifiedType == SystemManager.StatModifiedType.TARGETFIXEDAMOUNT)
                        {
                            //we always get the lowest amount
                            if (fixedAmount == -1 || fixedAmount > value.statIncreaseInt)
                            {
                                fixedAmount = value.statIncreaseInt;
                            }

                        }

                    }
                }

            }
        }

        int resultVar = 0;
        if (fixedAmount != -1)
        {
            resultVar = fixedAmount;
        }
        else
        {
            // Calculate combined attack
            int combinedVar = startingDmg + increaseStatValueInt;
            resultVar = combinedVar;
            resultVar = Mathf.Max(0, Mathf.FloorToInt(resultVar + (combinedVar * (increaseStatValueFloat / 100))));
        }



        return resultVar;

    }

    public void PlayerEndTurnButton()
    {
        if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerTurn)
        {
            StartCoroutine(WaitEnemyTurns());
        }
    }



    public IEnumerator FlashEntityAfterHit(GameObject target)
    {
        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDamagedEntity, target);
        yield return new WaitForSeconds(0.5f);
        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, target);
    }


    public bool CheckIfEntityIsDeadAfterCard(GameObject target, int damage)
    {

        EntityClass entityClass = target.GetComponent<EntityClass>();


        int entityTotalHP = 0;

        //then do dmg to shield
        entityTotalHP = (entityClass.health + entityClass.shield) - damage;

        if (entityTotalHP <= 0)
        {
            entityClass.entityMode = SystemManager.EntityMode.DEAD;
            return true;
        }
        else
        {
            return false;
        }

    }


    public IEnumerator AttackSingleTargetEnemy(ScriptableCard scriptableCard, int damageAmount, int multiplyDamage, GameObject entityUsedCardGlobal, GameObject realTarget, int multiHits, float multiHitDuration = 2, bool pierce = false)
    {

        int calculatedDmg = Combat.Instance.CalculateEntityDmg(damageAmount * multiplyDamage, entityUsedCardGlobal, realTarget);

        //if dead mark is as dead
        //Combat.Instance.CheckIfEntityIsDeadAfterCard(realTarget, (calculatedDmg * multiHits));
        Animator entityAnimator = null;
        if (entityUsedCardGlobal != null)
        {
            entityAnimator = entityUsedCardGlobal.transform.Find("model").GetComponent<Animator>();
        }


        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(scriptableCard.entityAnimation.ToString());
        }

        for (int i = 0; i < multiHits; i++)
        {

            //if target dies during multi attack then stop
            if (realTarget == null)
            {
                break;
            }

            if (scriptableCard != null)
            {
                CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);
            }


            yield return StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, pierce, SystemManager.AdjustNumberModes.ATTACK));

            if (scriptableCard != null && scriptableCard.cardSoundEffect != null)
            {
                AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
            }

            // Wait between hits
            yield return new WaitForSeconds(scriptableCard.waitOnQueueTimer / multiHits);
        }
    }

    public IEnumerator AttackBlindlyEnemy(ScriptableCard scriptableCard, int damageAmount, GameObject entityUsedCardGlobal, GameObject realTarget, int multiHits, float multiHitDuration = 2, bool pierce = false)
    {


        //if dead mark is as dead
        //Combat.Instance.CheckIfEntityIsDeadAfterCard(realTarget, (calculatedDmg * multiHits));

        Animator entityAnimator = entityUsedCardGlobal.transform.Find("model").GetComponent<Animator>();

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(scriptableCard.entityAnimation.ToString());
        }

        for (int i = 0; i < multiHits; i++)
        {

            realTarget = AIManager.Instance.GetRandomTarget(entityUsedCardGlobal);
            int calculatedDmg = Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCardGlobal, realTarget);

            //if target dies during multi attack then stop
            if (realTarget == null)
            {
                break;
            }


            CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);


            yield return StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK));

            if (scriptableCard.cardSoundEffect != null)
            {
                AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
            }

            // Wait between hits
            yield return new WaitForSeconds(scriptableCard.waitOnQueueTimer / multiHits);
        }
    }


    public IEnumerator AttackAllEnemy(ScriptableCard scriptableCard, int damageAmount, GameObject entityUsedCardGlobal, List<GameObject> realTargets, int multiHits, float multiHitDuration = 2, bool pierce = false)
    {


        //if dead mark is as dead
        //Combat.Instance.CheckIfEntityIsDeadAfterCard(realTarget, (calculatedDmg * multiHits));

        Animator entityAnimator = entityUsedCardGlobal.transform.Find("model").GetComponent<Animator>();

        if (entityAnimator != null)
        {
            entityAnimator.SetTrigger(scriptableCard.entityAnimation.ToString());
        }

        for (int i = 0; i < multiHits; i++)
        {


            foreach (GameObject realTarget in realTargets)
            {
                //if target dies during multi attack then stop
                if (realTarget == null)
                {
                    break;
                }

                int calculatedDmg = Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCardGlobal, realTarget);

                CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);


                yield return StartCoroutine(Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK));

                if (scriptableCard.cardSoundEffect != null)
                {
                    AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
                }
            }

            // Wait between hits
            yield return new WaitForSeconds(scriptableCard.waitOnQueueTimer / multiHits);
        }
    }


    public List<EntityClass> ScriptableToEntityClass(List<ScriptableEntity> scriptableEntities)
    {
        List<EntityClass> entityClasses = new List<EntityClass>();

        foreach (ScriptableEntity scriptableEntity in scriptableEntities)
        {
            EntityClass entityCustomClass = new EntityClass();
            entityCustomClass.scriptableEntity = scriptableEntity;
            entityClasses.Add(entityCustomClass);
        }

        return entityClasses;
    }


    public IEnumerator SummonEntity(GameObject entityUsedCard, List<EntityClass> entityClasses, CardScriptData cardScriptData)
    {

        List<GameObject> summonedEntities = new List<GameObject>();

        foreach (EntityClass entityClass in entityClasses)
        {
            ScriptableEntity summonInCard = entityClass.scriptableEntity;

            GameObject summon;
            //get all targets
            if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
            {
                //allow to activate coroutine on scriptable object
                MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

                // Start the coroutine for each hit
                yield return runner.StartCoroutine(InstantiateEntity(summonInCard, null, "PlayerSummon", entityClass, cardScriptData, null));
            }
            else
            {

                //allow to activate coroutine on scriptable object
                MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

                // Start the coroutine for each hit
                yield return runner.StartCoroutine(InstantiateEntity(summonInCard, null, "EnemySummon", entityClass, cardScriptData, null));

            }



        }

        yield return null;



    }



    public void ModifyMana(int modifyMana)
    {

        ManaAvailable += modifyMana;

        if (ManaAvailable <= 0)
        {
            ManaAvailable = 0;
        }

    }


    public IEnumerator InitializeEvent()
    {


        UI_Combat.Instance.DisableCombatUI();

        //hide the dungeon generato
        if (StaticData.staticDungeonParent != null)
        {
            StaticData.staticDungeonParent.SetActive(false);
        }
        else
        {
            StaticData.staticDungeonParent = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("DungeonParent").gameObject;
            StaticData.staticDungeonParent.SetActive(false);
        }

        GameObject spaceship = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("spaceship").gameObject;
        GameObject spaceshipLine = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("spaceshipLine").gameObject;
        if (spaceship)
        {
            spaceship.SetActive(false);
            spaceshipLine.SetActive(false);
        }

        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

        //Debug.Log("Test dsck"  + DeckManager.Instance.scriptableDeck.mainDeck[0].scriptableCard.cardName);

        //reset turns
        Turns = 0;

        //play music
        AudioManager.Instance.PlayMusic("Combat_1");



        //destroy any previous characters
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Characters").gameObject));

        //destroy any previous enemies
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Enemies").gameObject));

        //destroy battleground
        battleground = GameObject.FindGameObjectWithTag("BattleGround");
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(battleground.transform.GetChild(0).gameObject));

        //spawn the characters
        yield return StartCoroutine(SpawnCharacters());

        //spawn the enemies
        yield return StartCoroutine(SpawnEnemies(true));

        //spawn battleground
        yield return StartCoroutine(SpawnBattleground());

        yield return StartCoroutine(SystemManager.Instance.TriggerLoadEndAnimation());

        UIManager.Instance.AnimateTextTypeWriter(CombatManager.Instance.ScriptableEvent.title, UIManager.Instance.turnText.GetComponent<TMP_Text>(), 4f);

        yield return new WaitForSeconds(0.5f);

        //show the Event UI
        yield return StartCoroutine(UIManager.Instance.StartUIEvent());

    }

    public void CalculateLootBoxRewards(int caseOfLoot) //0 = good, 1 = bad
    {

        List<ScriptablePlanets.ItemClassPlanet> loot = null;

        if (caseOfLoot == 0)
        {
            //good loot
            loot = CombatManager.Instance.scriptablePlanet.lootBoxGood;
        }
        else
        {
            //bad loot
            loot = CombatManager.Instance.scriptablePlanet.lootBoxBad;
        }

        foreach (ScriptablePlanets.ItemClassPlanet itemClassPlanet in loot)
        {

            //based on min and max value
            int quantity = UnityEngine.Random.Range(itemClassPlanet.minQuantity, itemClassPlanet.maxQuantity);

            int percQuantity = 0;

            if (itemClassPlanet.percentage != 0)
            {


                //then give value based on range
                for (int i = 0; i < quantity; i++)
                {

                    int randomChance = UnityEngine.Random.Range(0, 100);

                    if (randomChance <= itemClassPlanet.percentage)
                    {
                        percQuantity++; //add quantity by 1
                    }

                }

                quantity = percQuantity;

            }



            //if 0 then nothing should be added on loot
            if (quantity == 0)
            {
                continue;
            }

            //create the classItem
            ClassItemData classItem = new ClassItemData(itemClassPlanet.scriptableItem, quantity);

            //then add item to loot
            StaticData.lootItemList.Add(classItem);

            //ItemManager.Instance.AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);
            // yield return new WaitForSeconds(0.1f);
        }
        //yield return null;

        //yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnLoot));

    }

    public List<CombatPosition> CheckCardTargets(GameObject target, ScriptableCard scriptableCard)
    {

        //spawn target on the target
        //Instantiate(UI_Combat.Instance.targettingPrefab, target.transform.position, Quaternion.identity);

        //ScriptableCard scriptableCard = targetUIElement.gameObject.GetComponent<CardScript>().scriptableCard;
        List<CombatPosition> combatPositionsToCheck = new List<CombatPosition>();

        if (SystemManager.Instance.GetPlayerTagsList().Contains(target.tag) || target.transform.parent.tag == "PlayerPos")
        {
            combatPositionsToCheck = Combat.Instance.characterCombatPositions;
        }
        else if (SystemManager.Instance.GetEnemyTagsList().Contains(target.tag) || target.transform.parent.tag == "EnemyPos")
        {
            combatPositionsToCheck = Combat.Instance.enemiesCombatPositions;
        }


        List<CombatPosition> combatPositions = Combat.Instance.GetCardTargetingCombatPositions(combatPositionsToCheck, scriptableCard, target);

        return combatPositions;
    }

    public List<CombatPosition> GetCardTargetingCombatPositions(List<CombatPosition> combatPositionsToCheck, ScriptableCard scriptableCard, GameObject target)
    {
        List<CombatPosition> combatPositions = new List<CombatPosition>();

        CombatPosition combatPosition = GetCombatPosition(target);

        //add the first clicked position
        combatPositions.Add(combatPosition);

        //then start the targeting
        int listIndex = combatPositionsToCheck.IndexOf(combatPosition);

        //ignore the first one
        if (scriptableCard.toggle1)
        {

        }

        listIndex = GoThoughListNextIndex(combatPositionsToCheck, listIndex);

        if (scriptableCard.toggle2)
        {
            combatPositions.Add(combatPositionsToCheck[listIndex]);
        }

        listIndex = GoThoughListNextIndex(combatPositionsToCheck, listIndex);

        if (scriptableCard.toggle3)
        {
            combatPositions.Add(combatPositionsToCheck[listIndex]);
        }

        listIndex = GoThoughListNextIndex(combatPositionsToCheck, listIndex);

        if (scriptableCard.toggle4)
        {
            combatPositions.Add(combatPositionsToCheck[listIndex]);
        }

        listIndex = GoThoughListNextIndex(combatPositionsToCheck, listIndex);

        if (scriptableCard.toggle5)
        {
            combatPositions.Add(combatPositionsToCheck[listIndex]);
        }

        return combatPositions;

    }


    public int GoThoughListNextIndex(List<CombatPosition> combatPositionsToCheck, int currentIndex)
    {
        //go to next index
        currentIndex++;

        //but loop if it goes over the list index
        if (currentIndex >= combatPositionsToCheck.Count)
        {
            //go back to first index
            currentIndex = 0;
        }

        return currentIndex;

    }

    public void AddHazard(ScriptableHazard scriptableHazard, CombatPosition combatPosition)
    {

        //can replace hazards


        combatPosition.hazard = scriptableHazard;

        //show icon
        combatPosition.position.transform.Find("Hazard").gameObject.SetActive(true);
        combatPosition.position.transform.Find("Hazard").GetComponent<TooltipContent>().description = "<color=#" + SystemManager.Instance.colorGolden + ">" + scriptableHazard.hazardName + "</color> : " + scriptableHazard.hazardDescription;
        combatPosition.position.transform.Find("Hazard").Find("Icon").GetComponent<Image>().sprite = scriptableHazard.hazardIcon;



    }


    public void RemoveHazard(ScriptableHazard scriptableHazard, CombatPosition combatPosition)
    {
        combatPosition.hazard = null;

        //show icon
        combatPosition.position.transform.Find("Hazard").gameObject.SetActive(false);
    }

    public IEnumerator RemoveHazardIE(ScriptableHazard scriptableHazard, CombatPosition combatPosition)
    {
        combatPosition.hazard = null;

        //show icon
        combatPosition.position.transform.Find("Hazard").gameObject.SetActive(false);

        yield return null;
    }

}
