using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Footsteps : MonoBehaviour
{
    private AudioSource _audioSource;
    private static List<MaterialSounds> _materialSounds;
    protected bool IsPlaying;

    private void Start()
    {
        _materialSounds ??= Resources.LoadAll<MaterialSounds>("Entities/!Common/Scripts/Sounds/MaterialSounds").ToList();
        _audioSource = GetComponent<AudioSource>();
    }

    public abstract void TakeStep(Transform transform);

    protected IEnumerator PlayFootstepSound(float volume, Transform floor)
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
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.PlayOneShot(_materialSounds[index].FootstepsSounds[Random.Range(0, _materialSounds[index].FootstepsSounds.Count)], volume);
        yield return new WaitForSeconds(0.2f);
        IsPlaying = false;
    }
}
