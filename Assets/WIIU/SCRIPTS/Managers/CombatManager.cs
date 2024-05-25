using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    //targeting system
    public bool targetMode = false;
    public RectTransform targetUIElement;
    public LineRenderer lineRenderer;
    public GameObject targetClicked;

    
    public enum combatTurn { playerStartTurn,playerTurn, playerEndTurn, enemyStartTurn,enemyTurn, enemyEndTurn }
    public int turns = 0;

    public combatTurn currentTurn;

    //---------------

    //damage system
    public GameObject numberOnScreen;

    //---------------

    public int manaMaxAvailable = 3;
    public int manaAvailable = 0;

    //ui healthbar colors
    public Color32 fillbarWithoutShield =  new Color32(255,0,0,255);
    public Color32 fillbarWithShield = new Color32(0, 121, 255, 255);

    private float fillbarVelocity = 0;
    private float fillbarSmoothValue = 0.3f;

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
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList) {
            UpdateCardAfterManaChange(cardPrefab);
        } 

    }

    public void Start()
    {
        lineRenderer = this.transform.Find("LINERENDERER").GetComponent<LineRenderer>();

        // Set the initial positions of the line (start and end)
        lineRenderer.positionCount = 2;

        // Get the material of the Line Renderer
        Material material = lineRenderer.material;

        // Set the sorting order to a high value to ensure it renders on top
        material.renderQueue = 999999; // Adjust the value as needed
    }

    public void Update()
    {

        if (targetMode)
        {

            // Check if the target UI element is available
            if (targetUIElement != null)
            {
                // Get the position of the target UI element in screen space
                Vector3 targetScreenPosition = targetUIElement.Find("LineRendererStart").position;

                // Convert the screen space position to world space
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(targetScreenPosition);
                targetPosition.z = 0f; // Ensure that the z-coordinate is set to 0

                // Get the position of the mouse in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f; // Ensure that the z-coordinate is set to 0

                //remove line renderer
                lineRenderer.gameObject.SetActive(true);

                // Update the positions of the line renderer
                lineRenderer.SetPosition(0, targetPosition);
                lineRenderer.SetPosition(1, mousePosition );
            }

            CheckClickTarget();
        }

    }

    public void CheckClickTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // Check if the ray intersects with any colliders
            if (hit.collider != null)
            {
                // Handle the click
                Debug.Log("Mouse clicked on: " + hit.collider.gameObject.name);
                // Add your click handling code here
                targetClicked = hit.collider.gameObject;

                //do the effects 
                DeckManager.Instance.PlayCard(targetUIElement.gameObject.GetComponent<CardScript>());
                //return everything where it was
                HandManager.Instance.SetHandCards();

                //remove line renderer
                lineRenderer.gameObject.SetActive(false);

                //leave from target
                targetMode = false;


            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            //remove line renderer
            lineRenderer.gameObject.SetActive(false);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            //leave from target
            targetMode = false;
        }
    }

    public void AdjustHealth(GameObject target, int adjustNumber, bool bypassShield)
    {


        //update ui

        //update based on type

        if (target.tag == "Enemy")
        {
            AdjustHealthEnemy(target, adjustNumber, bypassShield);
        }
        else if(target.tag == "Player")
        {

        }
        else
        {
            //we do nothing
        }

    }

    public void AdjustHealthEnemy(GameObject target, int adjustNumber, bool bypassShield)
    {
      
        EnemyClass enemyClass = target.GetComponent<EnemyClass>();

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
                enemyClass.fillBar.GetComponent<Image>().color = fillbarWithShield;

            }
            else
            {
                //then the enemy has no shield left
                enemyClass.shield = 0;

                //make the bar red
                enemyClass.fillBar.GetComponent<Image>().color = fillbarWithoutShield;

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

            if(enemyClass.health < 0)
            {
                enemyClass.health = 0;
            }

            //update text on hp
            enemyClass.healthText.GetComponent<TMP_Text>().text = enemyClass.health + " / " + enemyClass.maxHealth;

            //adjust the hp bar
            UpdateEnemyHealthBarSmoothly(enemyClass);

        }

      


        //spawn numberOn screen
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject numberOnScreenPrefab = Instantiate(numberOnScreen, target.transform.position, Quaternion.identity);
        numberOnScreenPrefab.transform.SetParent(target.transform);

        //assign the number change
        numberOnScreenPrefab.transform.Find("Text").GetComponent<TMP_Text>().text = adjustNumber.ToString();

        Destroy(numberOnScreenPrefab, 1f);

        //if an enemy reach 0 hp then it should be destroyed

        if (enemyClass.health <= 0)
        {
            Destroy(target,1f);
        }


    }

    public void UpdateEnemyHealthBarSmoothly(EnemyClass enemyClass)
    {
        StartCoroutine(SmoothUpdateEnemyHealthBar(enemyClass));
    }

    private IEnumerator SmoothUpdateEnemyHealthBar(EnemyClass enemyClass)
    {
        float targetValue = (float)enemyClass.health / (float)enemyClass.maxHealth;
        while (Mathf.Abs(enemyClass.slider.value - targetValue) > 0.01f)
        {
            enemyClass.slider.value = Mathf.SmoothDamp(enemyClass.slider.value, targetValue, ref fillbarVelocity, fillbarSmoothValue);
            yield return null; // Wait for the next frame
        }
        enemyClass.slider.value = targetValue; // Ensure it's set to the target value at the end
    }

    public void UpdateCardAfterManaChange(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        TMP_Text cardManaCostText = cardPrefab.transform.GetChild(0).transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>();


        //check the mana cost of each
        if (CombatManager.Instance.manaAvailable < scriptableCard.primaryManaCost)
        {
            //cannot be played
            cardManaCostText.color = new Color(255, 0, 0, 255);
        }
        else
        {
            //can be played
            cardManaCostText.color = new Color(255, 255, 255, 255);
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
        //give mana to player
        RefillMana();

        //change into combat mode
        SystemManager.Instance.currentSystemMode = SystemManager.SystemModes.COMBAT;

        //close the UI window
        UIManager.Instance.UIMENU.SetActive(false);

        //open the UI combat menu
        UIManager.Instance.UICOMBAT.SetActive(true);

        //test deck
        DeckManager.Instance.BuildStartingDeck();

        StartCoroutine(WaitPlayerTurns());



        //initialize the characters

        //initialize the enemies

        //we draw for each character




    }

    public void PlayerTurnStart()
    {
        //start player turn
        currentTurn = combatTurn.playerStartTurn;

        //mana should go back to full
        RefillMana();

        //discard cards
        DeckManager.Instance.DiscardWholeHand();

        //draw cards
        DeckManager.Instance.DrawMultipleCards(DeckManager.Instance.turnHandCardsLimit);

        UIManager.Instance.OnNotification("PLAYER STARTING TURN", 1);
    }

    public void PlayerTurn()
    {
        currentTurn = combatTurn.playerTurn;
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
        if (currentTurn == combatTurn.playerTurn) {
            StartCoroutine(WaitEnemyTurns());
        }
    }

    public void PlayerTurnEnd()
    {
        currentTurn = combatTurn.playerEndTurn;
        UIManager.Instance.OnNotification("PLAYER ENDING TURN", 1);

    }

    public void EnemyTurnStart()
    {
        currentTurn = combatTurn.enemyStartTurn;
        UIManager.Instance.OnNotification("ENEMY STARTING TURN", 1);

        //loop for all buffs and debuffs
        BuffSystemManager.Instance.EnemyTurnStartBD();
    }

    public void EnemyTurn()
    {
        currentTurn = combatTurn.enemyTurn;
        UIManager.Instance.OnNotification("ENEMY TURN", 1);

    }

    public void EnemyTurnEnd()
    {
        currentTurn = combatTurn.enemyEndTurn;
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
}
