using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;

public class Player : NetworkBehaviour
{
    #region CameraData
    [FoldoutGroup("Camera")] public Transform Cam;
    [FoldoutGroup("Camera")][SerializeField] Transform _cameraRoot;
    [FoldoutGroup("Camera")][SerializeField] float _topClamp = 90.0f;
    [FoldoutGroup("Camera")][SerializeField] float _bottomClamp = -90.0f;
    [FoldoutGroup("Camera")][SerializeField] bool _cameraBob = true;
    const float CameraRotationThreshold = 0.01f;
    float _cameraPitch;
    #endregion

    #region MovementData
    public float SprintSpeed { get { return _sprintSpeed; } }
    public float Stamina { get { return _stamina; } }
    public float TargetSpeed { get { return _targetSpeed; } }

    [Header("Speed")]
    [FoldoutGroup("Movement")][SerializeField] float _walkSpeed = 4f;
    [FoldoutGroup("Movement")][SerializeField] float _sprintSpeed = 8f;
    [FoldoutGroup("Movement")][SerializeField] float _rotationSpeed = 1.0f;
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] float _mouseSensitivity;
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] float _targetSpeed;

    //[HorizontalLine(1.5f, EColor.Gray)]
    [Header("Sprint")]
    [FoldoutGroup("Movement")][SerializeField] float _maxStamina = 15f;
    [FoldoutGroup("Movement")][SerializeField] float _staminaDecSpeed = 1f;
    [FoldoutGroup("Movement")][SerializeField] float _staminaRegSpeed = 0.5f;
    [FoldoutGroup("Movement")][SerializeField] float _staminaDecBreakpoint = 1f;
    [FoldoutGroup("Movement")][SerializeField] float _staminaRegBreakpoint = 4f;
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] float _stamina;
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] bool _sprint;

    //[HorizontalLine(1.5f, EColor.Gray)]
    [Header("Fall & Ground")]
    [FoldoutGroup("Movement")][SerializeField] float _fallAcceleration = -15.0f; //The character uses its own gravity value. The engine default is -9.81f
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] bool _grounded = true;
    [FoldoutGroup("Movement")][ReadOnly][SerializeField] LayerMask _levelLayerMask = 1; //What layers the character uses as ground. PLAYER MUST BE AT THE DIFFERENT LAYER
    const float _fallTimeout = 0.15f; //Time required to pass before entering the fall state. Useful for walking down stairs
    float _maxVerticalVelocity = 50.0f;
    float _fallTimeoutDelta;
    float _groundedOffset = -0.1f; //Useful for rough ground
    float _groundedRadius = 0.5f; //The radius of the grounded check. Should match the radius of the CharacterController
    float _verticalVelocity;

    Vector2 _mousePosition;
    Vector2 _moveDirection;
    Vector3 _moveVelocity;
    Vector2 _animVelocity;
    Vector2 _lookDelta;
    float _rotationVelocity;
    #endregion

    #region GameplayData
    public float PickUpDistance { get; private set; }
    #endregion

    #region AppearenceData
    [FoldoutGroup("Appearance")][SerializeField] List<GameObject> characters;
    #endregion

    #region NetworkData
    public Friend owner;
    [FoldoutGroup("Network")][ReadOnly] public NetworkVariable<ulong> steamId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [FoldoutGroup("Network")] public NetworkVariable<int> skin = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [FoldoutGroup("Network")] public NetworkVariable<bool> flashlightIsOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [FoldoutGroup("Network")] public NetworkVariable<bool> ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    #endregion

    #region References
    Animator _animator;
    HUDManager _hudManager;
    CharacterController _controller;
    #endregion

    void OnEnable()
    {
        Cam.gameObject.SetActive(true);
        GetComponent<PlayerInput>().enabled = true;
        GetComponentInChildren<AimPoint>().enabled = true;
        GetComponentInChildren<HUDManager>(true).gameObject.SetActive(true);
        _controller = GetComponent<CharacterController>();
        _controller.enabled = true;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _hudManager = GetComponentInChildren<HUDManager>();

        _mouseSensitivity = PlayerPrefs.HasKey("MouseSensitivity") ? PlayerPrefs.GetFloat("MouseSensitivity") : 1;
        _fallTimeoutDelta = _fallTimeout; //reset our timeouts on start
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        Animate();
    }

    void LateUpdate()
    {
        MoveCamera();
    }

    void Animate()
    {
        Vector2 targetVelocity = _moveDirection * _targetSpeed;
        _animVelocity = Vector3.Lerp(_animVelocity, targetVelocity, 0.25f);//.Round();
        _animator.SetFloat("VelocityX", _animVelocity.x);
        _animator.SetFloat("VelocityZ", _animVelocity.y);
    }

    #region Gameplay
    void PickUp()
    {
        var ray = Camera.main.ScreenPointToRay(_mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            hit.transform.gameObject.TryGetComponent<Item>(out var item);
            if (item && hit.distance < PickUpDistance) item.ItemDespawnServerRpc();
        }
    }
    #endregion

    #region Network
    public override void OnNetworkSpawn()
    {
        StartCoroutine(Spawn());
        skin.OnValueChanged += OnSkinChanged;
        UpdateSkin();
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.players.Remove(gameObject);
        //if (NetworkObject.IsLocalPlayer) GameManager.Instance.MakePlayerSpectator();

        if (!GameManager.Instance.gameStarted)
        {
            var PLUI = FindObjectsByType<PlayerLobbyUI>(FindObjectsSortMode.None).SingleOrDefault(x => x.player == gameObject);
            if (PLUI) Destroy(PLUI.gameObject);
        }
        base.OnNetworkDespawn();
    }

    IEnumerator Spawn()
    {
        transform.position = GameObject.FindGameObjectsWithTag("LobbySpawnPoint")[OwnerClientId].transform.position;
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Banka"));
        if (IsOwner)
        {
            GameManager.Instance.owner = gameObject;
            steamId.Value = SteamClient.SteamId.Value;
        }
        else
        {
            steamId.OnValueChanged += OnSteamIdChanged;
            UpdateSteamId();
        }
        yield return new WaitForSeconds(2);
        UIManager.lobby.GetComponent<LobbyUI>().CreatePlayerInfo(gameObject);
        yield break;
    }

    void OnSteamIdChanged(ulong prev, ulong curr)
    {
        steamId.Value = curr;
    }

    void UpdateSteamId() => OnSteamIdChanged(steamId.Value, steamId.Value);

    void OnSkinChanged(int prev, int curr)
    {
        characters.ForEach(x => x.SetActive(false));
        characters[curr].SetActive(true);
    }

    void UpdateSkin() => OnSkinChanged(skin.Value, skin.Value);
    #endregion

    #region Controller
    void MoveCamera()
    {
        if (_cameraBob)
        {
            Cam.position = _cameraRoot.position;
        }
        // if there is an input
        if (_lookDelta.sqrMagnitude >= CameraRotationThreshold)
        {
            _rotationVelocity = _lookDelta.x * _rotationSpeed * _mouseSensitivity;
            _cameraPitch += _lookDelta.y * _rotationSpeed * _mouseSensitivity;

            // clamp our pitch rotation
            _cameraPitch = Mathf.Clamp(_cameraPitch, _bottomClamp, _topClamp);

            // Update camera pitch
            Cam.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    void Move()
    {
        bool CheckCollisionWithObstacle()
        {
            Vector3 capsuleBottomPoint = transform.position + new Vector3(0, _controller.stepOffset + 0.01f, 0f);
            Vector3 capsuleTopPoint = transform.position + new Vector3(0, 1.8f, 0f);
            Vector3 directionX = transform.rotation * new Vector3(_moveDirection.x, 0f, 0f);
            Vector3 directionZ = transform.rotation * new Vector3(0f, 0f, _moveDirection.y);
            bool hitX = Physics.CapsuleCast(capsuleBottomPoint, capsuleTopPoint, 0.45f, directionX, 0.25f, _levelLayerMask);
            bool hitZ = Physics.CapsuleCast(capsuleBottomPoint, capsuleTopPoint, 0.45f, directionZ, 0.25f, _levelLayerMask);
            Debug.Log($"CheckHit: { hitX } { hitZ }");
            return hitX || hitZ;
        }

        float ChooseSpeed()
        {
            if (_moveDirection == Vector2.zero) return 0f;
            else
            {
                if (_sprint && (_targetSpeed == _sprintSpeed))
                {
                    if (_moveDirection.y < 0 || _stamina < _staminaDecBreakpoint) return _walkSpeed;
                    else return _sprintSpeed;
                }
                else if (_sprint && (_targetSpeed == _walkSpeed))
                {
                    if (_moveDirection.y < 0 || _stamina < _staminaRegBreakpoint) return _walkSpeed;
                    else return _sprintSpeed;
                }
                else return _walkSpeed;
            }
        }

        if (CheckCollisionWithObstacle())
        {
            _moveVelocity = Vector2.zero;
            _targetSpeed = 0.0f;
            return;
        }

        Vector3 targetDirection = new Vector3(_moveDirection.x, 0.0f, _moveDirection.y).normalized;
        if (_moveDirection != Vector2.zero) targetDirection = transform.right * _moveDirection.x + transform.forward * _moveDirection.y;

        _targetSpeed = ChooseSpeed();
        Vector3 targetVelocity = targetDirection.normalized * _targetSpeed;
        _moveVelocity = Vector3.Lerp(_moveVelocity, targetVelocity, 0.25f).Round() + new Vector3(0.0f, _verticalVelocity, 0.0f);
        _controller.Move(_moveVelocity * Time.deltaTime);

        if (_targetSpeed == _sprintSpeed) _stamina = Mathf.Clamp(_stamina - Time.deltaTime * _staminaDecSpeed, 0, _maxStamina);
        else _stamina = Mathf.Clamp(_stamina + Time.deltaTime * _staminaRegSpeed, 0, _maxStamina);
    }

    void CheckGround()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _levelLayerMask, QueryTriggerInteraction.Ignore);

        if (_grounded)
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
    #endregion

    #region Inputs
    public void OnMove(InputAction.CallbackContext context) => 
        _moveDirection = context.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext context) => 
        _lookDelta = context.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext context) => 
        _sprint = context.action.IsPressed();

    public void OnPickUp(InputAction.CallbackContext context) 
        { if (context.performed) PickUp(); }

    public void OnFlashlight(InputAction.CallbackContext context) 
        { if (context.performed) flashlightIsOn.Value = !flashlightIsOn.Value; }

    public void OnMousePosition(InputAction.CallbackContext context) => 
        _mousePosition = context.ReadValue<Vector2>();

    public void OnShowHUD(InputAction.CallbackContext context) => 
        _hudManager.showHUD = context.action.IsPressed();

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!UIManager.pauseMenu.GetComponent<PauseMenuUI>().pause) UIManager.Open(UIManager.pauseMenu);
            else UIManager.Close(UIManager.pauseMenu);
        }
    }

    public void OnVoiceChat(InputAction.CallbackContext context)
    {
        if (context.performed) SteamUser.VoiceRecord = true;
        else SteamUser.VoiceRecord = false;
    }

    public void OnTestKey(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log(SteamAudio.SteamAudioManager.Singleton.mListener.gameObject.name);
    }
    #endregion
}
