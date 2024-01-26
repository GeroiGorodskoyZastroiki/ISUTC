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

        //{
        //    var lerptX = 1/Mathf.Abs(transform.position.x - aimTransform.position.x);
        //    var lerptY = 1/Mathf.Abs(transform.position.y - aimTransform.position.y);
        //    var lerptZ = 1/Mathf.Abs(transform.position.z - aimTransform.position.z);
        //    Debug.Log(lerptX);
        //    //Debug.Log(lerptY);
        //    //Debug.Log(lerptZ);
        //    var newX = Mathf.Lerp(transform.position.x, aimTransform.position.x, lerptX);
        //    var newY = Mathf.Lerp(transform.position.y, aimTransform.position.y, lerptY);
        //    var newZ = Mathf.Lerp(transform.position.z, aimTransform.position.z, lerptZ);
        //    transform.position = new Vector3(newX, newY, newZ);
        //}   
    }
}
