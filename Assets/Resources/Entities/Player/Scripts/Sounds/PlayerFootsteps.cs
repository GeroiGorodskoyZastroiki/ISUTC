using UnityEngine;

public class PlayerFootsteps : Footsteps
{
    private void OnTriggerEnter(Collider other) => TakeStep(other.transform);

    public override void TakeStep(Transform floor)
    {
        float volumeFactor = 0.5f;
        if (GetComponentInParent<Player>().Movement.Sprint) volumeFactor = 1;
        if (GetComponentInParent<Player>().Movement.Crouch) volumeFactor = 0.25f;
        StartCoroutine(PlayFootstepSound(volumeFactor, floor));
    }
}
