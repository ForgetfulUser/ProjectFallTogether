using UnityEngine;

public class WinBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Player>()  != null)
        {
            if (GameManager.Instance.PowerIsOn && GameManager.Instance.BarricadeIsGone && GameManager.Instance.DoorIsUnlocked) GameManager.Instance.DoorIsOpen = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if (GameManager.Instance.PowerIsOn && GameManager.Instance.BarricadeIsGone && GameManager.Instance.DoorIsUnlocked) GameManager.Instance.DoorIsOpen = true;
        }
    }
}
