using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatCardHandler : MonoBehaviour
{

    public static CombatCardHandler Instance;

    [Header("CAMERA")]
    public Camera mainCamera;

    [Header("VARIABLES THAT ARE ASSIGNED DYNAMICALLY")]
    public RectTransform targetUIElement;
    public bool targetCardActivated = false;
    public GameObject targetClicked;
    public CombatPosition posClicked = null;
    public List<CombatPosition> posClickedTargeting = new List<CombatPosition>();

    [Header("VISUAL PREFABS")]
    public GameObject discardEffect;
    public GameObject banishEffect;

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


    public void Update()
    {

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.TARGET)
        {

            // Check if the target UI element is available
            if (targetUIElement != null)
            {
                // Get the position of the target UI element in screen space
                //Vector3 targetScreenPosition = targetUIElement.Find("LineRendererStart").position;
                Vector3 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(CombatCardHandler.Instance.mainCamera, targetUIElement.transform.Find("Panel").Find("UtilityFront").Find("LineRendererStart").position);


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
                UI_Combat.Instance.cardLineRenderer.positionCount = curvePoints.Length;
                UI_Combat.Instance.cardLineRenderer.SetPositions(curvePoints);

                // Position the arrowhead at the end of the curve
                UI_Combat.Instance.cardLineRendererArrow.transform.position = curvePoints[curvePoints.Length - 1];
                // Rotate the arrowhead to face the direction of the line
                Vector3 direction = curvePoints[curvePoints.Length - 1] - curvePoints[curvePoints.Length - 2];
                UI_Combat.Instance.cardLineRendererArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);


                //line renderer
                UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(true);
                UI_Combat.Instance.cardLineRendererArrow.SetActive(true);


                // ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity);



                // Update the positions of the line renderer
                //lineRenderer.SetPosition(0, targetPosition);
                //lineRenderer.SetPosition(1, mousePosition);
            }
            CheckHitTarget();
            CheckClickTarget();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.MOVEHERO)
        {

            // Check if the target UI element is available
            if (targetClicked != null)
            {
                // Get the position of the target UI element in screen space

                GameObject hero = targetClicked;
                //Vector3 targetScreenPosition = targetUIElement.Find("LineRendererStart").position;
                Vector3 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(CombatCardHandler.Instance.mainCamera, hero.transform.position);


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
                UI_Combat.Instance.cardLineRenderer.positionCount = curvePoints.Length;
                UI_Combat.Instance.cardLineRenderer.SetPositions(curvePoints);

                // Position the arrowhead at the end of the curve
                UI_Combat.Instance.cardLineRendererArrow.transform.position = curvePoints[curvePoints.Length - 1];
                // Rotate the arrowhead to face the direction of the line
                Vector3 direction = curvePoints[curvePoints.Length - 1] - curvePoints[curvePoints.Length - 2];
                UI_Combat.Instance.cardLineRendererArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);


                //line renderer
                UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(true);
                UI_Combat.Instance.cardLineRendererArrow.SetActive(true);
                UI_Combat.Instance.moveHeroText.SetActive(true);

                UI_Combat.Instance.moveHeroText.transform.position = new Vector3(hero.transform.position.x + 0.5f, hero.transform.position.y, hero.transform.position.z);
                // ChangeTargetMaterial(SystemManager.Instance.materialTargetEntity);



                // Update the positions of the line renderer
                //lineRenderer.SetPosition(0, targetPosition);
                //lineRenderer.SetPosition(1, mousePosition);
            }
            CheckHitMoveHero();
            CheckClickMoveHero();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HitMoveHero();
            }


        }
    }

    public void CheckHitTarget()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));

    


        // Check if the ray intersects with any colliders
        if (hit.collider != null && CardCanTargetEntity(hit.collider.gameObject))
        {

            ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed));

            List<CombatPosition> combatPositionList = Combat.Instance.CheckCardTargets(hit.collider.gameObject, targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard);

            foreach (CombatPosition combatPosition in combatPositionList)
            {
                combatPosition.position.transform.Find("TargetingPrefab").gameObject.SetActive(true);
            }

        }
        else
        {
            ResetAllTargeting();
        }

    }

    public void ResetAllTargeting()
    {
        foreach (CombatPosition combatPosition in Combat.Instance.characterCombatPositions)
        {
            combatPosition.position.transform.Find("TargetingPrefab").gameObject.SetActive(false);
        }

        foreach (CombatPosition combatPosition in Combat.Instance.enemiesCombatPositions)
        {
            combatPosition.position.transform.Find("TargetingPrefab").gameObject.SetActive(false);
        }
    }


    public void CheckHitMoveHero()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));



        // Check if the ray intersects with any colliders
        if (hit.collider != null && (SystemManager.Instance.GetPlayerTagsList().Contains(hit.collider.gameObject.tag) || hit.collider.transform.parent.tag == "PlayerPos"))
        {

            CombatPosition posClicked = Combat.Instance.GetCombatPosition(hit.collider.gameObject);

            ////if it belongs to the positioning tag
            //if (hit.collider.gameObject.name == "Visual")
            //{
            //    posClicked = Combat.Instance.FindCombatPositionByGameObject(hit.collider.gameObject.transform.parent.gameObject);
            //}
            //else
            //{
            //    posClicked = Combat.Instance.FindCombatPositionByEntity(hit.collider.gameObject.transform.gameObject);
            //}

      

            //get hero position
            CombatPosition posHero = Combat.Instance.GetCombatPosition(targetClicked);

            int manaToDrain = Combat.Instance.GetStepsBetweenPositions(posHero, posClicked);

            TMP_Text manaMoveText = UI_Combat.Instance.moveHeroText.transform.Find("TEXT").GetComponent<TMP_Text>();
            manaMoveText.text = manaToDrain.ToString();

            if (Combat.Instance.ManaAvailable < Combat.Instance.GetStepsBetweenPositions(posHero, posClicked))
            {
                manaMoveText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed));
            }
            else
            {
                manaMoveText.color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorGreen));
            }



        }

    }


    public void ChangeLineAndArrowColor(Color color)
    {
        UI_Combat.Instance.cardLineRendererArrow.GetComponent<SpriteRenderer>().color = color;
        UI_Combat.Instance.cardLineRenderer.startColor = color;
        UI_Combat.Instance.cardLineRenderer.endColor = color;
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

        //ResetAllTargeting();

        if (Input.GetMouseButtonDown(0))
        {
            HitTarget();
        }
        else if (Input.GetMouseButtonDown(1))
        {

            targetUIElement.transform.GetChild(0).Find("UtilityBack").Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);

            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);
            UI_Combat.Instance.moveHeroText.SetActive(false);
            //ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            Combat.Instance.HideAllCombatPosVisuals();

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

            ResetAllTargeting();
        }
    }


    public void CheckClickMoveHero()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartMoveHero();
        }
        else if (Input.GetMouseButtonDown(1))
        {



            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);
            UI_Combat.Instance.moveHeroText.SetActive(false);
            //ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            Combat.Instance.HideAllCombatPosVisuals();

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
            //check if the target is the required one or it is not dead
            if (hit.collider.gameObject.GetComponent<EntityClass>() != null)
            {
                if (hit.collider.gameObject.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
                {
                    return;
                }
            }



            if (!CardCanTargetEntity(hit.collider.gameObject))
            {
                return;
            }

            // Handle the click
            //Debug.Log("Mouse clicked on: " + hit.collider.gameObject.name);
            // Add your click handling code here
            targetClicked = hit.collider.gameObject;

            posClickedTargeting = Combat.Instance.CheckCardTargets(targetClicked, targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard);

            //if it belongs to the positioning tag
            if (targetClicked.transform.parent.tag == "EnemyPos" ||
                targetClicked.transform.parent.tag == "PlayerPos")
            {
                posClicked = Combat.Instance.FindCombatPositionByGameObject(targetClicked.transform.parent.gameObject);
            }

            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);
            UI_Combat.Instance.moveHeroText.SetActive(false);
            //ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity);

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

            //do the effects 

            //put it on the list
            PlayedCard playedCard = new PlayedCard();
            playedCard.timer = targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard.waitOnQueueTimer;
            playedCard.target = CombatCardHandler.Instance.targetClicked;
            playedCard.scriptableCard = targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard;
            playedCard.cardScriptData = targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData;
            playedCard.cardScript = targetUIElement.gameObject.GetComponent<CardScript>();
            playedCard.cardObject = targetUIElement.gameObject;

            //decrease available mana
            Combat.Instance.ManaAvailable -= playedCard.cardScriptData.primaryManaCost;

            //DeckManager.Instance.PlayerPlayedCard(targetUIElement.gameObject.GetComponent<CardScript>());
            Combat.Instance.playedCardList.Add(playedCard);


            //save the cardScript temp
            CardScriptData tempCardScriptData = new CardScriptData();
            tempCardScriptData = playedCard.cardScriptData;
            //remove from hand and add it to the played card
            DeckManager.Instance.RemovePlayedCardFromHand(tempCardScriptData);

            playedCard.playedCardUI = UI_Combat.Instance.AddPlayedCardUI(playedCard);

            //return everything where it was
            HandManager.Instance.SetHandCards();


            Combat.Instance.HideAllCombatPosVisuals();

            ResetAllTargeting();


        }
    }

    public void StartMoveHero()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one or it is not dead
            if (hit.collider.gameObject.GetComponent<EntityClass>() != null)
            {
                if (hit.collider.gameObject.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
                {
                    return;
                }
            }



            if (!SystemManager.Instance.GetPlayerTagsList().Contains(hit.collider.gameObject.tag) && hit.collider.transform.parent.tag != "PlayerPos")
            {
                return;
            }

            //get the positioning

            CombatPosition posClicked = Combat.Instance.GetCombatPosition(hit.collider.gameObject);

            ////if it belongs to the positioning tag
            //if (hit.collider.gameObject.name == "Visual")
            //{
            //    posClicked = Combat.Instance.FindCombatPositionByGameObject(hit.collider.gameObject.transform.parent.gameObject);
            //}
            //else
            //{
            //    posClicked = Combat.Instance.FindCombatPositionByEntity(hit.collider.gameObject.transform.gameObject);
            //}

            //get hero position
            CombatPosition posHero = Combat.Instance.GetCombatPosition(targetClicked);

            //check if enough mana
            int manaToDrain = Combat.Instance.GetStepsBetweenPositions(posHero, posClicked);

            if (Combat.Instance.ManaAvailable < manaToDrain)
            {
                return;
            }
            else
            {
                Combat.Instance.ModifyMana(manaToDrain * -1);
            }



            //move the hero to that position
            if (posClicked.entityOccupiedPos == null)
            {
                //then just move to the position
                posClicked.entityOccupiedPos = targetClicked;
                posHero.entityOccupiedPos = null;

                //targetClicked.transform.position = posClicked.position.transform.position;
                StartCoroutine(MoveEntity(targetClicked, posClicked.position.transform.position.x, 0.2f));
            }
            else
            {
                GameObject clickedEntity = posClicked.entityOccupiedPos;
                //then just move to the position
                posClicked.entityOccupiedPos = targetClicked;
                posHero.entityOccupiedPos = clickedEntity;

                //targetClicked.transform.position = posClicked.position.transform.position;
                //clickedEntity.transform.position = posHero.position.transform.position;
                StartCoroutine(MoveEntity(targetClicked, posClicked.position.transform.position.x, 0.2f));
                StartCoroutine(MoveEntity(clickedEntity, posHero.position.transform.position.x, 0.2f));
            }

            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);
            UI_Combat.Instance.moveHeroText.SetActive(false);
            //ChangeTargetMaterial(SystemManager.Instance.materialDefaultEntity);

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;


            //decrease available mana
            // Combat.Instance.ManaAvailable -= playedCard.cardScript.primaryManaCost;


            Combat.Instance.HideAllCombatPosVisuals();
            posClicked = null;

        }
    }

    public IEnumerator MoveEntity(GameObject entity, float moveToX, float moveSpeed)
    {
        UI_Combat.Instance.DisableCombatUI();
        LeanTween.moveX(entity, moveToX, moveSpeed);

        yield return new WaitForSeconds(moveSpeed);
        UI_Combat.Instance.EnableCombatUI();


        yield return null;
    }

    public void HitMoveHero()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one or it is not dead
            if (hit.collider.gameObject.GetComponent<EntityClass>() != null)
            {
                if (hit.collider.gameObject.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD)
                {
                    return;
                }
            }



            if (!SystemManager.Instance.GetPlayerTagsList().Contains(hit.collider.gameObject.tag) && hit.collider.gameObject.name != "Visual")
            {
                return;
            }


            //check if the target is root
            BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(hit.collider.gameObject,"Root");

            if (buffDebuffClass != null)
            {
                NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.ERROR, buffDebuffClass.scriptableBuffDebuff.nameID, buffDebuffClass.scriptableBuffDebuff.description );
                return;
            }

            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.MOVEHERO;

            List<string> tagList = new List<string>();
            tagList.Add("Player");
            tagList.Add("PlayerSummon");
            tagList.Add("PlayerPos");
            List<GameObject> targetPosList = Combat.Instance.FindPosTargeting(tagList);

            List<GameObject> targets = SystemManager.Instance.FindGameObjectsWithTags(tagList);

            targets.AddRange(targetPosList);
            targetClicked = hit.collider.gameObject;

        }
    }

    public bool CardCanTargetEntity(GameObject target)
    {

        //if its targeting position then get the parent
        if (target.name == "Visual")
        {
            target = target.transform.parent.gameObject;
        }

        bool canTarget = false;

        if (System.Enum.TryParse(target.tag, out SystemManager.EntityTag entityTag))
        {
            // Check if the parsed EntityTag is in the targetEntityTag list
            if (targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard.targetEntityTagList.Contains(entityTag))
            {

                canTarget = true;

            }
        }

        return canTarget;
    }

    public void ChangeTargetMaterial(Material customMaterial)
    {

        List<SystemManager.EntityTag> cardEntityTags = targetUIElement.gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard.targetEntityTagList;

        foreach (SystemManager.EntityTag cardEntityTag in cardEntityTags)
        {
            //get all from the tag
            GameObject[] targetsFound = GameObject.FindGameObjectsWithTag(cardEntityTag.ToString());

            foreach (GameObject targetFound in targetsFound)
            {

                if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
                {
                    SystemManager.Instance.ChangeTargetMaterial(customMaterial, targetFound);
                }





            }

        }
    }
}
