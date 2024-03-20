using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private float _timeScale = 1f;
    public KeyCode TestButton = KeyCode.Tilde;

    void Update()
    {
        Time.timeScale = _timeScale;
        if (Input.GetKeyDown(TestButton)) OnTestKey();
    }

    public void OnTestKey()
    {
        
    }
}
