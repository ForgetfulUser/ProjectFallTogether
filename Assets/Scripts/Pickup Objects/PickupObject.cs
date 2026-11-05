using UnityEngine;

public enum ItemType
{
    Crowbar,
    PowerCell,
    Note
}



public class PickupObject : MonoBehaviour
{
    public ItemType ItemType;
    float rotationSpeed = 50;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
        {
            PickUp(other.GetComponent<Player>());
        }
    }

    public virtual void PickUp(Player player)
    {
        if (PlayerManager.PickUpItem(ItemType))
            Destroy(gameObject);
    }
}
