using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("ENEMIES IN BATTLE")]
    public List<ScriptableEntity> scriptableEnemyList;


    //targeting system


    public GameObject discardEffect;
    public GameObject banishEffect;
    public GameObject arrowHead;

    public RectTransform targetUIElement;
    public LineRenderer lineRenderer;
    public GameObject targetClicked;

    //our combat scene
    public GameObject combatScene;

    public List<GameObject> characterSpawns;


    public int turns = 0;

    //win /lose conditions
    public int charactersAlive = 0;
    public int enemiesAlive = 0;



    //---------------

    //damage system
    public GameObject numberOnScreen;

    //---------------

    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;

    //ui healthbar colors
    private float fillbarVelocity = 0;
    private float fillbarSmoothValue = 0.3f;

    public int tempBoostAttack = 0;


    //leader
    //public GameObject leaderCharacter;
    public GameObject leaderIndicator;




    //public void EntityDeadDestroy(GameObject entity)
    //{
    //    GameObject buffsdebuffs = entity.transform.Find("gameobjectUI").Find("BuffDebuffList").Find("Panel").gameObject;
    //    GameObject intends = null;
    //    if (entity.tag == "Enemy") {
    //         intends = entity.transform.Find("gameobjectUI").Find("intendList").Find("intends").gameObject;
    //    }

    //    if (intends != null) {
    //        SystemManager.Instance.DestroyAllChildren(intends);
    //    }

    //    if (buffsdebuffs != null)
    //    {
    //        SystemManager.Instance.DestroyAllChildren(buffsdebuffs);
    //    }

    //}

    //public int CalculateEntityDmg(int startingDmg, GameObject entity, GameObject target)
    //{
    //    // Get character attack, debuff, and buff percentages
    //    int entity_Attack = entity.GetComponent<EntityClass>().attack;
    //    float entity_attackDebuffPerc = entity.GetComponent<EntityClass>().attackDebuffPerc;
    //    float entity_attackBuffPerc = entity.GetComponent<EntityClass>().attackBuffPerc;

    //    // Calculate combined attack
    //    int combinedAttack = startingDmg + entity_Attack;

    //    // Apply debuff and buff multiplicatively
    //    float finalAttackMultiplier = 1 + (entity_attackBuffPerc / 100) - (entity_attackDebuffPerc / 100);

    //    // Check if the enemy is vulnerable and apply additional damage multiplier
    //    //bool isVulnerable = enemy.GetComponent<EnemyClass>().isVulnerable; // Assume the enemy class has an isVulnerable property
    //    //if (isVulnerable)
    //    //{
    //    //    finalAttackMultiplier += 0.25f; // Apply 25% more damage
    //    //}

    //    // Calculate final damage and clamp to a minimum of zero
    //    int finalDmg = Mathf.Max(0, Mathf.FloorToInt((combinedAttack+ tempBoostAttack) * finalAttackMultiplier));

    //    return finalDmg;
    //}

    //public Color32 AdjustNumberModeColor(SystemManager.AdjustNumberModes adjustNumberMode)
    //{
    //    Color32 colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

    //    if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
    //    {
    //        colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
    //    }
    //    else if (adjustNumberMode == SystemManager.AdjustNumberModes.HEAL)
    //    {
    //        colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed); ;
    //    }
    //    else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
    //    {
    //        colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue); ;
    //    }

    //    return colorToChange;
    //}

    //public void UpdateHealthBarSmoothly(float health, float maxHealth, Slider slider)
    //{
    //    StartCoroutine(SmoothUpdateHealthBar(health, maxHealth, slider));
    //}

    //private IEnumerator SmoothUpdateHealthBar(float health, float maxHealth, Slider slider)
    //{
    //    float targetValue = health / maxHealth;
    //    while (Mathf.Abs(slider.value - targetValue) > 0.01f)
    //    {
    //        slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref fillbarVelocity, fillbarSmoothValue);
    //        yield return null; // Wait for the next frame
    //    }
    //    slider.value = targetValue; // Ensure it's set to the target value at the end
    //}

    //public void UpdateCardAfterManaChange(GameObject cardPrefab)
    //{
    //    ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
    //    CardScript cardScript = cardPrefab.GetComponent<CardScript>();
    //    TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


    //    //check the mana cost of each
    //    if (CombatManager.Instance.manaAvailable < cardScript.primaryManaCost)
    //    {
    //        //cannot be played
    //        cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
    //    }
    //    else
    //    {
    //        //can be played
    //        cardManaCostText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
    //    }
    //}

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

    //public void RefillMana()
    //{
    //    //initialize mana and UI
    //    UI_Combat.Instance.ManaAvailable = manaMaxAvailable;
    //}

    //public void StartCombat()
    //{

    //    ////hide ui
    //    //SystemManager.Instance.uiManager.SetActive(true);
    //    //SystemManager.Instance.combatManager.SetActive(true);
    //    //SystemManager.Instance.combatScene.SetActive(true);

    //    ////generate dungeon
    //    //SystemManager.Instance.dungeonGeneratorManager.SetActive(false);



    //    ////close the UI window
    //    //UIManager.Instance.UIMENU.SetActive(false);
    //    //UIManager.Instance.bgCanvas.SetActive(false);
    //    //UIManager.Instance.uiParticles.SetActive(false);

    //    //reset turns
    //    turns = 0;

    //    ////open the UI combat menu
    //    //UIManager.Instance.UICOMBAT.SetActive(true);

    //    //play music
    //    AudioManager.Instance.PlayMusic("Combat_1");

    //    //give mana to player
    //    RefillMana();

    //    //initialize the combat deck
    //    DeckManager.Instance.combatDeck.Clear();
    //    DeckManager.Instance.combatDeck = new List<CardScript>(DeckManager.Instance.mainDeck);
    //    DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);

    //    //clean up discard pile
    //    DeckManager.Instance.discardedPile.Clear();

    //    //clean up banished pile
    //    DeckManager.Instance.banishedPile.Clear();

    //    //asign leader the first character
    //    //AssignLeader(CharacterManager.Instance.charactersInAdventure[0]);

    //    //initialize dead count
    //    GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

    //    charactersAlive = 0;
    //    foreach (GameObject character in characters)
    //    {
    //        //count how many alive
    //        if (character.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD )
    //        {
    //            charactersAlive += 1;
    //        }
    //    }

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

    //    //change into combat mode
    //    SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

    //    StartCoroutine(WaitPlayerTurns());



    //    //initialize the characters

    //    //initialize the enemies

    //    //we draw for each character




    //}

    //public void AssignLeader(GameObject characterInCombat)
    //{
    //    //assign the leader
    //    leaderCharacter = characterInCombat;

    //    //position the ui indicator on the leader
    //    leaderIndicator.SetActive(true);
    //    leaderIndicator.transform.position = new Vector2(leaderCharacter.transform.position.x, leaderCharacter.GetComponent<EntityClass>().scriptableEntity.leaderIndicatorHeight);

    //    //parent it
    //    leaderIndicator.transform.SetParent(leaderCharacter.transform);

    //}

    //public void ReverseLeader()
    //{
    //    foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
    //    {

    //        //if (leaderCharacter != characterInCombat)
    //        //{
    //        //    AssignLeader(characterInCombat);
    //        //    break;
    //        //}

    //    }
    //}

    //public void AssignLeaderToAlive()
    //{
    //    foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
    //    {

    //        //if (characterInCombat.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
    //        //{
    //        //    AssignLeader(characterInCombat);
    //        //    break;
    //        //}

    //    }
    //}


    //public void PlayerTurnStart()
    //{
    //    //start player turn
    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

    //    //increase turn;
    //    turns += 1;

    //    //mana should go back to full
    //    RefillMana();

    //    //remove mana
    //    RemoveShieldFromEntitiesBasedOnTag("Player");

    //    //draw cards
    //    DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit);

    //    UI_Combat.Instance.OnNotification("PLAYER STARTING TURN", 1);

    //    //check if its not turn 1 and also only in duo mode
 

    //    //if there is only 1 character then assign it to it
    //    if (charactersAlive == 1)
    //    {
    //        AssignLeaderToAlive();
    //    }
    //    else
    //    {
    //        if (turns != 1)
    //        {
    //            ReverseLeader();
    //        }
    //    }

    //    //loop for all buffs and debuffs
    //    BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

    //    //generate intend for all enemies
    //    GenerateEnemyIntends();
    //}

    //public void GenerateEnemyIntends()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    List<GameObject> enemyList = enemies.ToList();

    //    foreach (GameObject enemy in enemyList)
    //    {

    //        //destroy intends
    //        SystemManager.Instance.DestroyAllChildren(enemy.transform.Find("gameobjectUI").Find("intendList").Find("intends").gameObject);

    //        //generate new intends
    //        enemy.GetComponent<AIBrain>().GenerateIntend();
    //    }

    //}

    //public void PlayerTurn()
    //{
    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerTurn;
    //    UI_Combat.Instance.OnNotification("PLAYER TURN", 1);

    //}

    //IEnumerator WaitPlayerTurns()
    //{
    //    yield return new WaitForSeconds(2f);
    //    //player turn start
    //    PlayerTurnStart();
    //    yield return new WaitForSeconds(2f);

    //    //player turn
    //    PlayerTurn();

    //}

    //public void PlayerEndTurnButton()
    //{
    //    if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerTurn)
    //    {
    //        StartCoroutine(WaitEnemyTurns());
    //    }
    //}

    //public void PlayerTurnEnd()
    //{

    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerEndTurn;
    //    UI_Combat.Instance.OnNotification("PLAYER ENDING TURN", 1);

    //    //discard cards
    //    DeckManager.Instance.DiscardWholeHand();

    //    //loop for all buffs and debuffs
    //    BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

    //}

    //public void EnemyTurnStart()
    //{
    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
    //    UI_Combat.Instance.OnNotification("ENEMY STARTING TURN", 1);

    //    //loop for all buffs and debuffs
    //    BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

    //    //remove shield from all enemies
    //    RemoveShieldFromEntitiesBasedOnTag("Enemy");
    //}

    //public void EnemyTurn()
    //{
    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyTurn;
    //    UI_Combat.Instance.OnNotification("ENEMY TURN", 1);

    //    //do the ai logic for each enemy
    //    StartCoroutine(EnemyAiAct());
    //}

    //public IEnumerator EnemyAiAct()
    //{

    //    //get how many enemies will act
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    //    foreach (GameObject enemy in enemies)
    //    {
    //        AIBrain aIBrain = enemy.GetComponent<AIBrain>();

    //        //if no ai brain the get continue to the next or deasd
    //        if (aIBrain == null || enemy.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
    //        {
    //            continue;
    //        }

    //        ScriptableCard scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep].scriptableCard;
    //        float totalAbilitiesWaitTime = 0;

    //        //go throught every ability and calculate the waitTime
    //        foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
    //        {
    //            totalAbilitiesWaitTime += scriptableCardAbility.GetFullAbilityWaitingTime(enemy);
    //        }

    //        //execute ai
    //        aIBrain.ExecuteCommand();

    //        //loop between them and execute the command
    //        yield return new WaitForSeconds(totalAbilitiesWaitTime);
    //    }


    //}

    //public void EnemyTurnEnd()
    //{
    //    SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyEndTurn;
    //    UI_Combat.Instance.OnNotification("ENEMY ENDING TURN", 1);

    //    //loop for all buffs and debuffs
    //    BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

    //}

    //IEnumerator WaitEnemyTurns()
    //{

    //    //player turn start
    //    PlayerTurnEnd();
    //    yield return new WaitForSeconds(2f);

    //    //player turn
    //    EnemyTurnStart();
    //    yield return new WaitForSeconds(2f);

    //    EnemyTurn();
    //    yield return new WaitForSeconds(2f);

    //    EnemyTurnEnd();
    //    yield return new WaitForSeconds(2f);

    //    PlayerTurnStart();
    //    yield return new WaitForSeconds(2f);

    //    PlayerTurn();
    //    yield return new WaitForSeconds(2f);

    //}

    //public GameObject InstantiateCharacter(ScriptableEntity scriptableEntity, int spawnPosition)
    //{

    //    //instantiate the character prefab based on the spawnPosition
    //    GameObject character = Instantiate(scriptableEntity.entityPrefab, characterSpawns[spawnPosition].transform.position, Quaternion.identity);

    //    //pass it the scriptable object to the class
    //    character.GetComponent<EntityClass>().scriptableEntity = scriptableEntity;
    //    //svae the position as it can be used later
    //    character.GetComponent<EntityClass>().originalCombatPos = characterSpawns[spawnPosition].transform;

    //    //parent it to our characters object
    //    character.transform.SetParent(CombatManager.Instance.combatScene.transform.Find("Characters"));

    //    return character;

    //}

    //public void RemoveShieldFromEntitiesBasedOnTag(string tag)
    //{

    //    GameObject[] characters = GameObject.FindGameObjectsWithTag(tag);

    //    foreach (GameObject character in characters)
    //    {
    //        RemoveShieldFromEntity(character);
    //    }

    //}

    //public GameObject[] GetAllCharactersGameObjects()
    //{
    //    GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

    //    return characters;
    //}

    //public void RemoveShieldFromEntity(GameObject entity)
    //{

    //    EntityClass entityClass = entity.GetComponent<EntityClass>();

    //    //characterClass.InititializeCharacter();

    //    entityClass.shield = 0;

    //    //dont show the icon
    //    entityClass.shieldIcon.SetActive(false);

    //    //update text on shield
    //    entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();

    //    //make the bar red
    //    entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

    //}

    //public GameObject GetTheCharacterThatUsesTheCard(CardScript cardScript)
    //{

    //    GameObject character = null;

    //    //check if the card belongs to any of our characters
    //    foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
    //    {

    //        if (characterInCombat.GetComponent<EntityClass>().scriptableEntity.mainClass == cardScript.scriptableCard.mainClass 
    //            && characterInCombat.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
    //        {

    //            //then it belongs to a character we have
    //            character = characterInCombat;
    //            break;
    //        }

    //    }

    //    ////in case the type of the card does not belong to the card then
    //    //if (character == null)
    //    //{
    //    //    //then assign the card to the leader
    //    //    character = CombatManager.Instance.leaderCharacter;
    //    //}

    //    //return the appropriate character
    //    return character;

    //}
}
