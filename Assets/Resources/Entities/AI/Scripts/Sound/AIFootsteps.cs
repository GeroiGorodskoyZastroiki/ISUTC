using UnityEngine;

public class AIFootsteps : Footsteps
{
    [SerializeField] private AudioSource _leftFootstep;
    [SerializeField] private AudioSource _rightFootstep;

    public void TakeStep(string foot)
    {
        AudioSource audioSource = null;
        if (foot == "left") audioSource = _leftFootstep;
        else if (foot == "right") audioSource = _rightFootstep;
        else audioSource = _rightFootstep;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("StaticGeometry"));
        if (hit.transform == null) return;
        StartCoroutine(PlayFootstepSound(audioSource, 1, hit.transform));
    }
}
