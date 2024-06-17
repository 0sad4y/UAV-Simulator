using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    Platform platform;

    Vector3 platformUpOld;
    Vector3 platformForwardOld;
    Vector3 platformRightOld;

    void Start()
    {
        platform = transform.parent.GetComponent<Platform>();
    }

    void FixedUpdate()
    {
        transform.LookAt(
            transform.position + Vector3.ProjectOnPlane(platform.transform.forward, new Vector3(0, 1, 0)).normalized,
            Vector3.ProjectOnPlane(
                Vector3.ProjectOnPlane(platform.transform.up, new Vector3(1, 0, 0)),
                new Vector3(0, 0, 1)
                ).normalized
            );

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 0.1f);
    }
}
