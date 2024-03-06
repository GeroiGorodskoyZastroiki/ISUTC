using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRig : MonoBehaviour
{
    #region Data
    [SerializeField] private Rig _leftArmRig;
    [SerializeField] private Rig _runningRig;
    [SerializeField] private Rig _crouchingRig;
    [SerializeField] private Rig _crouchingAimRig;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    private Animator _animator;
    #endregion

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        LeftArm();
        Running();
        Crouching();
    }

    void LeftArm()
    {
        var clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        float idleWeight = clipInfos.SingleOrDefault(x => x.clip.name == "Idle").weight;
        float crouchWeight = clipInfos.Where(x => x.clip.name.Contains("Crouching")).Select(x => x.weight).DefaultIfEmpty(0).Max() / 1.75f;
        _leftArmRig.weight = Mathf.Lerp(_leftArmRig.weight, Mathf.Max(idleWeight, crouchWeight), 0.1f);
    }

    void Running()
    {
        var clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        float runningLeftWeight = clipInfos.SingleOrDefault(x => x.clip.name == "RunningLeft").weight;
        float runningRightWeight = clipInfos.SingleOrDefault(x => x.clip.name == "RunningRight").weight;
        _runningRig.weight = Mathf.Max(runningLeftWeight, runningRightWeight);
    }

    void Crouching()
    {
        float crouchingWeight = _animator.GetCurrentAnimatorClipInfo(0).Where(x => x.clip.name.Contains("Crouching")).Select(x => x.weight).DefaultIfEmpty(0).Max();
        _crouchingRig.weight = Mathf.Clamp(crouchingWeight, 0, 0.75f);
        if (crouchingWeight < 0.5f) return;
        _crouchingAimRig.weight = Mathf.Lerp(_crouchingAimRig.weight, Mathf.Clamp01(Player.Camera.CameraPitch * -0.04f), 0.1f);
    }
}
