using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public abstract class CharacterFootsteps : MonoBehaviour
{
    AudioSource audioSource;
    List<MaterialSounds> materialSounds;
    protected bool isPlaying = false;

    void Start()
    {
        materialSounds = Resources.LoadAll<MaterialSounds>("Characters/!Common/Scripts").ToList();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void ValidateFootstep()
    {
        if (!isPlaying) StartCoroutine(PlayFootstepSound());
    }

    protected IEnumerator PlayFootstepSound()
    {
        isPlaying = true;
        if (!Physics.Raycast(audioSource.transform.position, (-audioSource.transform.up), out RaycastHit hitInfo, audioSource.transform.localPosition.y + 0.05f, LayerMask.GetMask("StaticGeometry")))
        {
            isPlaying = false;
            yield break;
        }
        Material materialUnderCharacter = hitInfo.transform.GetComponent<Renderer>().sharedMaterial;
        int index = materialSounds.IndexOf(materialSounds.First(x => x.Materials.Exists(x => (x.name == (materialUnderCharacter.parent == null ? materialUnderCharacter.name : materialUnderCharacter.parent.name) == true))));
        audioSource.PlayOneShot(materialSounds[index].FootstepsSounds[Random.Range(0, materialSounds[index].FootstepsSounds.Count)]);
        yield return new WaitForSeconds(0.2f);
        isPlaying = false;
        yield break;
    }
}
