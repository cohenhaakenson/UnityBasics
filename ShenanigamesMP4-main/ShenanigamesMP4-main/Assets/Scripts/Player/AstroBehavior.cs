using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroBehavior : MonoBehaviour
{
    
    public bool followMouse = true; // control toggle
    public float astroSpeed = 20f;
    public float astroRotateSpeed = 90f / 2f; // 90 degree in 2 sec

    private int numDestroyed = 0;
    private int collisions = 0;

    public float spawnPeriod = 0.2f; // projectile per .2 second
    //private float nextSpawn;
    public GameObject projectilePrefab;
    public CoolDownBar coolDown;

    playModeStatus pStatus = playModeStatus.MouseControl;

    public CamSupport heroCam;

    public enum playModeStatus
    {
        MouseControl,
        KeyboardControl
    }

    //private int projCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Astro";
        coolDown.SetCoolDownLength(spawnPeriod);

        Debug.Assert(projectilePrefab != null);
        Debug.Assert(coolDown != null);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p = transform.localPosition;

        //HeroCam 
        heroCam.SetLerpParameters(0.5F, 8F); // 0.5 sec duration, TimeLerp rate of 8 
        heroCam.MoveTo(transform.position.x, transform.position.y);

        // if M is pressed toggle control with mouse
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (followMouse)
            {
                pStatus = playModeStatus.KeyboardControl;
            }
            else
            {
                pStatus = playModeStatus.MouseControl;
            }
            followMouse = !followMouse;
        }

        if (followMouse) // astro follows mouse
        {
            p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f; // important that z position is 0

            // Rotates with A/D or Left/Right arrows
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(transform.forward, astroRotateSpeed * Time.smoothDeltaTime);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(transform.forward, -astroRotateSpeed * Time.smoothDeltaTime);
            }
        }
        else // astro is controlled by key input
        {
            // astro is moving up at astroSpeed
            p += ((astroSpeed * Time.smoothDeltaTime) * transform.up);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                astroSpeed++;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                astroSpeed--;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(transform.forward, astroRotateSpeed * Time.smoothDeltaTime);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(transform.forward, -astroRotateSpeed * Time.smoothDeltaTime);
            }
        }

        // Spawn Projectile with coolDownBar
        if (Input.GetKey(KeyCode.Space))
        {
            if (coolDown.ReadyForNext()) // Spawn Projectile
            {
                //GameObject projectile = Instantiate(Resources.Load("Prefabs/Projectile") as GameObject);
                GameObject projectile = Instantiate(projectilePrefab, GameManager.globalBehavior.projectileParent.transform);
                projectile.transform.localPosition = transform.localPosition;
                projectile.transform.localRotation = transform.localRotation;
                //HeroCam shake
                if (inHeroCam())
                {
                    heroCam.SetShakeParameters(1, 1);
                    heroCam.ShakeCamera(new Vector2(1, 1));
                } else
                {
                    heroCam.SetLerpParameters(0.5F, 8F); // 0.5 sec duration, TimeLerp rate of 8 
                    heroCam.MoveTo(transform.position.x, transform.position.y);
                }
                Debug.Log("Spawn Projectile:" + projectile.transform.localPosition);
                //projCount++;

                coolDown.TriggerCoolDown();
            }
        }

        transform.localPosition = p;

        // Player bounds 
        CamSupport s = Camera.main.GetComponent<CamSupport>();
        if (s != null)
        {
            Bounds pBound = GetComponent<Renderer>().bounds; // player bounds
            CamSupport.WorldBoundStatus status = s.CollideWorldBound(pBound);

            if (status != CamSupport.WorldBoundStatus.Inside) // if hits bounds
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }

    
    // returns string of current projectile status
    public string projStatus() 
    { //return "Current projectiles: " + projCount; 
        return $"Current projectiles: {GameManager.globalBehavior.projectileParent.transform.childCount}";
    }

    // returns string of current play mode
    public string getPlayMode()
    {
        if (pStatus == playModeStatus.MouseControl)
        {
            return "Player Mode: Mouse";
        }
        else
        {
            return "Player Mode: Keyboard";
        }
    }
    // Increments alien destroyed count for a collision with astro
    public void alienCollision() { collisions++; }
    // returns string of collision count
    public string collisonStatus() { return "Player Hits: " + collisions; }
    // Increments destroyed count
    public void destroyAlien() { numDestroyed++; }
    // returns string of destroyed count
    public string destroyedStatus() { return "Destroyed Enemies: " + numDestroyed; }

    //True if the hero is inside the HeroCam
    public bool inHeroCam()
    {
        Camera cam = heroCam.theCam;
        float yOffset = cam.orthographicSize;
        float xOffset = yOffset * cam.aspect;
        float dist = transform.position.x - cam.transform.position.x;
        if (dist < 0)
        {
            dist *= -1;
        }
        if (dist > xOffset)
        {
            return false;
        }
        dist = transform.position.y - cam.transform.position.y;
        if (dist < 0)
        {
            dist *= -1;
        }
        if (dist > yOffset)
        {
            return false;
        }
        return true;
    }
}
