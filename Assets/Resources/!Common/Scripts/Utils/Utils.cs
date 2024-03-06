using UnityEngine;
using System.Linq;

public static class Utils
{
    public static GameObject FindObjectWithInstanceID(int instanceId) =>
        (Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList()).Find(x => x.GetInstanceID() == instanceId);
}
