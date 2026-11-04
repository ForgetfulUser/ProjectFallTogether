using UnityEngine;
using UnityEngine.SearchService;

public class Keypad : UnlockableObject
{
    public override bool UnlockCheck()
    {
        Debug.Log("Checking Lock");
        GameManager instance = GameManager.Instance;
        bool unlocked = PlayerManager.NoteCount >= 3 && instance.PowerIsOn;
        instance.DoorIsUnlocked  = unlocked;
        Debug.Log("Is Unlocked " + unlocked);
        return unlocked;
    }
}
