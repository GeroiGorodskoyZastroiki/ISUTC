using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Data
    public float SprintSpeed => _sprintSpeed;
    public float Stamina => _stamina;
    public float MaxStamina => _maxStamina;
    public float TargetSpeed => _targetSpeed;

    [ReadOnly] public Vector2 RawMoveDirection;
    [ReadOnly] public Vector2 MoveDirection;
    [ReadOnly] public Vector2 AnimMoveDirection;

    [Header("Speed")]
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 8f;
    [ReadOnly][SerializeField] private float _targetSpeed;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 15f;
    [SerializeField] private float _staminaDecSpeed = 1f;
    [SerializeField] private float _staminaRegSpeed = 0.5f;
    [SerializeField] private float _staminaDecBreakpoint = 1f;
    [SerializeField] private float _staminaRegBreakpoint = 4f;
    [ReadOnly][SerializeField] private float _stamina;

    [Header("Modes")]
    [ReadOnly] public bool Sprint;
    [ReadOnly] public bool Crouch;

    [Header("Fall & Ground")]
    [SerializeField] private float _fallAcceleration = -15.0f; //The character uses its own gravity value. The engine default is -9.81f
    [ReadOnly] public bool Grounded = true;
    [ReadOnly][SerializeField] private LayerMask _levelLayerMask; //What layers the character uses as ground. PLAYER MUST BE AT THE DIFFERENT LAYER

    private const float _fallTimeout = 0.15f; //Time required to pass before entering the fall state. Useful for walking down stairs
    private readonly float _maxVerticalVelocity = 50.0f;
    private float _fallTimeoutDelta;
    private readonly float _groundedOffset = -0.1f; //Useful for rough ground -0.1
    private readonly float _groundedRadius = 0.5f; //The radius of the grounded check. Should match the radius of the CharacterController 0.5
    private float _verticalVelocity;
    private Vector3 _moveVelocity;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    private CharacterController _controller;
    private CapsuleCollider _collider;
    #endregion

    private void OnEnable()
    {
        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<CapsuleCollider>();
        _controller.enabled = true;
    }

    private void Start()
    {
        _levelLayerMask = LayerMask.GetMask("StaticGeometry");
        _fallTimeoutDelta = _fallTimeout;
        _stamina = _maxStamina;
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        //ChangeColliders();
        ChangeCollidersFixed();
    }

    private void ChangeColliders()
    {
        Bounds bounds = Player.Appearance.Characters[Player.Network.Skin.Value].GetComponent<SkinnedMeshRenderer>().bounds;
        _collider.height = _controller.height = bounds.max.y;
        _collider.center = _controller.center = new Vector3(0, bounds.center.y, 0);
    }

    private void ChangeCollidersFixed()
    {
        _collider.height = _controller.height = Crouch ? 1.2f : 1.8f;
        _collider.center = _controller.center = new Vector3(0, Crouch ? 0.6f : 0.9f, 0);
    }

    private void Move()
    {
        bool[] CheckCollisionWithObstacle(float detectionOffset)
        {
            float capsuleRadius = _collider.radius / 2;
            Vector3 capsuleBottomPoint = transform.position + new Vector3(0, _controller.stepOffset * 2, 0f);
            Vector3 capsuleTopPoint = transform.position + new Vector3(0, _collider.height, 0f);
            Vector3 directionX = transform.rotation * new Vector3(RawMoveDirection.x, 0f, 0f);
            Vector3 directionZ = transform.rotation * new Vector3(0f, 0f, RawMoveDirection.y);
            float maxDistance = _collider.radius - capsuleRadius + detectionOffset;
            bool hitX = Physics.CapsuleCast(capsuleBottomPoint, capsuleTopPoint, capsuleRadius, directionX, maxDistance, _levelLayerMask);
            bool hitZ = Physics.CapsuleCast(capsuleBottomPoint, capsuleTopPoint, capsuleRadius, directionZ, maxDistance, _levelLayerMask);
            //Debug.Log($"MoveDirection: {MoveDirection.x} {MoveDirection.y}");
            //Debug.Log($"CheckHit: {hitX} {hitZ}");
            return new bool[2] { hitX, hitZ };
        }

        float ChooseSpeed()
        {
            if (MoveDirection == Vector2.zero) return 0f;
            if (Crouch) return _crouchSpeed;
            if (Sprint && (_targetSpeed == _sprintSpeed))
            {
                if (MoveDirection.y < 0 || _stamina < _staminaDecBreakpoint) return _walkSpeed;
                return _sprintSpeed;
            }

            if (Sprint && (_targetSpeed == _walkSpeed))
            {
                if (MoveDirection.y < 0 || _stamina < _staminaRegBreakpoint) return _walkSpeed;
                return _sprintSpeed;
            }
            return _walkSpeed;
        }

        bool[] geometryHits = CheckCollisionWithObstacle(0.1f);
        AnimMoveDirection = new Vector2(geometryHits[0] ? 0f : RawMoveDirection.x, geometryHits[1] ? 0f : RawMoveDirection.y);
        if (RawMoveDirection.x == 0 || RawMoveDirection.y == 0)
            MoveDirection = AnimMoveDirection;
        else MoveDirection = RawMoveDirection;

        Vector3 targetDirection = new Vector3(MoveDirection.x, 0.0f, MoveDirection.y).normalized;
        if (MoveDirection != Vector2.zero) targetDirection = transform.right * MoveDirection.x + transform.forward * MoveDirection.y;

        _targetSpeed = ChooseSpeed();
        Vector3 targetVelocity = targetDirection.normalized * _targetSpeed;
        _moveVelocity = Vector3.Lerp(_moveVelocity, targetVelocity, 0.25f).Round() + new Vector3(0.0f, _verticalVelocity, 0.0f);
        _controller.Move(_moveVelocity * Time.deltaTime);

        if (_targetSpeed == _sprintSpeed) _stamina = Mathf.Clamp(_stamina - Time.deltaTime * _staminaDecSpeed, 0, _maxStamina);
        else _stamina = Mathf.Clamp(_stamina + Time.deltaTime * _staminaRegSpeed, 0, _maxStamina);
    }

    private void CheckGround()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _levelLayerMask, QueryTriggerInteraction.Ignore);

        if (Grounded)
        {
            
            // reset the fall timeout timer
            _fallTimeoutDelta = _fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _maxVerticalVelocity) _verticalVelocity += _fallAcceleration * Time.deltaTime;
    }
}
