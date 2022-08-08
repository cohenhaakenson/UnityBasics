using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Finite State Machine - Cohen
 * This file implements the FSM for the enemies, described in mp4 assignment
 */
public partial class AlienBehavior : MonoBehaviour
{
    
    private enum AlienState
    {
        aPatrol,
        aIdle,
        aCCWRotation,
        aCWRotation, 
        aChase,
        aEnlarge,
        aShrink,
        aStunned,
        aEgg
    }

    private float sizeFrames = 60f; // 1 sec for size change
    private float rotateFrames = 60f; // 1 sec for rotation
    private float scaleRate = 2f / 60f; // 2 per sec
    private float rotateRate = 90f / 60f; // 90 degrees per sec
    private float distToAstro = 40f; // within 40 units of astro

    private int frameTracker = 0; // tracks frames per state

    private AlienState state = AlienState.aPatrol; // starts in patrol state
    

    // updates the aliens FSM and calls appropriate helper method
    private void updateFSM()
    {
        cs = GameObject.Find("EnemyCam").GetComponent<enemyCamera>();
        enemies = cs.enemies;

        switch (state)
        {
            case AlienState.aCCWRotation:
                serviceCCWRotation();
                break;
            case AlienState.aCWRotation:
                serviceCWRotation();
                break;
            case AlienState.aChase:
                serviceChase();
                break;
            case AlienState.aEnlarge:
                serviceEnlarge();
                break;
            case AlienState.aShrink:
                serviceShrink();
                break;
            case AlienState.aStunned:
                serviceStunned();
                break;
            case AlienState.aEgg:
                serviceEgg();
                break;
            case AlienState.aPatrol:
                servicePatrolS();
                break;
            case AlienState.aIdle:
                serviceIdle();
                break;
        }
    }

    // Counter Clockwise Rotation State: alien turns red and rotates 90 degrees counterclockwise
    // Sends to clockwise rotation state
    private void serviceCCWRotation()
    {
        if (frameTracker > rotateFrames)
        {
            // go to next state, reset tracker
            state = AlienState.aCWRotation;
            frameTracker = 0;
        }
        else
        {
            frameTracker++;

            // turns red
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            s.color = new Color(1, 0, 0, 1f);

            // rotate
            Vector3 angles = transform.rotation.eulerAngles;
            angles.z += rotateRate;
            transform.rotation = Quaternion.Euler(0, 0, angles.z);

        }
    }

    // Clockwise Rotation State: alien turns 90 degrees clockwise
    // Sends to Chase State
    private void serviceCWRotation()
    {
        if (frameTracker > rotateFrames)
        {
            // go to next state, reset tracker
            state = AlienState.aChase;
            frameTracker = 0;
        }
        else
        {
            frameTracker++;

            // turns red
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            s.color = new Color(1, 0, 0, 1f);

            // rotate
            Vector3 angles = transform.rotation.eulerAngles;
            angles.z -= rotateRate;
            transform.rotation = Quaternion.Euler(0, 0, angles.z);

        }
    }

    // Chase State: if alien is within 40 units of astro, alien chases astro
    // sends to Englarge State when out of 40 unit range
    private void serviceChase()
    {
        // if farther than 40 units from astro, go to next state
        float dist = Vector3.Distance(astro.transform.localPosition, transform.localPosition);
        if (dist > distToAstro)
        {
            gm.updateEnemyCamBool(false);
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
            state = AlienState.aEnlarge;
            frameTracker = 0;            
        }
        // if still in range chase astro
        else
        {
            gm.updateEnemyCamBool(true);
            //Debug.Log("alien id:  " + currentID);
            //Debug.Log("count is: " + enemies.Count);
            Vector3 astroPos = astro.transform.localPosition;
            Vector3 v = astroPos - transform.localPosition;
            transform.up = Vector3.LerpUnclamped(transform.up, v, .01f);
            transform.localPosition += moveSpeed * Time.smoothDeltaTime * transform.up;
            if (!isInArray)//if not in array add to queue-elygh
            {
                enemies.Add(gameObject);
                currentID = enemies.Count-1 ;
                isInArray = true;
            }
        }
    }

