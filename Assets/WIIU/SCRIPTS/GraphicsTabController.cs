using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsTabController : MonoBehaviour
{
    public static GraphicsTabController Instance;

    public bool changeQualitySettings = false;
    public int checkQualitySettingIndexNow = 0;

    //controls
    public CustomDropdown selectQualityOption;
    public CustomDropdown selectShadowResolution;
    public CustomDropdown selectAntiAliasing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //initialize it
        checkQualitySettingIndexNow = QualitySettings.GetQualityLevel();
    }

    public void SelectQualityOption()
    {

        AudioManager.Instance.PlaySfx("UI_Confirm");
        int qualityOptionIndex = 0;

        qualityOptionIndex = int.Parse(selectQualityOption.selectedText.text.Split(":")[0].Trim());

        //change setting
        if (qualityOptionIndex != checkQualitySettingIndexNow)
        {
            //then a change was occured different from what the user had
            changeQualitySettings = true;
        }
        else
        {
            changeQualitySettings = false;
        }
        QualitySettings.SetQualityLevel(qualityOptionIndex,false);

        //initialize all other controls
        int index_ShadowResolution = selectShadowResolution.items.FindIndex(item => item.itemName == QualitySettings.shadowResolution.ToString());
        selectShadowResolution.ChangeDropdownInfo(index_ShadowResolution);

        int index_AntiAliasing = selectAntiAliasing.items.FindIndex(item => item.itemName.Split(":")[0].Trim() == QualitySettings.antiAliasing.ToString());
        selectAntiAliasing.ChangeDropdownInfo(index_AntiAliasing);

    }
}
