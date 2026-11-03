using System.Collections;
using UnityEngine;

public class PlayerDetectionArea : MonoBehaviour
{
    public GameObject Mesh_OBJ;
    public Coroutine CurrentRoutine;
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

    public void HighlightArea()
    {
        if (CurrentRoutine != null) return;
        CurrentRoutine = StartCoroutine(HighlightAreaCoroutine());
    }

    private IEnumerator HighlightAreaCoroutine()
    {
        Mesh_OBJ.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Mesh_OBJ.SetActive(false);
        CurrentRoutine = null;
    }
}
