using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AI : MonoBehaviour
{
    #region Data
    [ReadOnly][ShowInInspector] public AIStates State;
    #endregion

    #region References
    public NavMeshAgent Agent {  get; private set; }
    public AIAnimator Animator { get; private set; }
    public AIMovement Movement { get; private set; }
    public AIDetection Detection { get; private set; }
    public AINavigation Navigation { get; private set; }
    public AIMelee Melee { get; private set; }
    #endregion

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Destroy(GetComponentInChildren<AIVision>());
            Destroy(GetComponentInChildren<EnemyHearing>());
            Destroy(this);
        }

        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<AIAnimator>();
        Movement = GetComponent<AIMovement>();
        Detection = GetComponent<AIDetection>();
        Navigation = GetComponent<AINavigation>();
        Melee = GetComponent<AIMelee>();

        Animator.AI = Movement.AI = Detection.AI = Navigation.AI = Melee.AI = this;
    }

    public void OnEnable()
    {
        if (NetworkManager.Singleton.IsHost)
            Agent.enabled = Animator.enabled = Movement.enabled = Detection.enabled = Navigation.enabled = Melee.enabled = true;
    }
}