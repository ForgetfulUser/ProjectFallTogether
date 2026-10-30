using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public List<UnlockableObject> UnlockableObjects = new List<UnlockableObject>();

    public void StartManager()
    {
        foreach (var unlockableObject in FindObjectsByType<UnlockableObject>(FindObjectsSortMode.None)) 
        {
            unlockableObject.StartObject(this);
            UnlockableObjects.Add(unlockableObject);
        }
    }

    public void UpdateManager()
    {
        foreach(UnlockableObject unlockableObject in UnlockableObjects)
        {
            unlockableObject.UpdateObject();
        }
    }
}
