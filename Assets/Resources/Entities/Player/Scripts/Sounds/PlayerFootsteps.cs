using UnityEngine;

public class PlayerFootsteps : Footsteps
{
    [SerializeField] private AudioSource _leftFootstep;
    [SerializeField] private AudioSource _rightFootstep;

    public void TakeStep(string foot)
    {
        AudioSource audioSource = null;
        if (foot == "left") audioSource = _leftFootstep;
        else if (foot == "right") audioSource = _rightFootstep;
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("StaticGeometry"));
        Debug.Log(hit.transform.gameObject.name);
        float volumeFactor = 0.5f;
        if (GetComponentInParent<Player>().Movement.Sprint) volumeFactor = 1;
        if (GetComponentInParent<Player>().Movement.Crouch) volumeFactor = 0.25f;
        StartCoroutine(PlayFootstepSound(audioSource, volumeFactor, hit.transform));
    }
}
