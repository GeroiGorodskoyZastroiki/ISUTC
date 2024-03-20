using UnityEngine;

public class EnemyFootsteps : Footsteps
{
    [SerializeField] private AudioSource _leftFootstep;
    [SerializeField] private AudioSource _rightFootstep;

    public void TakeStep(string foot)
    {
        AudioSource audioSource = null;
        if (foot == "left") audioSource = _leftFootstep;
        else if (foot == "right") audioSource = _rightFootstep;
        else audioSource = _rightFootstep;
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("StaticGeometry"));
        StartCoroutine(PlayFootstepSound(audioSource, 1, hit.transform));
    }
}
