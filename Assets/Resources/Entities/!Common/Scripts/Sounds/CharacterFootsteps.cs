using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public abstract class CharacterFootsteps : MonoBehaviour
{
    private AudioSource _audioSource;
    private List<MaterialSounds> _materialSounds;
    protected bool IsPlaying;

    private void Start()
    {
        _materialSounds = Resources.LoadAll<MaterialSounds>("Entities/!Common/Scripts").ToList();
        _audioSource = GetComponent<AudioSource>();
    }

    public abstract void TakeStep();

    protected IEnumerator PlayFootstepSound(float volume)
    {
        if (IsPlaying) yield break;
        IsPlaying = true;
        if (!Physics.Raycast(_audioSource.transform.position, (-_audioSource.transform.up), out RaycastHit hitInfo, _audioSource.transform.localPosition.y + 0.05f, LayerMask.GetMask("StaticGeometry")))
        {
            IsPlaying = false;
            yield break;
        }
        Material materialUnderCharacter = hitInfo.transform.GetComponent<MeshRenderer>().sharedMaterial;
        int index = _materialSounds.IndexOf(_materialSounds.First(x => x.Materials.Exists(x => x.name == materialUnderCharacter.name)));
        _audioSource.PlayOneShot(_materialSounds[index].FootstepsSounds[Random.Range(0, _materialSounds[index].FootstepsSounds.Count)], volume);
        yield return new WaitForSeconds(0.2f);
        IsPlaying = false;
    }
}
