using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "Item/ScriptableItem")]
public class ScriptableItem : ScriptableObject
{
    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite Icon;

    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string itemName;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public SystemManager.ItemCategory itemCategory = SystemManager.ItemCategory.RESOURCE;

    [Title("Activation Type")]
    public SystemManager.ActivationType activationType;
    public SystemManager.ActivationType initializationType;
    public SystemManager.ActivationType expiredType;

    [Title("Details")]
    [TextArea] public string itemDescription;

    public int maxLevel = 1;

    // Abstract method for activation, implemented by derived classes
    public virtual void Activate(ClassItem classItem)
    {

    }

    public virtual void Initialiaze(ClassItem classItem)
    {

    }

    public virtual void Expired(ClassItem classItem)
    {

    }

    public virtual void GameStart()
    {

    }



}
