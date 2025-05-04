using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ScriptableEvent", menuName = "Events/ScriptableEvent")]
public class ScriptableEvent : ScriptableObject
{
    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite icon;
    public string title;
    [TextArea(1, 20)]
    public List<string> conversation = new List<string>();

    public List<ScriptableButtonEvent> scriptableButtonEventList;

    public ScriptablePlanets scriptablePlanet;




}
