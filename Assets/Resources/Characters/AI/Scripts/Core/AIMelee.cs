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
            while (true)
            {
                if (AI.Detection.TargetGameObject)
                {
                    var distance = Vector3.Distance(transform.position, AI.Detection.TargetGameObject.transform.position);
                    //Debug.Log(AI.Detection.TargetGameObject);
                    //Debug.Log(distance);
                    if (AI.Detection.TargetGameObject && distance <= AttackDistance) break;
                    else yield return null;
                }
                else yield return null;
            }
            AI.State = AIStates.Kill;
            AI.Agent.isStopped = true;

            StartCoroutine(AI.Detection.TargetGameObject.GetComponent<Player>().Rig.Die());
            yield return new WaitForSeconds(0.25f);
            AI.Agent.isStopped = false;
            AI.Navigation.SearchMode = AI.Navigation.KillSearchMode;
            AI.Detection.StartCoroutine(AI.Detection.OnTargetLost(0f));
            yield return null;
        }
    }

}
