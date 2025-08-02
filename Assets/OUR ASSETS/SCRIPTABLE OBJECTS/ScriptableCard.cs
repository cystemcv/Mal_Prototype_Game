using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Michsky.MUIP;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Master")]
public class ScriptableCard : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{


    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite cardArt; // Art to be displayed and attached to the card

    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string cardName;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.MainClass mainClass = SystemManager.MainClass.MONSTER;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.CardType cardType = SystemManager.CardType.Attack;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.CardType cardTypeSecondary = SystemManager.CardType.None;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.Rarity cardRarity = SystemManager.Rarity.Common;

    [BoxGroup("TARGETING")]
    [HorizontalGroup("TARGETING/Row", Width = 20), HideLabel]
    public bool toggle1;

    [BoxGroup("TARGETING")]
    [HorizontalGroup("TARGETING/Row", Width = 20), HideLabel]
    public bool toggle2;

    [BoxGroup("TARGETING")]
    [HorizontalGroup("TARGETING/Row", Width = 20), HideLabel]
    public bool toggle3;

    [BoxGroup("TARGETING")]
    [HorizontalGroup("TARGETING/Row", Width = 20), HideLabel]
    public bool toggle4;

    [BoxGroup("TARGETING")]
    [HorizontalGroup("TARGETING/Row", Width = 20), HideLabel]
    public bool toggle5;

    [Title("SETTINGS")]
    public List<SystemManager.EntityTag> targetEntityTagList;
    public float waitOnQueueTimer = 0.5f;
    public bool cannotDiscard = false;
    public bool cannorRemoveFromDeck = false;
    public bool discardCardAtEndTurn = true;

    [Title("KEYWORDS")]
    public List<ScriptableKeywords> scriptableKeywords = new List<ScriptableKeywords>();

    [Title("BUFFS/DEBUFFS")]
    public List<ScriptableBuffDebuff> scriptableBuffDebuffs = new List<ScriptableBuffDebuff>();

    [Title("HAZARDS")]
    public List<ScriptableHazard> scriptableHazards = new List<ScriptableHazard>();

    [Title("CARD DETAILS")]
    [Range(0, 9)]
    public int primaryManaCost; // Amount of mana of the primary type to spend to play the card
    public string cardDesc; // Description of what the card does

    [Title("VFX")]
    public GameObject abilityEffect;
    public float abilityEffectLifetime = 0.6f;
    public float abilityEffectYaxis = 0f;


    [Title("CARD FLAVOR")]
    public SystemManager.EntityAnimation entityAnimation = SystemManager.EntityAnimation.MeleeAttack;
    public AudioClip cardSoundEffect;
    public string cardFlavor; // Flavor text- maybe not needed





    [FoldoutGroup("ABILITIES")]
    [RequiredListLength(0, 3)]
    public List<CardAbilityClass> cardAbilityClass;




    [Title("UNLOCKABLES")]
    public bool unlockable = false;//available from start = false, unlockable = true

    [Serializable]
    public class CardAbilityClass
    {
        [Title("ABILITY"), GUIColor("orange")]
        public ScriptableCardAbility scriptableCardAbility;
        [Title("ABILITY TIME")]
        public float waitForAbility = 0.2f;
        [Title("ABILITY VISUALS")]
        public GameObject abilityEffect;
        public float abilityEffectLifetime = 0.2f;
        public float abilityEffectYaxis = 0f;
        public SystemManager.EntityAnimation entityAnimation = SystemManager.EntityAnimation.MeleeAttack;
        public SystemManager.EntitySound entitySound = SystemManager.EntitySound.Generic;
        [Title("ABILITY VALUES")]
        public List<int> abilityIntValueList;
        public List<string> abilityStringValueList;
        public List<bool> abilityBoolValueList;
        public List<float> abilityFloatValueList;
        [Title("SUMMONS")]
        public List<ScriptableEntity> summonList;


    }

    [FoldoutGroup("CONDITIONS")]
    [RequiredListLength(0, 3)]
    public List<CardConditionClass> cardConditionClass;

    [Serializable]
    public class CardConditionClass
    {
        [Title("CONDITION"), GUIColor("orange")]
        public ScriptableCardCondition ScriptableCardCondition;
        [Title("CONDITION VALUES")]
        public List<int> abilityIntValueList;
        public List<string> abilityStringValueList;
        public List<bool> abilityBoolValueList;
        public List<float> abilityFloatValueList;
    }


    public virtual string OnCardDescription(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        return cardDesc;
    }

    public virtual void OnPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        ////remove tooltips
        //if (cardScriptData.gameObject != null)
        //{
        //    cardScript.gameObject.GetComponent<TooltipContent>().ProcessExit();
        //}


        CharacterManager.Instance.ProceedWithAnimationAndSound(entityUsedCard, this);
    }

    public virtual void OnAiPlayCard(CardScriptData cardScriptData, GameObject entityUsedCard)
    {
        //remove tooltips
        //if (cardScriptData.gameObject != null)
        //{
        //    cardScript.gameObject.GetComponent<TooltipContent>().ProcessExit();
        //}

        CharacterManager.Instance.ProceedWithAnimationAndSound(entityUsedCard, this);
    }

    public virtual void OnAiPlayTarget(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }

    public virtual void OnDrawCard(CardScriptData cardScriptData)
    {

    }

    public virtual void OnInitializeCard(CardScriptData cardScriptData)
    {

    }

    public virtual void OnDelayEffect(CardScriptData cardScriptData)
    {

    }

    public virtual void OnCardCondition(CardScriptData cardScriptData, GameObject entityUsedCard)
    {

    }


}