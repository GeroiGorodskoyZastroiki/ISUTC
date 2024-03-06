using UnityEngine;

public class AimPoint : MonoBehaviour
{
    private void Update()
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, 9))
            transform.position = hit.point;
    }
}
