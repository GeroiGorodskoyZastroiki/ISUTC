using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public Transform AimTransform;
    private bool _focus = true;

    private void OnApplicationFocus(bool focus)
    {
        _focus = focus;
    }

    private void Update()
    {
        Player owner = GetComponentInParent<Player>();
        if (owner && AimTransform && _focus) transform.position = Vector3.Lerp(transform.position, AimTransform.position, 0.1f);
    }
}
