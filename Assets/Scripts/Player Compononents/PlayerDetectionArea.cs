using UnityEngine;

public class PlayerDetectionArea : MonoBehaviour
{
    public Player Player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DigitalSnare>())
        {
            Player.DigitalSnare = other.GetComponent<DigitalSnare>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DigitalSnare>())
        {
            Player.DigitalSnare = null;
        }
    }
}
