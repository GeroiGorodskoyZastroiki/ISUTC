using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    #region Data
    private Vector2 _animVelocity;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    private Animator _animator;
    #endregion

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Footstep() =>
        GetComponentInChildren<PlayerFootsteps>().ValidateFootstep();

    private void FixedUpdate() =>
        Animate();

    private void Animate()
    {
        Vector2 targetVelocity = Player.Movement.MoveDirection * Player.Movement.TargetSpeed;
        _animVelocity = Vector3.Lerp(_animVelocity, targetVelocity, 0.25f);
        _animator.SetFloat("VelocityX", _animVelocity.x);
        _animator.SetFloat("VelocityZ", _animVelocity.y);
    }
}
