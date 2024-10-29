using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "Item/ScriptableItem")]
public class ScriptableItem : ScriptableObject
{
    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite Icon;

    [Required, VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string itemName;
    [Required, VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string itemCategory;

    [TextArea] public string itemDescription;



}
