using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour
{

    public ScriptablePlayer scriptablePlayer;

    public List<GameObject> listBuffDebuffClass;

    public int poisongDmg;



    // Start is called before the first frame update
    void Start()
    {
        InititializeCharacter();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InititializeCharacter()
    {
        //variables that can change during battle
        poisongDmg = scriptablePlayer.poisonDmg;
    }
}
