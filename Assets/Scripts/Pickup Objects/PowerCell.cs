using UnityEngine;

public class PowerCell : PickupObject
{
    public bool IsBlue;

    public override void PickUp(Player player)
    {
        if (PlayerManager.PickUpItem(IsBlue))
            Destroy(gameObject);
    }
}
