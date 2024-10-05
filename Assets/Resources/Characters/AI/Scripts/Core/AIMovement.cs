using UnityEngine;
using Sirenix.OdinInspector;

public class AIMovement : MonoBehaviour
{
    #region References
    [HideInInspector] public AI AI;
    #endregion

    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 8f;
    [ReadOnly] public bool Sprint;

    private void Update()
    {
        Move();
    }

    private void Move() =>
        AI.Agent.speed = Sprint ? _sprintSpeed : _walkSpeed;
}
