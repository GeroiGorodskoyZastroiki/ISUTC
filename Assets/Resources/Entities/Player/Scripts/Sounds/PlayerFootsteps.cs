using UnityEngine;

public class PlayerFootsteps : CharacterFootsteps
{
    public override void TakeStep()
    {
        var animator = GetComponentInParent<Animator>();
        float volumeFactor = 0.7f;
        if (GetComponentInParent<Player>().Movement.Sprint) volumeFactor = 1;
        if (GetComponentInParent<Player>().Movement.Crouch) volumeFactor = 0.5f;
        if (Mathf.Abs(animator.GetFloat("VelocityX")) > 0.19 || Mathf.Abs(animator.GetFloat("VelocityZ")) > 0.19)
            StartCoroutine(PlayFootstepSound(volumeFactor));
    }
}
