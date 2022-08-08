using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCamera : MonoBehaviour
{
    public GameObject waypoint;
    public CamSupport theCam;
    public float freq = 10f;
    public float duration = 1f;
    public int magX = 1;
    public int magY = 1;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(theCam != null);
    }

    // Update is called once per frame
    void Update()
    {
        // Perform Shake
        if (Input.GetKeyDown(KeyCode.X))
        {
            theCam.SetShakeParameters(freq, duration);
            theCam.ShakeCamera(new Vector2(magX, magY));
        }
    }
}
