using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] float maxForce;

    public Vector3 Force => Vector3.RotateTowards(new Vector3(0, maxForce * throttle, 0), transform.up, 4, 0);

    float throttle = 0;

    public float Throttle
    {
        get {return throttle;}
        set
        {
            throttle = value;
            if (throttle < 0) throttle = 0;
            if (throttle > 1) throttle = 1;
        }
    }

    void Start()
    {

    }
}
