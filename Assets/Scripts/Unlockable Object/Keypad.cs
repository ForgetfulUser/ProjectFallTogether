using UnityEngine;
using UnityEngine.SearchService;

public class Keypad : UnlockableObject
{
    public override bool UnlockCheck()
    {
        bool unlocked = PlayerManager.NoteCount >= 3;
        GameManager.Instance.DoorIsUnlocked = unlocked;
        return unlocked;
    }
}
