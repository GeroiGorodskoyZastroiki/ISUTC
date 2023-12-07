using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerConstraintsManager : MonoBehaviour
{
    [SerializeField] private MultiRotationConstraint _leftArmConstraint;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if ((-0.1 < _animator.GetFloat("VelocityX") && _animator.GetFloat("VelocityX") <= 1) && (-0.1 < _animator.GetFloat("VelocityZ") && _animator.GetFloat("VelocityZ") <= 1))
            _leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(_leftArmConstraint.weight, 1, 0.1f), 0, 1);
        else _leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(_leftArmConstraint.weight, 0, 0.1f), 0, 1);
    }
}
