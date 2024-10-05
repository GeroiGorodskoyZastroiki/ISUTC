using UnityEngine;
using UnityEngine.Events;

public class EnemyHearing : MonoBehaviour, IHearing
{
    [field: SerializeField] public float HearingThreshold { get; private set; } = 0.08f;
}
