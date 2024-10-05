using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    #region Data
    private Animator _animator;
    #endregion

    #region References
    [HideInInspector] public AI AI;
    #endregion

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Animate();
    }

    public void Animate()
    {
        _animator.SetBool("Kill", AI.State == AIStates.Kill);
        _animator.SetFloat("Velocity", AI.Agent.velocity.magnitude);
    }
}
