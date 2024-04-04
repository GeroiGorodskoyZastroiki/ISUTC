using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class AIDetection : MonoBehaviour, IHearing
{
    #region Data    
    [ReadOnly][ShowInInspector] public AISenses? TargetDetectionType;
    [ReadOnly][ShowInInspector] public GameObject TargetGameObject;
    [ReadOnly][ShowInInspector] public Vector3 TargetPosition;
    [field: SerializeField] public float HearingThreshold { get; private set; }
    [SerializeField] private float _afterLossAwarenessTime;
    [ReadOnly][ShowInInspector] private bool _isLosingTarget;
    #endregion

    #region References
    [HideInInspector] public AI AI;
    #endregion

    private void Start()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        var vision = GetComponentInChildren<AIVision>();
        vision.OnVisualDetected.AddListener(OnVisualDetected);
        vision.OnVisualIndirectDetected.AddListener(OnVisualIndirectDetected);
        vision.OnVisualLost.AddListener(OnVisualLost);

        var soundSources = FindObjectsByType<AudioSourceLoudness>(FindObjectsSortMode.None);
        foreach (var source in soundSources)
            source.OnSoundDetected.AddListener(OnSoundDetected);
    }

    #region OnDetected
    private void OnVisualDetected(GameObject target)
    {
        if (target == TargetGameObject) return;
        SenseDetected(AISenses.Vision, AI.Navigation.Pursuit(), target);
    }

    private void OnVisualIndirectDetected(Vector3 position) =>
        SenseDetected(AISenses.VisionIndirect, AI.Navigation.Check(), position);
        

    private void OnSoundDetected(Vector3 position) =>
        SenseDetected(AISenses.Hearing, AI.Navigation.Check(), position);

    private void SenseDetected(AISenses newTargetDetectionType, IEnumerator State, Vector3 position)
    {
        //Debug.Log(newTargetDetectionType);
        if (TargetPosition != null)
            if (!IsTargetRelevant(newTargetDetectionType, TargetPosition, position)) return;

        TargetPosition = position;

        StartNewState(State);
    }

    private void SenseDetected(AISenses newTargetDetectionType, IEnumerator State, GameObject target)
    {
        //Debug.Log(newTargetDetectionType);
        if (TargetGameObject != null)
            if (!IsTargetRelevant(newTargetDetectionType, TargetGameObject.transform.position, target.transform.position)) return;

        TargetGameObject = target;

        StartNewState(State);
    }

    private bool IsTargetRelevant(AISenses newTargetDetectionType, Vector3 oldPosition, Vector3 newPosition)
    {
        if (TargetDetectionType < newTargetDetectionType) return false;
        if (TargetDetectionType == newTargetDetectionType)
        {
            var oldDistance = Vector3.Distance(transform.position, oldPosition);
            var newDistance = Vector3.Distance(transform.position, newPosition);
            if (oldDistance + 5f < newDistance) return false; //избавиться от маг. чисел
        }
        return true;
    }

    private void StartNewState(IEnumerator State)
    {
        StopAllCoroutines();
        _isLosingTarget = false;

        AI.Navigation.StopAllCoroutines();
        AI.Navigation.StartCoroutine(State);
    }
#endregion

    #region OnLost
    private void OnVisualLost(GameObject target)
    {
        if (TargetGameObject != target) return;
        if (TargetGameObject == target && _isLosingTarget) return;
        StartCoroutine(OnTargetLost(_afterLossAwarenessTime));
    }

    public IEnumerator OnTargetLost(float time)
    {
        _isLosingTarget = true;
        yield return new WaitForSeconds(time);
        AI.Navigation.StopAllCoroutines();
        TargetGameObject = null;
        TargetDetectionType = null;
        yield return new WaitForSeconds(2);
        AI.Navigation.StartCoroutine(AI.Navigation.Search());
        _isLosingTarget = false;
    }
    #endregion
}
