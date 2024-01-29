using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRig : MonoBehaviour
{
    #region Data
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private Transform _flashlight;
    [SerializeField] private Transform _flashlightPoint;
    [SerializeField] private Transform _voiceChat;
    [SerializeField] private Transform _mouth;
    [SerializeField] private Transform _mouthPoint;
    [SerializeField] private MultiRotationConstraint _leftArmConstraint;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    private Animator _animator;
    #endregion

    private void Start()
    {
        _animator = GetComponentInParent<Animator>();
        _flashlight = Player.Items.Flashlight;
    }

    private void Update()
    {
        UpdateBindedObjects();
        ConstrainLeftArm();
    }

    void UpdateBindedObjects()
    {
        _camera.position = _cameraPoint.position;
        _flashlight.SetPositionAndRotation(_flashlightPoint.position, _flashlightPoint.rotation);
        _mouth.SetPositionAndRotation(_mouthPoint.position, _mouthPoint.rotation);
        _voiceChat.SetPositionAndRotation(_mouthPoint.position, _mouthPoint.rotation);
    }

    void ConstrainLeftArm()
    {
        if ((-0.1 < _animator.GetFloat("VelocityX") && _animator.GetFloat("VelocityX") <= 1) && (-0.1 < _animator.GetFloat("VelocityZ") && _animator.GetFloat("VelocityZ") <= 1))
            _leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(_leftArmConstraint.weight, 1, 0.1f), 0, 1);
        else _leftArmConstraint.weight = Mathf.Clamp(Mathf.Lerp(_leftArmConstraint.weight, 0, 0.1f), 0, 1);
    }
}
