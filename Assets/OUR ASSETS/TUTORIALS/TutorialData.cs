using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public Sprite image;
    public string title;
    [TextArea(3, 5)]
    public string description;
}

[CreateAssetMenu(fileName = "NewTutorial", menuName = "Tutorial System/Tutorial")]
public class TutorialData : ScriptableObject
{
    public string tutorialName;
    public TutorialStep[] steps;
}
