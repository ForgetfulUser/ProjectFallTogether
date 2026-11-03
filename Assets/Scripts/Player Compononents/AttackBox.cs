using System.Collections;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public GameObject Mesh_OBJ;
    public Coroutine CurrentRoutine;
    public Virus VirusDetected;
    public float AttackTime = 0.2f;

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

    public void HighlightArea()
    {
        if (CurrentRoutine != null) return;
        CurrentRoutine = StartCoroutine(HighlightAreaCoroutine());
    }

    private IEnumerator HighlightAreaCoroutine()
    {
        Mesh_OBJ.SetActive(true);
        yield return new WaitForSeconds(AttackTime);
        Mesh_OBJ.SetActive(false);
        CurrentRoutine = null;
    }
}
