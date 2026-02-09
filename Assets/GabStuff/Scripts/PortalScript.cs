using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private Camera _cam;
    
    void Start()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
    }
}
