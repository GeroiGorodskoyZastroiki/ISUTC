using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class AIMelee : MonoBehaviour
{
    #region Data
    [SerializeField] public float AttackDistance = 1f;
    #endregion

    #region References
    [HideInInspector] public AI AI;
    #endregion

    void Start()
    {
        StartCoroutine(Kill());
    }

    private IEnumerator Kill()
    {
        while (true)
        {
            if ( AI.Detection.TargetGameObject)
            {
                var distance = Vector3.Distance(transform.position, AI.Detection.TargetGameObject.transform.position);
                if (AI.Detection.TargetGameObject && distance < AttackDistance) break;
                else yield return null;
            }
            else yield return null;
        }
        Debug.Log("Kill");
        AI.State = AIStates.Kill;
        AI.Navigation.SearchMode = AI.Navigation.KillSearchMode;

        AudioSource.PlayClipAtPoint(AI.Detection.TargetGameObject.GetComponentInChildren<PlayerMouth>().Death, AI.Detection.TargetGameObject.transform.position);
        AI.Detection.TargetGameObject.GetComponent<NetworkObject>().Despawn();
        AI.Detection.StartCoroutine(AI.Detection.OnTargetLost(0f));
    }
}
