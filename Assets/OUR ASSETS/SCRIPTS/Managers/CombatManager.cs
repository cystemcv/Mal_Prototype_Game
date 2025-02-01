using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public ScriptablePlanets scriptablePlanet;

    public GameObject planetClicked;

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

    }



    public void SpawnEffectPrefab(GameObject target, ScriptableCard scriptableCard)
    {
        //missing prefab vfx
        if (scriptableCard.abilityEffect == null || target == null)
        {
            return;
        }

        //spawn prefab
        GameObject abilityEffect = Instantiate(scriptableCard.abilityEffect, target.transform.Find("model").Find("SpawnEffect").position, Quaternion.identity);
        //abilityEffect.transform.SetParent(target.transform.Find("model").Find("SpawnEffect"));

        abilityEffect.transform.position = new Vector3(abilityEffect.transform.position.x,
            abilityEffect.transform.position.y + scriptableCard.abilityEffectYaxis,
            abilityEffect.transform.position.z);

        //if the target is player then we wanna rotate 
        if (target.tag == "Player")
        {
            abilityEffect.transform.Rotate(new Vector3(0, 180, 0));
        }

        Destroy(abilityEffect, scriptableCard.abilityEffectLifetime);
    }

}
