using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;
    void LateUpdate()
    {
        cam = Camera.main.transform;
        transform.LookAt(transform.position + cam.forward);
    }
}
