using UnityEngine;

public class FirewallCage : MonoBehaviour
{
    [Tooltip("How long the Firewall will be active")]
    public float LifeTime;
    public Virus CapturedVirus;

    private void Update()
    {
        if(CapturedVirus != null)
        {
            if(LifeTime > 0) 
                LifeTime -= Time.deltaTime;
            else
            {
                CapturedVirus.ToggleCapture();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Boo");
        Debug.Log(other.gameObject.name);
        if (other.GetComponent<Virus>())
        {
            Debug.Log("Got em");
            CapturedVirus = other.GetComponent<Virus>();
            CapturedVirus.ToggleCapture();
        }
    }
}
