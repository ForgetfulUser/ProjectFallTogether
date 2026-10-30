using UnityEngine;
using UnityEngine.SearchService;

public class Keypad : UnlockableObject
{
    public override bool UnlockCheck()
    {
        GameManager instance = GameManager.Instance;
        bool unlocked = PlayerManager.NoteCount >= 3 && instance.PowerIsOn && instance.BarricadeIsGone;
        instance.DoorIsUnlocked  = unlocked;
        return unlocked;
    }
}
