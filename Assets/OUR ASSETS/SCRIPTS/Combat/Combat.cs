using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    public static Combat Instance;


    [Header("SPAWNS")]
    public GameObject characterStartSpawn;
    public GameObject enemyStartSpawn;
    public float spawnCharacterDistance = 0f;
    public float spawnEnemyDistance = 0f;
    public float spawnCharacterDistanceVariance = 0f;
    public float spawnEnemyDistanceVariance = 0f;

    [Header("COMBAT COMMON VARIABLES")]
    public int turns = 0;

    [Header("LEADER MECHANIC")]
    public GameObject leaderCharacter;

    [Header("CONDITIONS")]
    public int charactersAlive = 0;
    public int enemiesAlive = 0;






    public int tempBoostAttack = 0;


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

    }

    // Start is called before the first frame update
    void Start()
    {
       
        //at the start of combat
        StartCombat();

    }




    public IEnumerator InitializeCombat()
    {

        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

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

        //initialize all things card related, deck, combat deck, discard pile, etc
        InitializeCardsAndDecks();

        //asign leader the first character
        AssignLeader(CharacterManager.Instance.charactersInAdventure[0]);

        //check the characters that are alive
        CheckCharactersAlive();

        //check the enemies that are alive
        CheckEnemiesAlive();

        //start the player
        yield return WaitPlayerTurns();
    }


    public IEnumerator WaitEnemyTurns()
    {
        yield return PlayerTurnEnd();
        yield return EnemyTurnStart();
        yield return EnemyTurn();
        yield return EnemyTurnEnd();
        yield return PlayerTurnStart();
        yield return PlayerTurn();


    }

    public IEnumerator PlayerTurnStart()
    {
        //start player turn
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

        //increase turn;
        turns += 1;

        //mana should go back to full
        RefillMana();

        //remove mana
        RemoveShieldFromEntitiesBasedOnTag("Player");

        //draw cards
        DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit);

        UI_Combat.Instance.OnNotification("PLAYER TURN", 1);

        //check if its not turn 1 and also only in duo mode


        //if there is only 1 character then assign it to it
        if (charactersAlive == 1)
        {
            AssignLeaderToAlive();
        }
        else
        {
            if (turns != 1)
            {
                ReverseLeader();
            }
        }

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        //generate intend for all enemies
        GenerateEnemyIntends();

        yield return null; // Wait for a frame 
    }

    public IEnumerator PlayerTurn()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerTurn;
     

        yield return null; //wait for frame

    }

    public IEnumerator PlayerTurnEnd()
    {

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerEndTurn;
   
        //discard cards
        DeckManager.Instance.DiscardWholeHand();

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        yield return null; //skip frame

    }

    public IEnumerator EnemyTurnStart()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
        UI_Combat.Instance.OnNotification("ENEMY TURN", 1);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        //remove shield from all enemies
        RemoveShieldFromEntitiesBasedOnTag("Enemy");

        yield return null;
    }

    public IEnumerator EnemyTurn()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyTurn;

        //do the ai logic for each enemy
        yield return EnemyAiAct();

        yield return null;
    }

    public IEnumerator EnemyTurnEnd()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyEndTurn;

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.ActivateAllBuffsDebuffs();

        yield return null;
    }

    public IEnumerator SpawnCharacters()
    {
        int positionSpawn = 0;
        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in CharacterManager.Instance.scriptablePlayerList)
        {
            //instantiate our character or characters
            GameObject characterInCombat = InstantiateCharacter(scriptableEntity, positionSpawn);

            //assign the characters in combat
            CharacterManager.Instance.charactersInAdventure.Add(characterInCombat);

            //initialize the stats
            characterInCombat.GetComponent<EntityClass>().InititializeEntity();

            //increase to the next position
            positionSpawn++;

            yield return null; // Wait for a frame 
        }

    }

    public IEnumerator SpawnEnemies()
    {
        int positionSpawn = 0;
        //generate the selected characters that will be used throught the game
        foreach (ScriptableEntity scriptableEntity in CombatManager.Instance.scriptableEnemyList)
        {
            //instantiate our character or characters
            GameObject enemyInCombat = InstantiateCharacter(scriptableEntity, positionSpawn);

            //assign the characters in combat
            CharacterManager.Instance.charactersInAdventure.Add(enemyInCombat);

            //initialize the stats
            enemyInCombat.GetComponent<EntityClass>().InititializeEntity();

            //increase to the next position
            positionSpawn++;

            yield return null; // Wait for a frame 
        }

    }

    public GameObject InstantiateCharacter(ScriptableEntity scriptableEntity, int spawnPosition)
    {

       
        Vector3 spawn = new Vector3(0,0,0);

        if (scriptableEntity.mainClass == SystemManager.MainClass.Enemy)
        {
       
            spawn = new Vector3( enemyStartSpawn.transform.position.x - spawnCharacterDistance - spawnEnemyDistanceVariance, enemyStartSpawn.transform.position.y, enemyStartSpawn.transform.position.z);
            spawnCharacterDistance += 2;
        }
        else
        {
   
            spawn = new Vector3(characterStartSpawn.transform.position.x + spawnEnemyDistance + spawnCharacterDistanceVariance, characterStartSpawn.transform.position.y, characterStartSpawn.transform.position.z);
            spawnEnemyDistance += 2;
        }


        //instantiate the character prefab based on the spawnPosition
        GameObject entity = Instantiate(scriptableEntity.entityPrefab, spawn, Quaternion.identity);

        //pass it the scriptable object to the class
        entity.GetComponent<EntityClass>().scriptableEntity = scriptableEntity;
        //svae the position as it can be used later
        entity.GetComponent<EntityClass>().originalCombatPos = spawn;

        //parent it to our characters object
        if (scriptableEntity.mainClass == SystemManager.MainClass.Enemy)
        {
            entity.transform.SetParent(this.gameObject.transform.Find("Enemies"));
        }
        else
        {
            entity.transform.SetParent(this.gameObject.transform.Find("Characters"));
        }


        return entity;

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

    public void InitializeCardsAndDecks()
    {

        //if deck is not created then create it / this should only be available when not having deck
        if (DeckManager.Instance.mainDeck.Count == 0)
        {
            DeckManager.Instance.BuildStartingDeck();
        }

        //initialize the combat deck
        DeckManager.Instance.combatDeck.Clear();
        DeckManager.Instance.combatDeck = new List<CardScript>(DeckManager.Instance.mainDeck);
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);

        //clean up discard pile
        DeckManager.Instance.discardedPile.Clear();

        //clean up banished pile
        DeckManager.Instance.banishedPile.Clear();
    }

    public void RefillMana()
    {
        //initialize mana and UI
        UI_Combat.Instance.ManaAvailable = CombatManager.Instance.manaMaxAvailable;
    }

    public void AssignLeader(GameObject characterInCombat)
    {
        //assign the leader
        leaderCharacter = characterInCombat;

        //position the ui indicator on the leader
        UI_Combat.Instance.leaderIndicator.SetActive(true);
        UI_Combat.Instance.leaderIndicator.transform.position = new Vector2(leaderCharacter.transform.position.x, leaderCharacter.GetComponent<EntityClass>().scriptableEntity.leaderIndicatorHeight);

        //parent it
        UI_Combat.Instance.leaderIndicator.transform.SetParent(leaderCharacter.transform);

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

    public void AssignLeaderToAlive()
    {
        foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
        {

            if (characterInCombat.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {
                AssignLeader(characterInCombat);
                break;
            }

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

    public void ReverseLeader()
    {
        foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
        {

            if (leaderCharacter != characterInCombat)
            {
                AssignLeader(characterInCombat);
                break;
            }

        }
    }

    public void GenerateEnemyIntends()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> enemyList = enemies.ToList();

        foreach (GameObject enemy in enemyList)
        {

            //destroy intends
            SystemManager.Instance.DestroyAllChildren(enemy.transform.Find("gameobjectUI").Find("intendList").Find("intends").gameObject);

            //generate new intends
            enemy.GetComponent<AIBrain>().GenerateIntend();
        }

    }



    public void AdjustTargetHealth(GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {

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

        if (entityClass.health <= 0)
        {

            entityClass.entityMode = SystemManager.EntityMode.DEAD;
            Animator animator = entityClass.transform.Find("model").GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Dead");
            }

            EntityDeadDestroy(entityClass.gameObject);

            if (entityClass.gameObject.tag == "Player")
            {
                charactersAlive -= 1;

                //need to regenerate intend 
                //generate intend for all enemies
                GenerateEnemyIntends();
            }
            else
            {
                enemiesAlive -= 1;
            }

        }

    }

    public void EntityDeadDestroy(GameObject entity)
    {
        GameObject buffsdebuffs = entity.transform.Find("gameobjectUI").Find("BuffDebuffList").Find("Panel").gameObject;
        GameObject intends = null;
        if (entity.tag == "Enemy")
        {
            intends = entity.transform.Find("gameobjectUI").Find("intendList").Find("intends").gameObject;
        }

        if (intends != null)
        {
            SystemManager.Instance.DestroyAllChildren(intends);
        }

        if (buffsdebuffs != null)
        {
            SystemManager.Instance.DestroyAllChildren(buffsdebuffs);
        }

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





    public IEnumerator EnemyAiAct()
    {

        //get how many enemies will act
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            AIBrain aIBrain = enemy.GetComponent<AIBrain>();

            //if no ai brain the get continue to the next or deasd
            if (aIBrain == null || enemy.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
            {
                continue;
            }

            ScriptableCard scriptableCard = aIBrain.cardScriptList[aIBrain.aiLogicStep].scriptableCard;
            float totalAbilitiesWaitTime = 0;

            //go throught every ability and calculate the waitTime
            foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
            {
                totalAbilitiesWaitTime += scriptableCardAbility.GetFullAbilityWaitingTime(enemy);
            }

            //execute ai
            aIBrain.ExecuteCommand();

            //loop between them and execute the command
            yield return new WaitForSeconds(totalAbilitiesWaitTime);
        }


    }







    public GameObject GetTheCharacterThatUsesTheCard(CardScript cardScript)
    {

        GameObject character = null;

        //check if the card belongs to any of our characters
        foreach (GameObject characterInCombat in CharacterManager.Instance.charactersInAdventure)
        {

            if (characterInCombat.GetComponent<EntityClass>().scriptableEntity.mainClass == cardScript.scriptableCard.mainClass
                && characterInCombat.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
            {

                //then it belongs to a character we have
                character = characterInCombat;
                break;
            }

        }

        //in case the type of the card does not belong to the card then
        if (character == null)
        {
            //then assign the card to the leader
            character = leaderCharacter;
        }

        //return the appropriate character
        return character;

    }
}
