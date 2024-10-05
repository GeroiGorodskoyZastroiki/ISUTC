using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public abstract class Footsteps : MonoBehaviour
{
    private static List<MaterialSounds> _materialSounds;
    protected bool IsPlaying;

    private void Start() =>
        _materialSounds ??= Resources.LoadAll<MaterialSounds>("Environment/Sounds/MaterialSounds").ToList();

    protected IEnumerator PlayFootstepSound(AudioSource audioSource, float volume, Transform floor)
    {
        if (IsPlaying) yield break;
        IsPlaying = true;
        Material materialUnderCharacter = floor.GetComponent<MeshRenderer>().sharedMaterial;
        MaterialSounds foundMaterialSounds = _materialSounds.FirstOrDefault(x => x.Materials.Exists(x => x.name == materialUnderCharacter.name));
        int index = foundMaterialSounds ? _materialSounds.IndexOf(foundMaterialSounds) : -1;
        if (index == -1)
        {
            IsPlaying = false;
            yield break;
        }
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(_materialSounds[index].FootstepsSounds[Random.Range(0, _materialSounds[index].FootstepsSounds.Count)], volume);
        yield return new WaitForSeconds(0.2f);
        IsPlaying = false;
    }
}
