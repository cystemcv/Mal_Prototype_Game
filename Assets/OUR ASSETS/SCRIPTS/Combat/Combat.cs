using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScriptableCard;

public class PlayedCard
{

    public bool isPlaying = false;
    public float timer;
    public GameObject target;
    public ScriptableCard scriptableCard;
    public CardScript cardScript;
    public GameObject cardObject;
    public GameObject playedCardUI;
}

public class Combat : MonoBehaviour
{
    public static Combat Instance;


    [Header("SPAWNS")]
    public GameObject characterStartSpawn;
    public GameObject enemyStartSpawn;


    [Header("COMBAT FORMATIONS")]
    public List<GameObject> characterFormation;
    public List<GameObject> enemyFormation;
    public int characterCount = 0;
    public int enemyCount = 0;
    public int maxPlayerSummons = 3;
    public int maxEnemies = 7;

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
    public List<CardScript> discardEffectCardList = new List<CardScript>();

    public SystemManager.BattleGroundType battleGroundType;

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
        StartCombat();
    }

    // Start is called before the first frame update
    void Start()
    {

        //StaticData.companionItemList.Add(ItemManager.Instance.SOItemToClass(StaticData.staticScriptableCompanion.companionItemList[0]));

    }


    public void Update()
    {

        if (conditionsEnabled == true &&
            (charactersAlive <= 0 || enemyFormation.Count <= 0))
        {
            CombatOver();
        }

        //check the queue
        if (playedCardList.Count > 0)
        {
      
            //check if the card is played
            //play the card
            if (playedCardList[0].isPlaying == false)
            {

                //save the cardscript
                DeckManager.Instance.savedPlayedCardScript = playedCardList[0].cardScript;

                DeckManager.Instance.PlayerPlayedCard(playedCardList[0].cardScript);

                playedCardList[0].isPlaying = true;

      


            }

            //if the card is playing then start decreasing the timer
            if (playedCardList[0].timer > 0 && playedCardList[0].isPlaying == true)
            {
                playedCardList[0].timer -= Time.deltaTime;
                playedCardList[0].timer = Mathf.Max(playedCardList[0].timer,0);
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
        UI_Combat.Instance.manaInfo.transform.Find("Text").GetComponent<TMP_Text>().text = manaAvailable.ToString();

        //go throught each card in the hand and update them
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            UpdateCardAfterManaChange(cardPrefab);
        }

    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (manaAvailable < cardScript.primaryManaCost)
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

        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnCombatEnd);
        combatEnded = true;

        //always check characters first, if both lost = game over
        if (charactersAlive <= 0)
        {

            //remove the dungeon
            StaticData.staticDungeonParent = null;

            //lose
            UI_Combat.Instance.gameover.SetActive(true);
        }
        else if (enemyFormation.Count <= 0)
        {

            //get the player
            GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
            EntityClass entityClass = playerCharacter.GetComponent<EntityClass>();

            //save character
            StaticData.staticCharacter.maxHealth = entityClass.maxHealth;
            StaticData.staticCharacter.currHealth = entityClass.health;

            StaticData.lootItemList.Clear();

            CalculateLootRewards();

            //win
            UI_Combat.Instance.victory.SetActive(true);
            ItemManager.Instance.ShowInventory();
            ItemManager.Instance.ShowLoot();
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
            ClassItem classItem = new ClassItem(itemClassPlanet.scriptableItem, quantity);

            //then add item to loot
            StaticData.lootItemList.Add(classItem);

            //ItemManager.Instance.AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);

        }


        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnLoot);

    }

    public IEnumerator InitializeCombat()
    {
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

        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

        //Debug.Log("Test dsck"  + DeckManager.Instance.scriptableDeck.mainDeck[0].scriptableCard.cardName);

        //reset turns
        Turns = 0;

        //play music
        AudioManager.Instance.PlayMusic("Combat_1");

        //clear all lists
        yield return StartCoroutine(ClearLists());

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
        yield return StartCoroutine(SpawnEnemies());

        //spawn battleground
        yield return StartCoroutine(SpawnBattleground());

        yield return new WaitForSeconds(0.5f);

        //initialize all things card related, deck, combat deck, discard pile, etc
        yield return StartCoroutine(InitializeCardsAndDecks());


        //check the characters that are alive
        yield return StartCoroutine(CheckCharactersAlive());

        //check the enemies that are alive
        //yield return StartCoroutine(CheckEnemiesAlive());

        //start win conditions
        conditionsEnabled = true;

        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnCombatStart);

        StaticData.staticScriptableCompanion.InitializeButton();

        //start the player
        yield return StartCoroutine(WaitPlayerTurns());
    }


    public IEnumerator WaitEnemyTurns()
    {

        UI_Combat.Instance.endTurnButton.GetComponent<Animator>().SetTrigger("PlayerEnd");

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

    public IEnumerator ClearLists()
    {
        characterFormation.Clear();
        enemyFormation.Clear();


        yield return null;
    }

    public IEnumerator PlayerTurnStart()
    {



        if (combatEnded)
        {
            yield return null; //stops function
        }



        //start player turn
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

        //increase turn;
        Turns += 1;

        yield return StartCoroutine(RearrangeFormation(characterFormation));

        //mana should go back to full
        yield return StartCoroutine(RefillMana());

        //remove mana
        yield return StartCoroutine(RemoveShieldFromEntitiesBasedOnTag("Player"));

        //generate intend for all players
        yield return StartCoroutine(AIManager.Instance.GenerateIntends(SystemManager.Instance.GetPlayerTagsList()));
        yield return StartCoroutine(AIManager.Instance.GenerateIntends(SystemManager.Instance.GetEnemyTagsList()));

        //do the ai logic for each enemy
        yield return StartCoroutine(AIManager.Instance.AiAct(SystemManager.Instance.GetPlayerTagsList()));

        //yield return StartCoroutine(LowerSummonTurns(SystemManager.Instance.GetPlayerTagsList()));

        //loop for all buffs and debuffs
        yield return StartCoroutine(BuffSystemManager.Instance.ActivateAllBuffsDebuffs());

        //draw cards
        yield return StartCoroutine(DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit, 0));

        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerTurnStart);

        yield return StartCoroutine(ActivateDelayedCardEffects());


        yield return null; // Wait for a frame 
    }

    public IEnumerator ActivateDelayedCardEffects()
    {

        foreach (CardScript cardScript in discardEffectCardList)
        {
            cardScript.scriptableCard.OnDelayEffect(cardScript);
           
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
        UI_Combat.Instance.endTurnButton.GetComponent<Animator>().SetTrigger("EnemyEnd");
        UI_Combat.Instance.OnNotification("PLAYER TURN", 1);
        //}




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
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerTurnEnd);

        yield return new WaitForSeconds(1f);

        yield return null; //skip frame

    }

    public IEnumerator EnemyTurnStart()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }


        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
        UI_Combat.Instance.OnNotification("ENEMY TURN", 1);

        yield return RearrangeFormation(enemyFormation);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

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
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        yield return null;
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

        //instantiate our character or characters
        GameObject characterInCombat = InstantiateCharacter(StaticData.staticCharacter);

        //assign the characters in combat
        //CharacterManager.Instance.charactersInAdventure.Add(characterInCombat);

        //initialize the stats
        StartCoroutine(characterInCombat.GetComponent<EntityClass>().InititializeEntity());

        yield return null; // Wait for a frame 


    }

    public IEnumerator SpawnEnemies()
    {

        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in CombatManager.Instance.scriptablePlanet.scriptableEntities)
        {
            //instantiate our character or characters
            GameObject enemyInCombat = InstantiateEnemies(scriptableEntity);

            //assign the characters in combat
            //CharacterManager.Instance.charactersInAdventure.Add(enemyInCombat);

            //initialize the stats
            StartCoroutine(enemyInCombat.GetComponent<EntityClass>().InititializeEntity());


            yield return null; // Wait for a frame 
        }

    }

    public IEnumerator SpawnBattleground()
    {

        GameObject bg = Instantiate(CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGround, battleground.transform.position, Quaternion.identity);
        bg.transform.SetParent(battleground.transform);

        battleGroundType = CombatManager.Instance.scriptablePlanet.planetBattleGround.battleGroundType;

        yield return null; // Wait for a frame 

    }

    public GameObject InstantiateCharacter(ScriptableEntity scriptableEntity)
    {


        Vector3 spawn = new Vector3(0, 0, 0);

        //get distance needed
        float distance = GetDistanceOfFormation(characterFormation, characterFormation.Count);

        spawn = new Vector3(characterStartSpawn.transform.position.x + distance, characterStartSpawn.transform.position.y, characterStartSpawn.transform.position.z);

        //instantiate the character prefab based on the spawnPosition
        GameObject entity = Instantiate(scriptableEntity.entityPrefab, spawn, Quaternion.identity);

        //pass it the scriptable object to the class
        entity.GetComponent<EntityClass>().scriptableEntity = scriptableEntity;
        //svae the position as it can be used later
        entity.GetComponent<EntityClass>().originalCombatPos = spawn;

        entity.transform.SetParent(this.gameObject.transform.Find("Characters"));

        if (entity.GetComponent<AIBrain>() != null)
        {

            FlipSprite(entity);
        }
        else
        {
            //entity.transform.Find("gameobjectUI").Find("DisplayCardName").gameObject.SetActive(false);
        }

        //assign the entity to formation

        characterFormation.Add(entity);

        //reassign them
        RearrangeFormation(characterFormation);

        return entity;

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

    public GameObject InstantiateEnemies(ScriptableEntity scriptableEntity)
    {

        Vector3 spawn = new Vector3(0, 0, 0);

        //get distance needed
        float distance = GetDistanceOfFormation(enemyFormation, enemyFormation.Count);

        spawn = new Vector3(enemyStartSpawn.transform.position.x - distance, enemyStartSpawn.transform.position.y, enemyStartSpawn.transform.position.z);

        //instantiate the character prefab based on the spawnPosition
        GameObject entity = Instantiate(scriptableEntity.entityPrefab, spawn, Quaternion.identity);

        //pass it the scriptable object to the class
        entity.GetComponent<EntityClass>().scriptableEntity = scriptableEntity;
        //svae the position as it can be used later
        entity.GetComponent<EntityClass>().originalCombatPos = spawn;

        entity.transform.SetParent(this.gameObject.transform.Find("Enemies"));

        //assign the entity to formation
        enemyFormation.Add(entity);

        //reassign them
        RearrangeFormation(enemyFormation);

        return entity;
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

    public void StartCombat()
    {
        StartCoroutine(InitializeCombat());

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
        DeckManager.Instance.combatDeck = new List<CardScript>(StaticData.staticMainDeck);
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

    public void AdjustTargetHealth(GameObject attacker, GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {

        if (target == null)
        {
            return;
        }

        EntityClass entityClass = target.GetComponent<EntityClass>();

        if (entityClass.entityMode == SystemManager.EntityMode.DEAD)
        {
            return;
        }

        if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
        {

            CalculateReceivedEntityDamage(entityClass,adjustNumber,bypassShield);

            UpdateEntityDamageVisuals(entityClass);

            //then calculate counter attack
            if (target.GetComponent<EntityClass>().counterDamage > 0)
            {
                AdjustTargetHealth(target, attacker, target.GetComponent<EntityClass>().counterDamage, false, SystemManager.AdjustNumberModes.COUNTER);
            }


        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.COUNTER)
        {

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


        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnEntityGetHit);

        //if the target reach to 0
        CheckIfEntityIsDead(entityClass);


    }



    public void CheckIfEntityIsDead(EntityClass entityClass)
    {

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
                ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayerDeath);
                charactersAlive -= 1;
            }
            else if (entityClass.gameObject.tag == "Enemy")
            {
                ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnEnemyDeath);
                //enemiesAlive -= 1;
            }

            if (SystemManager.Instance.GetPlayerTagsList().Contains(entityClass.gameObject.tag))
            {
                characterFormation.Remove(entityClass.gameObject);
            }
            else
            {
                enemyFormation.Remove(entityClass.gameObject);
            }

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

    public int CalculateEntityShield(int startingShield, GameObject entity, GameObject target)
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
            int entity_Defence = entity.GetComponent<EntityClass>().defence;
            float entity_defenceDebuffPerc = entity.GetComponent<EntityClass>().defenceDebuffPerc;
            float entity_defenceBuffPerc = entity.GetComponent<EntityClass>().defenceBuffPerc;

            // Calculate combined attack
            int combinedDefence = startingShield + entity_Defence;

            // Apply debuff and buff multiplicatively
            float finalDefenceMultiplier = 1 + (entity_defenceBuffPerc / 100) - (entity_defenceDebuffPerc / 100);

            // Check if the enemy is vulnerable and apply additional damage multiplier
            //bool isVulnerable = enemy.GetComponent<EnemyClass>().isVulnerable; // Assume the enemy class has an isVulnerable property
            //if (isVulnerable)
            //{
            //    finalAttackMultiplier += 0.25f; // Apply 25% more damage
            //}

            // Calculate final damage and clamp to a minimum of zero
            int finalDmg = Mathf.Max(0, Mathf.FloorToInt((combinedDefence + tempBoostDefence) * finalDefenceMultiplier));

            return finalDmg;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Calculating Entity Shield : " + " : ERROR MSG : " + ex.Message);

            return 0;
        }
    }

    public int CalculateEntityArmor(int startingArmor, GameObject entity, GameObject target)
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
            int entity_Resistance = entity.GetComponent<EntityClass>().resistance;
            float entity_resistanceDebuffPerc = entity.GetComponent<EntityClass>().resistanceDebuffPerc;
            float entity_resistanceBuffPerc = entity.GetComponent<EntityClass>().resistanceBuffPerc;

            // Calculate combined attack
            int combinedResistance = startingArmor + entity_Resistance;

            // Apply debuff and buff multiplicatively
            float finalResistanceMultiplier = 1 + (entity_resistanceBuffPerc / 100) - (entity_resistanceDebuffPerc / 100);

            // Check if the enemy is vulnerable and apply additional damage multiplier
            //bool isVulnerable = enemy.GetComponent<EnemyClass>().isVulnerable; // Assume the enemy class has an isVulnerable property
            //if (isVulnerable)
            //{
            //    finalAttackMultiplier += 0.25f; // Apply 25% more damage
            //}

            // Calculate final damage and clamp to a minimum of zero
            int finalDmg = Mathf.Max(0, Mathf.FloorToInt((combinedResistance + tempBoostResistance) * finalResistanceMultiplier));

            return finalDmg;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Calculating Entity Shield : " + " : ERROR MSG : " + ex.Message);

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

        try
        {
            // Get character attack, debuff, and buff percentages
            int entity_Attack = entity.GetComponent<EntityClass>().attack;
            float entity_attackDebuffPerc = entity.GetComponent<EntityClass>().attackDebuffPerc;
            float entity_attackBuffPerc = entity.GetComponent<EntityClass>().attackBuffPerc;

            // Calculate combined attack
            int combinedAttack = startingDmg + entity_Attack;

            // Apply debuff and buff multiplicatively
            float finalAttackMultiplier = 1 + (entity_attackBuffPerc / 100) - (entity_attackDebuffPerc / 100);

            // Check if the enemy is vulnerable and apply additional damage multiplier
            //bool isVulnerable = enemy.GetComponent<EnemyClass>().isVulnerable; // Assume the enemy class has an isVulnerable property
            //if (isVulnerable)
            //{
            //    finalAttackMultiplier += 0.25f; // Apply 25% more damage
            //}

            // Calculate final damage and clamp to a minimum of zero
            int finalDmg = Mathf.Max(0, Mathf.FloorToInt((combinedAttack + tempBoostAttack) * finalAttackMultiplier));

            if (scriptableCard != null)
            {

                //monster buff
                if (scriptableCard.mainClass == SystemManager.MainClass.MONSTER)
                {
                    finalDmg += Combat.Instance.tempMonsterAttackBoost;
                }

            }

            return finalDmg;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Calculating Entity Damage : " + " : ERROR MSG : " + ex.Message);

            return 0;
        }
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


            CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);


            Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, pierce, SystemManager.AdjustNumberModes.ATTACK);

            if (scriptableCard.cardSoundEffect != null)
            {
                AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
            }

            // Wait between hits
            yield return new WaitForSeconds(scriptableCard.waitOnQueueTimer / multiHits);
        }
    }

    public IEnumerator AttackBlindlyEnemy(ScriptableCard scriptableCard, int damageAmount, GameObject entityUsedCardGlobal, GameObject realTarget, int multiHits, float multiHitDuration = 2, bool pierce = false)
    {
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(damageAmount, entityUsedCardGlobal, realTarget);

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


            //if target dies during multi attack then stop
            if (realTarget == null)
            {
                break;
            }


            CombatManager.Instance.SpawnEffectPrefab(realTarget, scriptableCard);


            Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

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


                Combat.Instance.AdjustTargetHealth(entityUsedCardGlobal, realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

                if (scriptableCard.cardSoundEffect != null)
                {
                    AudioManager.Instance.cardSource.PlayOneShot(scriptableCard.cardSoundEffect);
                }
            }

            // Wait between hits
            yield return new WaitForSeconds(scriptableCard.waitOnQueueTimer / multiHits);
        }
    }



    public List<GameObject> SummonEntity(GameObject entityUsedCard, List<ScriptableEntity> scriptableEntities, EntityCustomClass entityCustomClass = null)
    {

        List<GameObject> summonedEntities = new List<GameObject>();

        foreach (ScriptableEntity summonInCard in scriptableEntities)
        {


            GameObject summon;
            //get all targets
            if (SystemManager.Instance.GetPlayerTagsList().Contains(entityUsedCard.tag))
            {

                GameObject[] summons = GameObject.FindGameObjectsWithTag("PlayerSummon");

                //check if it reach the limit
                if (summons.Length >= Combat.Instance.maxPlayerSummons)
                {
                    return null;
                }
                else
                {
                    summon = Combat.Instance.InstantiateCharacter(summonInCard);

                    summon.tag = "PlayerSummon";

                    //allow to activate coroutine on scriptable object
                    MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                                       //hit at least one time if its 0

                    // Start the coroutine for each hit
                    runner.StartCoroutine(summon.GetComponent<EntityClass>().InititializeEntity(entityCustomClass));


                    //initialize 
                    summon.GetComponent<AIBrain>().GenerateIntend();

                    summonedEntities.Add(summon);
                }


            }
            else
            {

                GameObject[] summons = GameObject.FindGameObjectsWithTag("EnemySummon");

                //check if it reach the limit
                if (enemyFormation.Count >= Combat.Instance.maxEnemies)
                {
                    return null;
                }
                else
                {
                    //enemiesAlive++;

                    summon = Combat.Instance.InstantiateEnemies(summonInCard);

                    summon.tag = "EnemySummon";

                    //initialize the stats
                    //allow to activate coroutine on scriptable object
                    MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                                       //hit at least one time if its 0

                    // Start the coroutine for each hit
                    runner.StartCoroutine(summon.GetComponent<EntityClass>().InititializeEntity(entityCustomClass));


                    //initialize 
                    summon.GetComponent<AIBrain>().GenerateIntend();

                    summonedEntities.Add(summon);
                }


            }



        }

        return summonedEntities;



    }

    public void ModifyMana(int modifyMana)
    {

        ManaAvailable += modifyMana;

        if (ManaAvailable <= 0)
        {
            ManaAvailable = 0;
        }

    }

}
