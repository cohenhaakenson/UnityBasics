using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointBehavior : MonoBehaviour
{
    static private AstroBehavior astro = null;
    public static void setAstro(AstroBehavior a) { astro = a; }
    static private GameManager gm = null;
    public static void setGM(GameManager g) { gm = g; }
    public CamSupport waypointCam;
    private ShakePosition shake = new ShakePosition(10f, 1f); // Oscillate for 5 cycles, in 0.5 seconds
    public float freq = 10f;
    public float duration = 1f;
    public int magX = 1;
    public int magY = 1;
    public static bool waypointCamState = false; // inactive
    public static bool activeShake = false;
    private Vector3 currStartPos;
    private int timesHit = 0;
    private float homeX;
    private float homeY;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(waypointCam != null);

        currStartPos = transform.localPosition;
        homeX = transform.localPosition.x;
        homeY = transform.localPosition.y;

        waypointCam.theCam.enabled = waypointCamState;
        Debug.Log("Waypoint cam enable: " + waypointCamState);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 pos = transform.localPosition;
        

        if (!shake.ShakeDone())
        {
            transform.position = shake.UpdateShake();
            if (shake.ShakeDone())
            {
                Debug.Log("Turn off waypoint cam");
                activeShake = false;
                waypointCamState = false;
                waypointCam.theCam.enabled = waypointCamState;
                Debug.Log("shaking: " + activeShake);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided with: " + collision);
        // if collides with projectile destroy projectile and update
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);

            updatePlanet();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("collided with: " + collision);
        // if collides with projectile destroy projectile and update
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);

            updatePlanet(); 
        }
    }

    private void updatePlanet()
    {
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        if (timesHit < 3)
        {
            timesHit++;
            // Change color
            Color c = s.color;
            const float delta = 0.25f;
            c.a -= delta;
            s.color = c;
            shakeWaypoint();
        }
        else
        {
            // Respawn at new random spot
            if (!activeShake)
            {
                Vector3 newPosition = new Vector3(Random.Range(homeX - 15f, homeX + 15f), Random.Range(homeY - 15f, homeY + 15f), 0);
                this.transform.localPosition = newPosition;
                currStartPos = transform.localPosition;
            }

            Color c = s.color;
            const float delta = 0.75f;
            c.a += delta;
            s.color = c;
            // reset times hit 
            timesHit = 0;
        }
    }

    private void shakeWaypoint()
    {
        Debug.Log("Turn on waypoint cam");
        if(!activeShake)
        {
            activeShake = true;
            waypointCamState = true;
            waypointCam.theCam.enabled = waypointCamState;
            waypointCam.Move(currStartPos.x, currStartPos.y);
        }
        
        shake.SetShakeParameters(freq*timesHit, duration*timesHit);
        shake.SetShakeMagnitude(new Vector2(magX*timesHit, magY*timesHit), currStartPos);
        Debug.Log("shaking: " + activeShake);
    }

    public string getWaypointState()
    {
        if (waypointCamState)
            return "Active";
        else
            return "Shut Off";
    }
}
