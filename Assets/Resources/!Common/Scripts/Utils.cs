using UnityEngine;
using System.Linq;

public static class Utils
{
    public static GameObject FindObjectWithInstanceID(int instanceID)
    {
        return (GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList()).Find(x => x.GetInstanceID() == instanceID);
    }
}
