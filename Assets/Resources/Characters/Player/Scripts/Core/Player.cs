using UnityEngine;

public class Player : MonoBehaviour
{
    #region References
    public PlayerAnimator Animator { get; private set; }
    public PlayerAppearance Appearance { get; private set; }
    public PlayerCamera Camera { get; private set; }
    public PlayerInputManager Input { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerNetwork Network { get; private set; }
    public PlayerHUD HUD { get; private set; }
    #endregion

    private void OnEnable()
    {
        Animator = GetComponent<PlayerAnimator>();
        Appearance = GetComponent<PlayerAppearance>();
        Camera = GetComponent<PlayerCamera>();
        Input = GetComponent<PlayerInputManager>();
        Interaction = GetComponent<PlayerInteraction>();
        Movement = GetComponent<PlayerMovement>();
        Network = GetComponent<PlayerNetwork>();
        GetComponentInChildren<PlayerHUD>(true).gameObject.SetActive(true);
        HUD = GetComponentInChildren<PlayerHUD>();

        Animator.Player = Appearance.Player = Camera.Player = Input.Player = HUD.Player =
        Movement.Player = Interaction.Player = Movement.Player = Network.Player = this;
    }

    public void EnableComponents()
    {
        Animator.enabled = Appearance.enabled = Camera.enabled = Input.enabled = HUD.enabled =
        Movement.enabled = Interaction.enabled = Movement.enabled = Network.enabled = true;
        GetComponentInChildren<AimPoint>().enabled = true;
    }
}
