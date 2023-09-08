using UnityEngine;

public class AimPoint : MonoBehaviour
{
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, 9))
        {
            transform.position = hit.point;
        }
    }
}
