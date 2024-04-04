using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SearchMode", menuName = "ScriptableObjects/SearchMode")]
public class SearchMode : ScriptableObject
{
    public bool Sprint;
    public sbyte Points;
    public float WaitTime;
    public float MaxRange;
}
