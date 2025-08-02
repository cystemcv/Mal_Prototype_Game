using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_Card_ConfusionSong", menuName = "Card/Common/Common_Card_ConfusionSong")]
public class Common_Card_ConfusionSong : ScriptableCard
{


    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);


        customDesc += "Progress all enemies turn by 1 changing their card played";

        return customDesc;
    }

    public override void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnPlayCard(cardScriptData, entityUsedCard);

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public override void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        base.OnAiPlayCard(cardScriptData, entityUsedCard);

        entityUsedCardGlobal = GameObject.FindGameObjectWithTag("Player");

        ExecuteCard();
    }

    public void ExecuteCard()
    {


        MonoBehaviour runner = CombatCardHandler.Instance;


        List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);

        foreach (GameObject target in targets)
        {

            AIBrain aIBrain = target.GetComponent<AIBrain>();

            if (aIBrain != null)
            {
                aIBrain.IncreaseAiStep();
                aIBrain.GenerateIntend();
            }

        }


    }




}
