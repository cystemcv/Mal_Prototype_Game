using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScriptableCard;

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
    public int maxEnemySummons = 3;

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

    public LTDescr moveEntityTween;


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



    }


    public void Update()
    {

        if (conditionsEnabled == true &&
            (charactersAlive <= 0 || enemiesAlive <= 0))
        {
            CombatOver();
        }

    }



    public void CombatOver()
    {

        //always check characters first, if both lost = game over
        if (charactersAlive <= 0)
        {
            //lose
            UI_Combat.Instance.gameover.SetActive(true);
        }
        else if (enemiesAlive <= 0)
        {
            //win
            UI_Combat.Instance.victory.SetActive(true);
        }

    }


    public IEnumerator InitializeCombat()
    {

        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

        //Debug.Log("Test dsck"  + DeckManager.Instance.scriptableDeck.mainDeck[0].scriptableCard.cardName);

        //reset turns
        turns = 0;

        //play music
        AudioManager.Instance.PlayMusic("Combat_1");



        //destroy any previous characters
        yield return SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Characters").gameObject);

        //destroy any previous enemies
        yield return SystemManager.Instance.DestroyAllChildrenIE(this.gameObject.transform.Find("Enemies").gameObject);

        //spawn the characters
        yield return SpawnCharacters();

        //spawn the enemies
        yield return SpawnEnemies();

        yield return new WaitForSeconds(0.5f);

        //build the deck from Scriptable Object deck
        yield return CopyDeckFromSO();

        //initialize all things card related, deck, combat deck, discard pile, etc
        yield return InitializeCardsAndDecks();


        //check the characters that are alive
        CheckCharactersAlive();

        //check the enemies that are alive
        CheckEnemiesAlive();

        //start win conditions
        conditionsEnabled = true;

        //start the player
        yield return WaitPlayerTurns();
    }


    public IEnumerator WaitEnemyTurns()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return PlayerTurnEnd();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return EnemyTurnStart();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return EnemyTurn();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return EnemyTurnEnd();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return PlayerTurnStart();

        if (combatEnded)
        {
            yield return null; //stops function
        }

        yield return PlayerTurn();

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

        //start player turn
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

        //increase turn;
        turns += 1;

        yield return RearrangeFormation(characterFormation);

        //mana should go back to full
        RefillMana();

        //remove mana
        RemoveShieldFromEntitiesBasedOnTag("Player");

        //draw cards
        Debug.Log("HandManager.Instance.turnHandCardsLimit : " + HandManager.Instance.turnHandCardsLimit);
        DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit);

        UI_Combat.Instance.OnNotification("PLAYER TURN", 1);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        //generate intend for all players
        AIManager.Instance.GenerateIntends(SystemManager.Instance.GetPlayerTagsList());
        AIManager.Instance.GenerateIntends(SystemManager.Instance.GetEnemyTagsList());

        //do the ai logic for each enemy
        yield return StartCoroutine(AIManager.Instance.AiAct(SystemManager.Instance.GetPlayerTagsList()));

        yield return StartCoroutine(LowerSummonTurns(SystemManager.Instance.GetPlayerTagsList()));

        yield return null; // Wait for a frame 
    }

    public IEnumerator LowerSummonTurns(List<string> tags)
    {
        List<GameObject> entities = SystemManager.Instance.FindGameObjectsWithTags(tags);

        foreach (GameObject entity in entities)
        {
            if (entity.tag == "PlayerSummon" || entity.tag == "EnemySummon")
            {
                EntityClass entityClass = entity.GetComponent<EntityClass>();
                //lower every turn by 1
                entityClass.AdjustSummonTurns(-1);
            }

        }

        yield return null;
    }

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



        yield return new WaitForSeconds(1f);

        yield return null; //skip frame

    }

    public IEnumerator EnemyTurnStart()
    {

        if (combatEnded)
        {
            yield return null; //stops function
        }



        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
        UI_Combat.Instance.OnNotification("ENEMY TURN", 1);

        yield return RearrangeFormation(enemyFormation);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        //remove shield from all enemies
        RemoveShieldFromEntitiesBasedOnTag("Enemy");

        yield return StartCoroutine(LowerSummonTurns(SystemManager.Instance.GetEnemyTagsList()));

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

        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in CharacterManager.Instance.scriptablePlayerList)
        {
            //instantiate our character or characters
            GameObject characterInCombat = InstantiateCharacter(scriptableEntity);

            //assign the characters in combat
            CharacterManager.Instance.charactersInAdventure.Add(characterInCombat);

            //initialize the stats
            StartCoroutine(characterInCombat.GetComponent<EntityClass>().InititializeEntity());

            yield return null; // Wait for a frame 
        }

    }

    public IEnumerator SpawnEnemies()
    {

        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in AIManager.Instance.scriptableEnemyList)
        {
            //instantiate our character or characters
            GameObject enemyInCombat = InstantiateEnemies(scriptableEntity);

            //assign the characters in combat
            CharacterManager.Instance.charactersInAdventure.Add(enemyInCombat);

            //initialize the stats
            StartCoroutine(enemyInCombat.GetComponent<EntityClass>().InititializeEntity());


            yield return null; // Wait for a frame 
        }

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
        spriteObject.transform.Find("model").localScale= scale;
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

    public void CheckCharactersAlive()
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


    }

    public void CheckEnemiesAlive()
    {
        //initialize dead count
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        enemiesAlive = 0;
        foreach (GameObject enemy in enemies)
        {
            //count how many alive
            if (enemy.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {
                enemiesAlive += 1;
            }
        }
    }

    public IEnumerator InitializeCardsAndDecks()
    {



        //initialize the combat deck
        DeckManager.Instance.combatDeck.Clear();
        DeckManager.Instance.combatDeck = new List<CardScript>(DeckManager.Instance.mainDeck);
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);

        //clean up discard pile
        DeckManager.Instance.discardedPile.Clear();

        //clean up banished pile
        DeckManager.Instance.banishedPile.Clear();

        yield return null; // Wait for a frame 
    }

    public IEnumerator CopyDeckFromSO()
    {

        //if deck is not created then create it / this should only be available when not having deck
        //if (DeckManager.Instance.mainDeck.Count == 0)
        //{
        DeckManager.Instance.BuildStartingDeckSO();
        //}

        //clear local deck
        DeckManager.Instance.mainDeck.Clear();

        //copy the deck
        DeckManager.Instance.mainDeck = new List<CardScript>(DeckManager.Instance.scriptableDeck.mainDeck);
        //mainDeck = scriptableDeck.mainDeck;

        yield return null; // Wait for a frame 
    }

    public void RefillMana()
    {
        //initialize mana and UI
        UI_Combat.Instance.ManaAvailable = CombatManager.Instance.manaMaxAvailable;
    }



    public IEnumerator WaitPlayerTurns()
    {
        //yield return new WaitForSeconds(2f);
        //player turn start
        yield return PlayerTurnStart();
        //yield return new WaitForSeconds(2f);
        //player turn
        yield return PlayerTurn();

    }



    public void RemoveShieldFromEntitiesBasedOnTag(string tag)
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject character in characters)
        {
            RemoveShieldFromEntity(character);
        }

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

        //make the bar red
        entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

    }







    public void AdjustTargetHealth(GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
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

            int remainingShield = 0;
            //check if there is a shield
            if (entityClass.shield > 0 && bypassShield == false)
            {

                //then do dmg to shield
                remainingShield = entityClass.shield - adjustNumber;

                //update the ui
                if (remainingShield > 0)
                {
                    //update enemy script
                    entityClass.shield = remainingShield;

                    //update text on shield
                    entityClass.shieldText.GetComponent<TMP_Text>().text = entityClass.shield.ToString();

                    //make the bar blue
                    entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);

                }
                else
                {
                    //then the enemy has no shield left
                    entityClass.shield = 0;

                    //make the bar red
                    entityClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

                    //hide the icon
                    entityClass.shieldIcon.SetActive(false);
                }

            }
            else
            {
                //if there is no shield go straight to the health hp
                remainingShield = adjustNumber * -1;
            }

            if (remainingShield < 0)
            {

                //then we subtract from the health the remaining amount
                entityClass.health += remainingShield;

                if (entityClass.health < 0)
                {
                    entityClass.health = 0;
                }

                //update text on hp
                entityClass.healthText.GetComponent<TMP_Text>().text = entityClass.health + " / " + entityClass.maxHealth;

                //adjust the hp bar
                UI_Combat.Instance.UpdateHealthBarSmoothly(entityClass.health, entityClass.maxHealth, entityClass.slider);

                //flash enemy when directly hit
                StartCoroutine(FlashEntityAfterHit(entityClass.gameObject));

            }

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
            entityClass.healthText.GetComponent<TMP_Text>().text = entityClass.health + " / " + entityClass.maxHealth;

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

        //spawn numberOn screen
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject numberOnScreenPrefab = Instantiate(UI_Combat.Instance.numberOnScreen, target.transform.position, Quaternion.identity);
        numberOnScreenPrefab.transform.SetParent(target.transform);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().color = AdjustNumberModeColor(adjustNumberMode);

        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().text = adjustNumber.ToString();

        Destroy(numberOnScreenPrefab, 1f);

        //if the target reach to 0
        CheckIfEntityIsDead(entityClass);


    }



    public void CheckIfEntityIsDead(EntityClass entityClass)
    {

        if (entityClass.health <= 0 || entityClass.summonTurns <= 0)
        {

            entityClass.entityMode = SystemManager.EntityMode.DEAD;
            Animator animator = entityClass.transform.Find("model").GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Dead");
            }

            if (entityClass.gameObject.tag == "Player")
            {
                charactersAlive -= 1;
            }
            else if (entityClass.gameObject.tag == "Enemy")
            {
                enemiesAlive -= 1;
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
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed); ;
        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
        {
            colorToChange = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue); ;
        }

        return colorToChange;
    }


    public int CalculateEntityDmg(int startingDmg, GameObject entity, GameObject target)
    {

        if (combatEnded || entity == null)
        {
            return 0; //stops function
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
        yield return new WaitForSeconds(0.2f);
        SystemManager.Instance.ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity, target);
    }










}
