using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public List<AudioClip> footstepsSounds;
    public AudioSource footstepsSource;
    bool isPlaying;

    public void Footstep()
    {
        if (gameObject.tag == "Player") FootstepForPlayer();
        else FootstepForEnemy();
    }

    void FootstepForPlayer()
    {
        if (Mathf.Abs(GetComponent<Animator>().GetFloat("VelocityX")) > 0.39 || Mathf.Abs(GetComponent<Animator>().GetFloat("VelocityZ")) > 0.39)
            if (!isPlaying) StartCoroutine(PlayFootstepSound());
    }

    void FootstepForEnemy()
    {
        //Debug.Log("enemyfootstep");
        if (!isPlaying) StartCoroutine(PlayFootstepSound());
    }

    private IEnumerator PlayFootstepSound()
    {
        isPlaying = true;
        footstepsSource.PlayOneShot(footstepsSounds[Random.Range(0, footstepsSounds.Count - 1)]);
        yield return new WaitForSeconds(0.2f);
        isPlaying = false;
        yield break;
    }
}
