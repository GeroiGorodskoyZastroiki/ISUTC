using UnityEngine;

public class PlayerFootsteps : CharacterFootsteps
{
    public override void ValidateFootstep()
    {
        var animator = GetComponentInParent<Animator>();
        if (Mathf.Abs(animator.GetFloat("VelocityX")) > 0.39 || Mathf.Abs(animator.GetFloat("VelocityZ")) > 0.39)
            base.ValidateFootstep();
    }
}
