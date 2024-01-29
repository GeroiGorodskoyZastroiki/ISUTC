using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public void Footstep() =>
        GetComponentInChildren<EnemyFootsteps>().TakeStep();
}
