using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "MaterialSounds", menuName = "ScriptableObjects/MaterialSounds")]
public class MaterialSounds : ScriptableObject
{
    [InfoBox("All material variants MUST be included separately!")]
    public List<Material> Materials;
    public List<AudioClip> FootstepsSounds;
}
