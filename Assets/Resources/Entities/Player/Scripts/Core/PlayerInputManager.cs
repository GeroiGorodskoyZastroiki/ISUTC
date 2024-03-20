using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerInputManager : MonoBehaviour
{
    #region Data
    [field: ReadOnly][field: SerializeField] public float MouseSensitivity { get; private set; }
    public Vector2 MousePosition { get; private set; }
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion

    private void Start()
    {
        MouseSensitivity = PlayerPrefs.HasKey("MouseSensitivity") ? PlayerPrefs.GetFloat("MouseSensitivity") : 1;
    }

    private void OnEnable() =>
        GetComponent<PlayerInput>().enabled = true;

    public void OnMove(InputAction.CallbackContext context) => //обновляется ТОЛЬКО при нажатии клавиш(не в Update)
        Player.Movement.RawMoveDirection = context.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext context) =>
        Player.Camera.LookDelta = context.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!Player.Movement.Crouch) Player.Movement.Sprint = context.action.IsPressed();
        else Player.Movement.Sprint = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    { if (context.performed) Player.Movement.Crouch = !Player.Movement.Crouch; }

    public void OnPickUp(InputAction.CallbackContext context)
        { if (context.performed) Player.Interaction.PickUp(); }

    public void OnFlashlight(InputAction.CallbackContext context)
    { if (context.performed) Player.Network.IsFlashlightOn.Value = !Player.Network.IsFlashlightOn.Value; }

    public void OnMousePosition(InputAction.CallbackContext context) =>
        MousePosition = context.ReadValue<Vector2>();

    public void OnShowHUD(InputAction.CallbackContext context) =>
        Player.HUD.IsHUDShown = context.action.IsPressed();

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!UIManager.PauseMenu.GetComponent<PauseMenuUI>().Pause) UIManager.Open(UIManager.PauseMenu);
            else UIManager.Close(UIManager.PauseMenu);
        }
    }

    public void OnVoiceChat(InputAction.CallbackContext context) =>
        SteamUser.VoiceRecord = context.performed;
}
