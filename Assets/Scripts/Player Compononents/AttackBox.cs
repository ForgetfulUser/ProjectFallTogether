using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public Virus VirusDetected;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Virus>())
        {
            VirusDetected = other.gameObject.GetComponent<Virus>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Virus>())
        {
            VirusDetected = null;
        }
    }
}
