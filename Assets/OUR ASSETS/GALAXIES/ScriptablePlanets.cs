using Sirenix.OdinInspector;
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



}
