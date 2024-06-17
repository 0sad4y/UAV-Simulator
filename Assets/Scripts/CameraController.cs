using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform platform;
    [SerializeField] Vector3 ralativePosition;
    [SerializeField] Vector3 rotation;

    void Update()
    {
        transform.position = platform.position;
        transform.Rotate(new Vector3(0, platform.rotation.eulerAngles.y, 0));
    }
}
