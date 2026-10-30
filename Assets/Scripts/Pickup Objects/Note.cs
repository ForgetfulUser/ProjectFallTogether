using UnityEngine;

public class Note : PickupObject
{
    public int noteID;

    public override void PickUp(Player player)
    {
        if(PlayerManager.PickUpItem(noteID))
            Destroy(gameObject);
        
    }
}
