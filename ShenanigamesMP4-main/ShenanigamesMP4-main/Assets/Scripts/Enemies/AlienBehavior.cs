using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AlienBehavior : MonoBehaviour
{
    #region Initialize Variables
    static private AstroBehavior astro = null;
    public static void setAstro(AstroBehavior a) { astro = a; }
    static private GameManager gm = null;
    public static void setGM(GameManager g) { gm = g; }
    // waypoint
    public WayPointBehavior theTarget = null;
    private int currentSequence = 0;

    //added for enemy cam functionality-elygh
    public int currentID = -1;
    private List<GameObject> enemies;
    private enemyCamera cs;
    private bool isInArray = false;

    // idle movement values;
    private float rotateSpeed = -1;
    private float radius = 1f;
    private Vector3 center;
    private float angle;

    // patrol movement values 
    private float moveSpeed = 5f; // Set to 5 for testing
    private float turnRate = 0.5f;
    private float distWaypoint = 25f;

    private TimedLerp positionLerp = new TimedLerp(2f, 2f);
    private float lerpDur = 2f;
    private float lerpRate = 2f;
    public Vector3 startPos;
    public Vector3 finalPos;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        center = this.transform.position;
        rotateSpeed = Random.Range(3f, 7f);
        // get random in array
        int rand = Random.Range(0, 5);
        theTarget = gm.wps[rand];
        //theTarget = gm.A;
    }

    // Update is called once per frame
    void Update()
    {
        updateFSM(); // UPDATE FS
    }

    #region Patrol Helpers
    // Checks distance to target
    private void checkTargetDist()
    {
        // check distance to current target
        float dist = Vector3.Distance(theTarget.transform.localPosition, transform.localPosition);
        //Debug.Log(gm.wps);
        // if close enough, call getNewTarget
        if (dist < distWaypoint)
        {
            getNewTarget();
        }
    }

    // Updates target based on gameMode
    private void getNewTarget()
    {
        if (gm.currentMode == GameManager.GameMode.Sequential)
        {
            // if last in sequence, reset to 0
            if (currentSequence == 5)
            {
                currentSequence = 0;
            }
            else // else get next
            {
                currentSequence++;
            }
            theTarget = gm.wps[currentSequence];
        }
        if (gm.currentMode == GameManager.GameMode.Random)
        {
            // get random in array
            int rand = Random.Range(0, 5);
            theTarget = gm.wps[rand];
        }
    }

    // Points toward current target
    private void pointAtTarget(Vector3 pos, float rot)
    {
        Vector3 v = pos - transform.localPosition;
        transform.up = Vector3.LerpUnclamped(transform.up, v, rot);
    }
    #endregion

    #region Collision Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if collides with projectile
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (state == AlienState.aEgg)
            {
                if (isInArray)//used for enemy cam  -elygh
                {
                    enemies.RemoveAt(currentID);
                    isInArray = false;
                    for (int i = currentID; i < enemies.Count; i++)
                    {
                        enemies[i].GetComponent<AlienBehavior>().currentID--;
                    }
                    currentID = -1;
                }
                astro.destroyAlien();
                destroyAlien();
            }
            else if (state == AlienState.aStunned)
            {
                // add lerp
                finalPos = transform.localPosition; // current alien pos
                finalPos += collision.transform.up * 4; // plus 4 unit in collision.up direction
                state = AlienState.aEgg;
            }
            else
            {
                // add lerp
                startPos = transform.localPosition; // current alien pos
                finalPos = startPos + (collision.transform.up * 8); // plus 8 unit in collision.up direction
                state = AlienState.aStunned;
            }
            Destroy(collision.gameObject);

        }
        // if collides with astro
        if (collision.gameObject.CompareTag("Astro"))
        {
            if (state == AlienState.aChase)
            {
                if (isInArray)//used for enemy cam  -elygh
                {
                    enemies.RemoveAt(currentID);
                    isInArray = false;
                    for (int i = currentID; i < enemies.Count; i++)
                    {
                        enemies[i].GetComponent<AlienBehavior>().currentID--;
                    }
                    currentID = -1;
                }
                astro.alienCollision();
                destroyAlien();
            }
            if (state == AlienState.aIdle || state == AlienState.aPatrol)
            {
                state = AlienState.aCCWRotation;
                frameTracker = 0;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        // if collides with projectile
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (state == AlienState.aEgg)
            {
                if (isInArray)//used for enemy cam  -elygh
                {
                    enemies.RemoveAt(currentID);
                    isInArray = false;
                    for (int i = currentID; i < enemies.Count; i++)
                    {
                        enemies[i].GetComponent<AlienBehavior>().currentID--;
                    }
                    currentID = -1;
                }
                astro.destroyAlien();
                destroyAlien();
            }
            else if (state == AlienState.aStunned)
            {
                // add lerp
                finalPos = transform.localPosition; // current alien pos
                finalPos += collision.transform.up * 4; // plus 4 unit in collision.up direction
                state = AlienState.aEgg;
            }
            else
            {
                // add lerp
                startPos = transform.localPosition; // current alien pos
                finalPos = startPos + (collision.transform.up * 8); // plus 8 unit in collision.up direction
                state = AlienState.aStunned;
            }
            Destroy(collision.gameObject);

        }
        // if collides with astro
        if (collision.gameObject.CompareTag("Astro"))
        {
            if (state == AlienState.aChase)
            {
                if (isInArray)//used for enemy cam  -elygh
                {
                    enemies.RemoveAt(currentID);
                    isInArray = false;
                    for (int i = currentID; i < enemies.Count; i++)
                    {
                        enemies[i].GetComponent<AlienBehavior>().currentID--;
                    }
                    currentID = -1;
                }
                astro.alienCollision();
                destroyAlien();
            }
            if (state == AlienState.aIdle || state == AlienState.aPatrol)
            {
                state = AlienState.aCCWRotation;
                frameTracker = 0;
            }
        }
    }
    #endregion

    private void destroyAlien()
    {
        GameObject explosion = Instantiate(Resources.Load("Prefabs/Explosion") as GameObject);
        explosion.transform.localPosition = transform.localPosition;
        explosion.transform.localRotation = transform.localRotation;
        Destroy(this.gameObject); // destroys alien
        gm.defeatAlien(); // deincrements enemy count

    }
}
