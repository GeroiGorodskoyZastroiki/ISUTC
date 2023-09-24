using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MaterialSounds", menuName = "ScriptableObjects/MaterialSounds")]
public class MaterialSounds : ScriptableObject
{
    [InfoBox("All material variants MUST be included separately!")]
    public List<Material> Materials;
    public List<AudioClip> FootstepsSounds;
}
