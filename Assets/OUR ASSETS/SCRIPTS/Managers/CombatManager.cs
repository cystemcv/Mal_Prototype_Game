using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    //targeting system


    public GameObject particleCardSoul;
    public GameObject arrowHead;

    public RectTransform targetUIElement;
    public LineRenderer lineRenderer;
    public GameObject targetClicked;

    //our combat scene
    public GameObject combatScene;

    public List<GameObject> characterSpawns;


    public int turns = 0;



    //---------------

    //damage system
    public GameObject numberOnScreen;

    //---------------

    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;

    //ui healthbar colors
    private float fillbarVelocity = 0;
    private float fillbarSmoothValue = 0.3f;



    //leader
    public GameObject leaderCharacter;
    public GameObject leaderIndicator;

    //characters
    public List<GameObject> charactersInCombat;

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

    public delegate void OnManaChange();
    public event OnManaChange onManaChange;

    private void OnEnable()
    {
        onManaChange += ManaDetectEvent;
    }

    private void OnDisable()
    {
        onManaChange -= ManaDetectEvent;
    }

    public void ManaDetectEvent()
    {

        //show the mana on UI
        UIManager.Instance.manaText.GetComponent<TMP_Text>().text = manaAvailable.ToString();

        //go throught each card in the hand and update them
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            UpdateCardAfterManaChange(cardPrefab);
        }

    }

    public void Start()
    {
        lineRenderer = this.transform.Find("TARGET LINERENDERER").GetComponent<LineRenderer>();

        // Set the initial positions of the line (start and end)
        lineRenderer.positionCount = 2;

        // Get the material of the Line Renderer
        Material material = lineRenderer.material;

        // Set the sorting order to a high value to ensure it renders on top
        material.renderQueue = 999999; // Adjust the value as needed
    }

    public void Update()
    {

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.TARGET)
        {

            // Check if the target UI element is available
            if (targetUIElement != null)
            {
                // Get the position of the target UI element in screen space
                //Vector3 targetScreenPosition = targetUIElement.Find("LineRendererStart").position;
                Vector3 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(SystemManager.Instance.uiCamera, targetUIElement.Find("LineRendererStart").position);


                // Convert the screen space position to world space
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(targetScreenPosition);
                targetPosition.z = 0f; // Ensure that the z-coordinate is set to 0

                // Get the position of the mouse in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f; // Ensure that the z-coordinate is set to 0

                // Calculate the control point for the Bezier curve
                Vector3 controlPoint = (targetPosition + mousePosition) / 2;
                controlPoint.y += Mathf.Abs(mousePosition.x - targetPosition.x) / 2; // Adjust the curve height


                // Generate the points for the Bezier curve
                int segmentCount = 20;
                Vector3[] curvePoints = new Vector3[segmentCount + 1];
                for (int i = 0; i <= segmentCount; i++)
                {
                    float t = i / (float)segmentCount;
                    curvePoints[i] = CalculateQuadraticBezierPoint(t, targetPosition, controlPoint, mousePosition);
                }

                // Set the positions for the line renderer
                lineRenderer.positionCount = curvePoints.Length;
                lineRenderer.SetPositions(curvePoints);

                // Position the arrowhead at the end of the curve
                arrowHead.transform.position = curvePoints[curvePoints.Length - 1];
                // Rotate the arrowhead to face the direction of the line
                Vector3 direction = curvePoints[curvePoints.Length - 1] - curvePoints[curvePoints.Length - 2];
                arrowHead.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);


                //remove line renderer
                lineRenderer.gameObject.SetActive(true);
                arrowHead.SetActive(true);

                // Update the positions of the line renderer
                //lineRenderer.SetPosition(0, targetPosition);
                //lineRenderer.SetPosition(1, mousePosition);
            }
            CheckHitTarget();
            CheckClickTarget();
        }

    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }

    public void CheckClickTarget()
    {

        if (Input.GetMouseButtonDown(0))
        {
            HitTarget();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HitTarget();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //remove line renderer
            lineRenderer.gameObject.SetActive(false);
            arrowHead.SetActive(false);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;
        }
    }

    public void HitTarget()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one
            if (hit.collider.gameObject.tag != "Enemy" && hit.collider.gameObject.tag != "Player")
            {
                return;
            }

            string cardTag = "";
            if (targetUIElement.gameObject.GetComponent<CardScript>().scriptableCard.targetEnemy == true)
            {
                cardTag = "Enemy";
            }
            else
            {
                cardTag = "Player";
            }

            if (hit.collider.gameObject.tag != cardTag)
            {
                return;
            }

            // Handle the click
            //Debug.Log("Mouse clicked on: " + hit.collider.gameObject.name);
            // Add your click handling code here
            targetClicked = hit.collider.gameObject;

            //do the effects 
            DeckManager.Instance.PlayCard(targetUIElement.gameObject.GetComponent<CardScript>());
            //return everything where it was
            HandManager.Instance.SetHandCards();

            //remove line renderer
            lineRenderer.gameObject.SetActive(false);
            arrowHead.SetActive(false);

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;


        }
    }

    public void CheckHitTarget()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        string cardTag = "";
        if (targetUIElement.gameObject.GetComponent<CardScript>().scriptableCard.targetEnemy == true)
        {
            cardTag = "Enemy";
        }
        else
        {
            cardTag = "Player";
        }

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one
            if (hit.collider.gameObject.tag == "Enemy" && cardTag == "Enemy")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue));

            }
            else if (hit.collider.gameObject.tag == "Player" && cardTag == "Player")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue));

            }
            else
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));
            }

        }
        else
        {
            if (cardTag == "Player")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorGreen));
            }
            else if (cardTag == "Enemy")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed));
            }
            else
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));
            }
        }

    }

    public void ChangeLineAndArrowColor(Color color)
    {
        arrowHead.GetComponent<SpriteRenderer>().color = color;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void AdjustHealth(GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {


        //update ui

        //update based on type

        if (target.tag == "Enemy")
        {
            AdjustHealthEnemy(target, adjustNumber, bypassShield, adjustNumberMode);
        }
        else if (target.tag == "Player")
        {
            AdjustHealthCharacter(target, adjustNumber, bypassShield, adjustNumberMode);
        }
        else
        {
            //we do nothing
        }

    }

    public void AdjustHealthEnemy(GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {

        EnemyClass enemyClass = target.GetComponent<EnemyClass>();

        if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
        {

            int remainingShield = 0;
            //check if there is a shield
            if (enemyClass.shield > 0 && bypassShield == false)
            {

                //then do dmg to shield
                remainingShield = enemyClass.shield - adjustNumber;

                //update the ui
                if (remainingShield > 0)
                {
                    //update enemy script
                    enemyClass.shield = remainingShield;

                    //update text on shield
                    enemyClass.shieldText.GetComponent<TMP_Text>().text = enemyClass.shield.ToString();

                    //make the bar blue
                    enemyClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);

                }
                else
                {
                    //then the enemy has no shield left
                    enemyClass.shield = 0;

                    //make the bar red
                    enemyClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

                    //hide the icon
                    enemyClass.shieldIcon.SetActive(false);
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
                enemyClass.health += remainingShield;

                if (enemyClass.health < 0)
                {
                    enemyClass.health = 0;
                }

                //update text on hp
                enemyClass.healthText.GetComponent<TMP_Text>().text = enemyClass.health + " / " + enemyClass.maxHealth;

                //adjust the hp bar
                UpdateHealthBarSmoothly(enemyClass.health, enemyClass.maxHealth, enemyClass.slider);

            }

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.HEAL)
        {
            //increase the hp

            enemyClass.health = enemyClass.health + adjustNumber;

            //check if max from hp
            if (enemyClass.health > enemyClass.maxHealth)
            {
                enemyClass.health = enemyClass.maxHealth;
            }

            //update text on hp
            enemyClass.healthText.GetComponent<TMP_Text>().text = enemyClass.health + " / " + enemyClass.maxHealth;

            //adjust the hp bar
            UpdateHealthBarSmoothly(enemyClass.health, enemyClass.maxHealth, enemyClass.slider);

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
        {
            //increase the shield

            enemyClass.shield = enemyClass.shield + adjustNumber;

            //check if max from hp
            if (enemyClass.shield > enemyClass.maxShield)
            {
                enemyClass.shield = enemyClass.maxShield;
            }

            if (enemyClass.shield > 0)
            {
                //show the icon
                enemyClass.shieldIcon.SetActive(true);

                //update text on shield
                enemyClass.shieldText.GetComponent<TMP_Text>().text = enemyClass.shield.ToString();

                //make the bar blue
                enemyClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
            }

        }

        //spawn numberOn screen
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject numberOnScreenPrefab = Instantiate(numberOnScreen, target.transform.position, Quaternion.identity);
        numberOnScreenPrefab.transform.SetParent(target.transform);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().color = AdjustNumberModeColor(adjustNumberMode);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().text = adjustNumber.ToString();

        Destroy(numberOnScreenPrefab, 1f);

        //if an enemy reach 0 hp then it should be destroyed

        if (enemyClass.health <= 0)
        {
            Destroy(target, 1f);
        }


    }

    public void AdjustHealthCharacter(GameObject target, int adjustNumber, bool bypassShield, SystemManager.AdjustNumberModes adjustNumberMode)
    {

        CharacterClass characterClass = target.GetComponent<CharacterClass>();

        if (adjustNumberMode == SystemManager.AdjustNumberModes.ATTACK)
        {

            int remainingShield = 0;
            //check if there is a shield
            if (characterClass.shield > 0 && bypassShield == false)
            {

                //then do dmg to shield
                remainingShield = characterClass.shield - adjustNumber;

                //update the ui
                if (remainingShield > 0)
                {
                    //update enemy script
                    characterClass.shield = remainingShield;

                    //update text on shield
                    characterClass.shieldText.GetComponent<TMP_Text>().text = characterClass.shield.ToString();

                    //make the bar blue
                    characterClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);

                }
                else
                {
                    //then the enemy has no shield left
                    characterClass.shield = 0;

                    //make the bar red
                    characterClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

                    //hide the icon
                    characterClass.shieldIcon.SetActive(false);
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
                characterClass.health += remainingShield;

                if (characterClass.health < 0)
                {
                    characterClass.health = 0;
                }

                //update text on hp
                characterClass.healthText.GetComponent<TMP_Text>().text = characterClass.health + " / " + characterClass.maxHealth;

                //adjust the hp bar
                UpdateHealthBarSmoothly(characterClass.health, characterClass.maxHealth, characterClass.slider);

            }

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.HEAL)
        {
            //increase the hp

            characterClass.health = characterClass.health + adjustNumber;

            //check if max from hp
            if (characterClass.health > characterClass.maxHealth)
            {
                characterClass.health = characterClass.maxHealth;
            }

            //update text on hp
            characterClass.healthText.GetComponent<TMP_Text>().text = characterClass.health + " / " + characterClass.maxHealth;

            //adjust the hp bar
            UpdateHealthBarSmoothly(characterClass.health, characterClass.maxHealth, characterClass.slider);

        }
        else if (adjustNumberMode == SystemManager.AdjustNumberModes.SHIELD)
        {
            //increase the shield

            characterClass.shield = characterClass.shield + adjustNumber;

            //check if max from hp
            if (characterClass.shield > characterClass.maxShield)
            {
                characterClass.shield = characterClass.maxShield;
            }

            if (characterClass.shield > 0)
            {
                //show the icon
                characterClass.shieldIcon.SetActive(true);

                //update text on shield
                characterClass.shieldText.GetComponent<TMP_Text>().text = characterClass.shield.ToString();

                //make the bar blue
                characterClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue);
            }

        }

        //spawn numberOn screen
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject numberOnScreenPrefab = Instantiate(numberOnScreen, target.transform.position, Quaternion.identity);
        numberOnScreenPrefab.transform.SetParent(target.transform);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().color = AdjustNumberModeColor(adjustNumberMode);

        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().text = adjustNumber.ToString();

        Destroy(numberOnScreenPrefab, 1f);

        //if an enemy reach 0 hp then it should be destroyed

        //if (enemyClass.health <= 0)
        //{
        //    Destroy(target, 1f);
        //}


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

    public void UpdateHealthBarSmoothly(float health, float maxHealth, Slider slider)
    {
        StartCoroutine(SmoothUpdateHealthBar(health, maxHealth, slider));
    }

    private IEnumerator SmoothUpdateHealthBar(float health, float maxHealth, Slider slider)
    {
        float targetValue = health / maxHealth;
        while (Mathf.Abs(slider.value - targetValue) > 0.01f)
        {
            slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref fillbarVelocity, fillbarSmoothValue);
            yield return null; // Wait for the next frame
        }
        slider.value = targetValue; // Ensure it's set to the target value at the end
    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (CombatManager.Instance.manaAvailable < cardScript.primaryManaCost)
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

    public void RefillMana()
    {
        //initialize mana and UI
        ManaAvailable = manaMaxAvailable;
    }

    public void StartCombat()
    {


        //change into combat mode
        SystemManager.Instance.systemMode = SystemManager.SystemModes.COMBAT;

        //close the UI window
        UIManager.Instance.UIMENU.SetActive(false);
        UIManager.Instance.bgCanvas.SetActive(false);
        UIManager.Instance.uiParticles.SetActive(false);

        //reset turns
        turns = 0;

        //open the UI combat menu
        UIManager.Instance.UICOMBAT.SetActive(true);

        //play music
        AudioManager.Instance.PlayMusic("Combat_1");

        //give mana to player
        RefillMana();

        //initialize the combat deck
        DeckManager.Instance.combatDeck.Clear();
        DeckManager.Instance.combatDeck = new List<CardScript>(DeckManager.Instance.mainDeck);
        DeckManager.Instance.ShuffleDeck(DeckManager.Instance.combatDeck);

        //clean up discard pile
        DeckManager.Instance.discardedPile.Clear();

        //clean up banished pile
        DeckManager.Instance.banishedPile.Clear();

        //asign leader the first character
        AssignLeader(charactersInCombat[0]);


        StartCoroutine(WaitPlayerTurns());



        //initialize the characters

        //initialize the enemies

        //we draw for each character




    }

    public void AssignLeader(GameObject characterInCombat)
    {
        //assign the leader
        leaderCharacter = characterInCombat;

        //position the ui indicator on the leader
        leaderIndicator.SetActive(true);
        leaderIndicator.transform.position = new Vector2(leaderCharacter.transform.position.x, leaderCharacter.GetComponent<CharacterClass>().scriptablePlayer.leaderIndicatorHeight);
    }

    public void ReverseLeader()
    {
        foreach (GameObject characterInCombat in charactersInCombat)
        {

            if (leaderCharacter != characterInCombat)
            {
                AssignLeader(characterInCombat);
                break;
            }

        }
    }


    public void PlayerTurnStart()
    {
        //start player turn
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerStartTurn;

        //increase turn;
        turns += 1;

        //mana should go back to full
        RefillMana();

        //remove mana
        RemoveShieldFromAllCharacters();

        //draw cards
        DeckManager.Instance.DrawMultipleCards(HandManager.Instance.turnHandCardsLimit);

        UIManager.Instance.OnNotification("PLAYER STARTING TURN", 1);

        //check if its not turn 1 and also only in duo mode
        if (turns != 1)
        {
            ReverseLeader();
        }
    }

    public void PlayerTurn()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerTurn;
        UIManager.Instance.OnNotification("PLAYER TURN", 1);

    }

    IEnumerator WaitPlayerTurns()
    {

        //player turn start
        PlayerTurnStart();
        yield return new WaitForSeconds(2f);

        //player turn
        PlayerTurn();

    }

    public void PlayerEndTurnButton()
    {
        if (SystemManager.Instance.combatTurn == SystemManager.CombatTurns.playerTurn)
        {
            StartCoroutine(WaitEnemyTurns());
        }
    }

    public void PlayerTurnEnd()
    {

        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.playerEndTurn;
        UIManager.Instance.OnNotification("PLAYER ENDING TURN", 1);

        //discard cards
        DeckManager.Instance.DiscardWholeHand();

    }

    public void EnemyTurnStart()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyStartTurn;
        UIManager.Instance.OnNotification("ENEMY STARTING TURN", 1);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.EnemyTurnStartBD();
    }

    public void EnemyTurn()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyTurn;
        UIManager.Instance.OnNotification("ENEMY TURN", 1);

    }

    public void EnemyTurnEnd()
    {
        SystemManager.Instance.combatTurn = SystemManager.CombatTurns.enemyEndTurn;
        UIManager.Instance.OnNotification("ENEMY ENDING TURN", 1);

    }

    IEnumerator WaitEnemyTurns()
    {

        //player turn start
        PlayerTurnEnd();
        yield return new WaitForSeconds(2f);

        //player turn
        EnemyTurnStart();
        yield return new WaitForSeconds(2f);

        EnemyTurn();
        yield return new WaitForSeconds(2f);

        EnemyTurnEnd();
        yield return new WaitForSeconds(2f);

        PlayerTurnStart();
        yield return new WaitForSeconds(2f);

        PlayerTurn();
        yield return new WaitForSeconds(2f);

    }

    public GameObject InstantiateCharacter(ScriptablePlayer scriptablePlayer, int spawnPosition)
    {

        //instantiate the character prefab based on the spawnPosition
        GameObject character = Instantiate(scriptablePlayer.characterPrefab, characterSpawns[spawnPosition].transform.position, Quaternion.identity);

        //pass it the scriptable object to the class
        character.GetComponent<CharacterClass>().scriptablePlayer = scriptablePlayer;
        //svae the position as it can be used later
        character.GetComponent<CharacterClass>().originalCombatPos = characterSpawns[spawnPosition].transform;

        //parent it to our characters object
        character.transform.SetParent(CombatManager.Instance.combatScene.transform.Find("Characters"));

        return character;

    }

    public void RemoveShieldFromAllCharacters()
    {

        GameObject[] characters = GetAllCharactersGameObjects();

        foreach (GameObject character in characters)
        {
            RemoveShieldFromCharacter(character);
        }

    }

    public GameObject[] GetAllCharactersGameObjects()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");

        return characters;
    }

    public void RemoveShieldFromCharacter(GameObject character)
    {

        CharacterClass characterClass = character.GetComponent<CharacterClass>();

        characterClass.InititializeCharacter();

        characterClass.shield = 0;

        //dont show the icon
        characterClass.shieldIcon.SetActive(false);

        //update text on shield
        characterClass.shieldText.GetComponent<TMP_Text>().text = characterClass.shield.ToString();

        //make the bar red
        characterClass.fillBar.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);

    }

    public GameObject GetTheCharacterThatUsesTheCard(CardScript cardScript)
    {

        GameObject character = null;

        //check if the card belongs to any of our characters
        foreach (GameObject characterInCombat in CombatManager.Instance.charactersInCombat)
        {

            if (characterInCombat.GetComponent<CharacterClass>().scriptablePlayer.mainClass == cardScript.scriptableCard.mainClass)
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
            character = CombatManager.Instance.leaderCharacter;
        }

        //return the appropriate character
        return character;

    }
}
