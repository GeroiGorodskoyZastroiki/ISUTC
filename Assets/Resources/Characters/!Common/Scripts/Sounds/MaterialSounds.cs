using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MaterialSounds", menuName = "ScriptableObjects/MaterialSounds")]
public class MaterialSounds : ScriptableObject
{
    [InfoBox("Only original materials, NOT material variants!")]
    public List<Material> Materials;
    public List<AudioClip> FootstepsSounds;
}
