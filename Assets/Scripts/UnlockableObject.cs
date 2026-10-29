using Unity.VisualScripting;
using UnityEngine;

public class UnlockableObject : MonoBehaviour
{
    public ItemType Type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            Player player = other.gameObject.GetComponent<Player>();
            foreach (ItemType type in player.ItemTypes)
            {
                if (UnlockObject(type))
                {
                    player.ItemTypes.Remove(type);
                    break;
                }
            }
        }
    }

    public bool UnlockObject(ItemType itemType)
    {
        if(itemType == Type)
        {
            Debug.Log("Unlocked");
            return true;
        }
        return false;
    }
}
