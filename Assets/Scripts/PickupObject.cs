using UnityEngine;

public enum ItemType
{
    Crowbar
}

public class PickupObject : MonoBehaviour
{
    public ItemType ItemType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
        {
           if(other.GetComponent<Player>().PickupItem(ItemType))
                Destroy(gameObject);
        }
    }
}
