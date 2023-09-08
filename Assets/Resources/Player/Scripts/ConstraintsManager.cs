using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstraintsManager : MonoBehaviour
{
    [SerializeField] MultiRotationConstraint leftArmConstraint;
    Animator animator;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if ((-0.1 < animator.GetFloat("VelocityX") && animator.GetFloat("VelocityX") <= 1) && (-0.1 < animator.GetFloat("VelocityZ") && animator.GetFloat("VelocityZ") <= 1)) 
            leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(leftArmConstraint.weight, 1, 0.1f), 0, 1);
        else leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(leftArmConstraint.weight, 0, 0.1f), 0, 1);
    }
}
