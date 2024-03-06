using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    #region Data
    [Required] public Transform Cam;
    [Required][SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _topClamp = 90.0f;
    [SerializeField] private float _bottomClamp = -90.0f;
    [SerializeField] private bool _cameraBob = true;

    private const float _cameraRotationThreshold = 0.01f;
    public float CameraPitch {  get; private set; }

    [FormerlySerializedAs("_lookDelta")] public Vector2 LookDelta;
    private float _rotationVelocity;
    [SerializeField] private float _rotationSpeed = 1.0f;
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion

    private void OnEnable()
    {
        Cam.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (_cameraBob)
        {
            Cam.position = _cameraRoot.position;
        }
        // if there is an input
        if (LookDelta.sqrMagnitude >= _cameraRotationThreshold)
        {
            _rotationVelocity = LookDelta.x * _rotationSpeed * Player.Input.MouseSensitivity;
            CameraPitch += LookDelta.y * _rotationSpeed * Player.Input.MouseSensitivity;

            // clamp our pitch rotation
            CameraPitch = Mathf.Clamp(CameraPitch, _bottomClamp, _topClamp);

            // Update camera pitch
            Cam.localRotation = Quaternion.Euler(CameraPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
        //Debug.Log(CameraPitch);
    }
}