    // Enlarge state: alien doubles in size 
    // Sends to Skrink State
    private void serviceEnlarge()
    {

        if (frameTracker > rotateFrames)
        {
            // Transite to next state
            state = AlienState.aShrink;
            frameTracker = 0;
        }
        else
        {
            frameTracker++;

            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s += scaleRate;
            transform.localScale = new Vector3(s, s, 1);
        }
    }

    // Shrink State: aliem shrinks to normal size, and returns to normal color
    // sends back to Idle or patrol state based on Game Mode
    private void serviceShrink()
    {
        if (frameTracker > sizeFrames)
        {
            // turns to original color
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            s.color = new Color(1, 1, 1, 1);
            // Move to next state
            // Idle movement
            if (gm.currentMode == GameManager.GameMode.Idle)
            {
                state = AlienState.aIdle;
            }
            // Sequential waypoint patrol
            // Random waypoint patrol
            else
            {
                state = AlienState.aPatrol;
            }
            frameTracker = 0;
        }
        else
        {
            frameTracker++;
            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s -= scaleRate;
            transform.localScale = new Vector3(s, s, 1);
        }
    }
    
    // Stunned State: getting hit by a projectile send alien into stunned state
    // Alien turns into an asteroid and lerped in direction of projectile
    // Does not send to another state
    private void serviceStunned()
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
        // check if enemy array is empty
        bool enemyEmpty = true;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                enemyEmpty = false;
            }
        }
        if (enemyEmpty)
            gm.updateEnemyCamBool(false);

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        Sprite newSprite = Resources.Load<Sprite>("Textures/asteroid");
        s.sprite = newSprite;

        Vector3 angles = transform.rotation.eulerAngles;
        angles.z += rotateRate;
        transform.rotation = Quaternion.Euler(0, 0, angles.z);

        // add lerp
        positionLerp.SetLerpParms(lerpDur, lerpRate);
        positionLerp.BeginLerp(transform.localPosition, finalPos);
        if (positionLerp.LerpIsActive())
        {
            Vector3 p = positionLerp.UpdateLerp();
            transform.localPosition = new Vector3(p.x, p.y, 0.0f);
        }
    }

    // Egg State: If alien is in Stunned State and is hit with another projectile, sent to Egg state
    // Turns alien into a skull, and lerps in projectile direction
    // Does not send to another state, if hit again, alien is destroyed
    private void serviceEgg()
    {
        transform.localRotation = Quaternion.identity;
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        Sprite newSprite = Resources.Load<Sprite>("Textures/Skull");
        s.sprite = newSprite;

        // add lerp
        positionLerp.SetLerpParms(lerpDur, lerpRate);
        positionLerp.BeginLerp(transform.localPosition, finalPos);
        // TRY ADDING THIS TO UPDATE
        if (positionLerp.LerpIsActive())
        {
            Vector3 p = positionLerp.UpdateLerp();
            transform.localPosition = new Vector3(p.x, p.y, 0.0f);
        }
    }

    // Idle state: Idle alien movement
    private void serviceIdle()
    {
        transform.localRotation = Quaternion.identity; // Resets rotation
        angle += rotateSpeed * Time.deltaTime;
        var offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
        transform.position = center + offset;
    }

    // Patrol State: patrols the waypoints in sequence determined by Game State
    private void servicePatrolS()
    {
        // point at pos
        pointAtTarget(theTarget.transform.localPosition, turnRate * Time.smoothDeltaTime);
        // move forward
        transform.localPosition += moveSpeed * Time.smoothDeltaTime * transform.up;
        // check target pos
        checkTargetDist();
        center = this.transform.position;

        // Idle movement
        if (gm.currentMode == GameManager.GameMode.Idle)
        {
            state = AlienState.aIdle;
        }
        // Sequential waypoint patrol
        // Random waypoint patrol
        else
        {
            state = AlienState.aPatrol;
        }
    }
}
