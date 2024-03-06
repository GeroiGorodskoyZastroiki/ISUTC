using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private float _timeScale = 1f;

    void Update()
    {
        Time.timeScale = _timeScale;
    }
}
