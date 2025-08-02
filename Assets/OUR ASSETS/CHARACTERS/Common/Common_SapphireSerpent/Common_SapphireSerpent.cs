using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Common_SapphireSerpent", menuName = "Card/Common/Common_SapphireSerpent")]
public class Common_SapphireSerpent : ScriptableCard
{

    public int drawAmount = 2;
    public ScriptableBuffDebuff wet;

    private GameObject realTarget;
    private GameObject entityUsedCardGlobal;

    public override string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        string customDesc = base.OnCardDescription(cardScriptData, entityUsedCard);

    
        customDesc += "Draw " + drawAmount + " on Odd Turn Number!<br>";
        customDesc += "Add " + BuffSystemManager.Instance.GetBuffDebuffColor(wet) + " to all enemies on Even Turn Number!<br>";
        customDesc += "<color=yellow>" + scriptableKeywords[0].keywordName + "</color>";

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

 
        entityUsedCardGlobal = entityUsedCard;

        ExecuteCard();

    }

    public void ExecuteCard()
    {
   
      
        if (IsOdd(Combat.Instance.turns))
        {
          

            MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                               //hit at least one time if its 0

            runner.StartCoroutine(DeckManager.Instance.DrawMultipleCards(drawAmount, 0));
        }
        else
        {

            List<GameObject> targets = AIManager.Instance.GetAllTargets(entityUsedCardGlobal);

            foreach (GameObject target in targets)
            {
                BuffSystemManager.Instance.AddBuffDebuff(target, wet, 1);
            }
      

        }
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }



}
