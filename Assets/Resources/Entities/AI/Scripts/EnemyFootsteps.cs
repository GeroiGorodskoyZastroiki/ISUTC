public class EnemyFootsteps : CharacterFootsteps
{
    public override void TakeStep() => StartCoroutine(PlayFootstepSound(1));
}
