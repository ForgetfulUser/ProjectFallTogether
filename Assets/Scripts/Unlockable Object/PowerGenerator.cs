using UnityEngine;

public class PowerGenerator : UnlockableObject
{
    public override bool UnlockCheck()
    {
        if (base.UnlockCheck() == false) return false;

        return PlayerManager.HasPowerCells();
    }
}
