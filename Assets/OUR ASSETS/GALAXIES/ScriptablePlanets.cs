using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptablePlanets", menuName = "scriptableGalaxies/ScriptablePlanets")]
public class ScriptablePlanets : ScriptableObject
{

    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite planetArt;

    [ VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.PlanetTypes planetType;

    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string planetName;

    [ShowIf("@(this.planetType == SystemManager.PlanetTypes.BATTLE)")]
    public List<ScriptableEntity> scriptableEntities;

    [ShowIf("@(this.planetType == SystemManager.PlanetTypes.BATTLE)")]
    public ScriptableBattleGrounds planetBattleGround;


    [FoldoutGroup("CONDITIONS")]
    public List<ItemClassPlanet> itemClassPlanet;

    [Serializable]
    public class ItemClassPlanet
    {
        [Title("CONDITION"), GUIColor("orange")]
        public ScriptableItem scriptableItem;
        [Title("CONDITION VALUES")]
        public int minQuantity = 0;
        public int maxQuantity = 0;
        public int percentage = 0;
    }

}
