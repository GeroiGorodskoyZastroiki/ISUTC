using UnityEngine;

public class EnemyFootsteps : Footsteps
{
    private void OnTriggerEnter(Collider other) => TakeStep(other.transform);

    public override void TakeStep(Transform floor) => StartCoroutine(PlayFootstepSound(1, floor));
}
