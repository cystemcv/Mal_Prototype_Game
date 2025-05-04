#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ForceSaveTagManager
{
    [MenuItem("Tools/Force Save TagManager")]
    public static void ForceSave()
    {
        Object tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        EditorUtility.SetDirty(tagManagerAsset);
        AssetDatabase.SaveAssets();
        Debug.Log("TagManager.asset forced to save!");
    }
}
#endif
