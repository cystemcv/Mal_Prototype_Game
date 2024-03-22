
using UnityEngine;

[CreateAssetMenu(fileName = "New Rogue", menuName = "Player/Rogue")]
public class ScriptableRogue : ScriptablePlayer
{
    public mainClass ourMainclass = mainClass.Rogue;

    public ScriptableRogue()
        {
        deftness = 2;//Rogue primary stat gets 1 higher by default
        }
}
