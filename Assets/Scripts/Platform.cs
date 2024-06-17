using System;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    const int engineCount = 4;
    [Header("Engines")]
    [SerializeField] Engine engineFL;
    [SerializeField] Engine engineFR;
    [SerializeField] Engine engineRL;
    [SerializeField] Engine engineRR;
    Engine[] engines;

    [Space]
    [Header("Throttle")]
    public float throttle = 0;
    public float thDelta = 0.01f;
    public List<float> thDistrib = new(engineCount) {1.0f, 1.0f, 1.0f, 1.0f};
    
    [Space]
    [Header("Gyroscope")]
    public Gyroscope gyroscope;

    [Space]
    [Header("Mouse Control")]
    public float mouseSens = 0.1f;
    public Vector2 mousePos = new Vector2(0, 0);

    [Space]
    [Header("PID Regulation")]
    public float maxAngle = 30;
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 rotatDest;
    [Space]
    public float P = 0;
    public float I = 0;
    public float D = 0;
    public float rollTh = 0;
    public float pitchTh = 0;
    public float[] rollDelta = {0, 0};
    public float[] pitchDelta = {0, 0};

    [Space]
    [Header("Yaw")]
    public float maxYawSpeed = 0.5f;
    public Vector3 up;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engines = new Engine[engineCount] {engineFL, engineFR, engineRL, engineRR};
        thDistrib = new(engineCount) {1.0f, 1.0f, 1.0f, 1.0f};
    }

    void FixedUpdate()
    {
        mousePos = new Vector2(
            Input.mousePosition.x / Screen.width * 2 - 1,
            Input.mousePosition.y / Screen.height * 2 - 1
        );
        // mousePos = new Vector2(
        //     Input.GetAxis("Mouse X"),
        //     Input.GetAxis("Mouse Y")
        // );
        if (mousePos.magnitude > 1)
            mousePos.Normalize();

        rotation.x = Math.Sign(90 - Vector3.Angle(gyroscope.transform.up, transform.forward)) * Vector3.Angle(gyroscope.transform.forward, transform.forward);
        rotation.y = 0;
        rotation.z = Math.Sign(90 - Vector3.Angle(gyroscope.transform.up, transform.right)) * Vector3.Angle(gyroscope.transform.right, transform.right);
        rotatDest = new Vector3(-mousePos.y * maxAngle, 0, -mousePos.x * maxAngle);

        rollDelta[0] = rotatDest.z - rotation.z;
        pitchDelta[0] = rotatDest.x - rotation.x;

        rollTh = -(P * rollDelta[0] + D * ((rollDelta[0] - rollDelta[1]) / Time.fixedDeltaTime));
        pitchTh = -(P * pitchDelta[0] + D * ((pitchDelta[0] - pitchDelta[1]) / Time.fixedDeltaTime));

        rollDelta[1] = rollDelta[0];
        pitchDelta[1] = pitchDelta[0];

        Vector2 controlVec = new Vector2(rollTh, pitchTh);
        if (controlVec.magnitude > 1)
            controlVec.Normalize();

        thDistrib[0] = 1 - (controlVec.magnitude * Vector2.Dot(controlVec.normalized, new Vector2(-1, 1).normalized));
        thDistrib[1] = 1 - (controlVec.magnitude * Vector2.Dot(controlVec.normalized, new Vector2(1, 1).normalized));
        thDistrib[2] = 1 - (controlVec.magnitude * Vector2.Dot(controlVec.normalized, new Vector2(-1, -1).normalized));
        thDistrib[3] = 1 - (controlVec.magnitude * Vector2.Dot(controlVec.normalized, new Vector2(1, -1).normalized));

        // thDistrib[0] = 1 - (mousePos.magnitude * mouseSens * Vector2.Dot(mousePos.normalized, new Vector2(-1, 1).normalized));
        // thDistrib[1] = 1 - (mousePos.magnitude * mouseSens * Vector2.Dot(mousePos.normalized, new Vector2(1, 1).normalized));
        // thDistrib[2] = 1 - (mousePos.magnitude * mouseSens * Vector2.Dot(mousePos.normalized, new Vector2(-1, -1).normalized));
        // thDistrib[3] = 1 - (mousePos.magnitude * mouseSens * Vector2.Dot(mousePos.normalized, new Vector2(1, -1).normalized));

        up = transform.up;
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(new Vector3(0, 1, 0), maxYawSpeed * Time.fixedDeltaTime);
            
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(new Vector3(0, 1, 0), -maxYawSpeed * Time.fixedDeltaTime);

        float newthrottle = throttle + thDelta * Input.mouseScrollDelta.y;
        throttle = newthrottle > 1 ? 1 : newthrottle < 0 ? 0 : newthrottle;
        float appliedThrottle = throttle;

        if (Input.GetKey(KeyCode.Space))
            appliedThrottle = 1;

        if (Input.GetKey(KeyCode.LeftShift))
            appliedThrottle = 0;

        for (int i = 0; i < engineCount; i ++)
            engines[i].Throttle = appliedThrottle * thDistrib[i];

        foreach (var engine in engines)
            rb.AddForceAtPosition(engine.Force, engine.transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < engineCount; i++)
            Gizmos.DrawRay(engines[i].transform.position, engines[i].transform.up * engines[i].Throttle * 0.1f);
    }
}
