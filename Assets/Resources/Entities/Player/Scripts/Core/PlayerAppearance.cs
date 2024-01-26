using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    #region Data
    [RequiredListLength(4)][field: SerializeField] public List<GameObject> Characters { get; private set; }
    #endregion

    #region References
    [HideInInspector] public Player Player;
    #endregion
}
