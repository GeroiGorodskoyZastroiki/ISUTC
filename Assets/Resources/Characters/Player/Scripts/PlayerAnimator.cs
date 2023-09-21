using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public void Footstep() =>
        GetComponentInChildren<PlayerFootsteps>().ValidateFootstep();
}
