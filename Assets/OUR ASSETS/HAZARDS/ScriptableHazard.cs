using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Michsky.MUIP;
using System.Collections;

[CreateAssetMenu(fileName = "New Hazard", menuName = "Hazard/Master")]
public class ScriptableHazard : ScriptableObject // Not sure if cards will be made from here or specialized for each class. Possibly change to abstract class later
{


    [HideLabel, PreviewField(80), HorizontalGroup("CardHeader", 80)]
    public Sprite hazardIcon; // Art to be displayed and attached to the card

    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string hazardName;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public string hazardDescription;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public GameObject hazardEffect;
    [VerticalGroup("CardHeader/CardHeaderDetails"), GUIColor("orange")]
    public AudioClip hazardAudio;

    public float yAxisEffect = 0f;

    public virtual IEnumerator OnTurnStart(CombatPosition combatPosition)
    {
        yield return null;
    }

    public virtual IEnumerator OnTurnEnd(CombatPosition combatPosition)
    {
        yield return null;
    }

    public void PlayEffects(CombatPosition combatPosition)
    {
        // Play the sound
        if (this.hazardAudio != null)
        {
            AudioManager.Instance.cardSource.PlayOneShot(this.hazardAudio);
        }

        if (this.hazardEffect != null)
        {
            Vector2 vector2 = new Vector2(combatPosition.position.transform.position.x, combatPosition.position.transform.position.y + yAxisEffect);
            GameObject effect = Instantiate(hazardEffect, vector2, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }


}