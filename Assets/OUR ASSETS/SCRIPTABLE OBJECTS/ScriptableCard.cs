using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Master")]
public class ScriptableCard : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{


    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite cardArt; // Art to be displayed and attached to the card

    [Required, VerticalGroup("CardHeader/CardHeaderDetails"),GUIColor("orange")]
    public string cardName;
    [Required, VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.MainClass mainClass = SystemManager.MainClass.MONSTER;
    [Required, VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.CardType cardType = SystemManager.CardType.Attack;
    [Required, VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.CardRarity cardRarity = SystemManager.CardRarity.Common;

    [Title("SETTINGS")]
    public List<SystemManager.EntityTag> targetEntityTagList;

    [Title("CARD DETAILS")]
    [Range(0, 9)]
    public int primaryManaCost; // Amount of mana of the primary type to spend to play the card
    public string cardDesc; // Description of what the card does
    public string cardFlavor; // Flavor text- maybe not needed


    [FoldoutGroup("ABILITIES")]
    [RequiredListLength(0,3)]
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



}