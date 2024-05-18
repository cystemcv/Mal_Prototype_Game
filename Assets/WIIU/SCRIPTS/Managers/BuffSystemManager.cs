using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystemManager : MonoBehaviour
{
    public static BuffSystemManager Instance;

    //for buff/debuff system
    public enum buffDebuffName { NONE, ATTACKUP, POISON };


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

    public BuffDebuffClass TurnAbilityToBuffClass(ScriptableCardAbility scriptableCardAbility)
    {

        BuffDebuffClass buffClass = new BuffDebuffClass();




        return buffClass;
    }

}
