using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // to add text

public class GameManager : MonoBehaviour
{
    public static GameManager globalBehavior = null;

    public AstroBehavior astro = null;
    public AlienBehavior alien;
    
    public WayPointBehavior A;
    public WayPointBehavior B;
    public WayPointBehavior C;
    public WayPointBehavior D;
    public WayPointBehavior E;
    public WayPointBehavior F;

    public WayPointBehavior[] wps;
    public GameObject waypointParent;
    public GameObject projectileParent;

    //UI Texts
    public Text projectileCount = null;
    public Text currentEnemyCount = null;
    public Text defeatedEnemyCount = null;
    public Text collisionCount = null;
    public Text playMode = null;
    public Text gameMode = null;
    public UICamBehavior uiCam;
    public bool enemyCamActive;

    private int enemyCount = 0; // enemies on screen
    public int totalEnemies = 10;
    private Camera mainCam; 
    private Vector3 spawnBounds; // enemy spawn bounds

    public GameMode currentMode = GameMode.Sequential;
    public enum GameMode
    {
        Sequential, 
        Random,
        Idle
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(currentMode);
        GameManager.globalBehavior = this;
        Debug.Assert(projectileCount != null);
        Debug.Assert(astro != null);
        Debug.Assert(waypointParent != null);
        Debug.Assert(alien != null);
        Debug.Assert(uiCam != null);

        mainCam = gameObject.GetComponent<Camera>();
        updateSpawnBounds(mainCam);
        Debug.Log("spawnBounds " + spawnBounds);

        // Initializes objects
        ProjectileBehavior.setAstro(astro);
        AlienBehavior.setAstro(astro);
        AlienBehavior.setGM(this);
        WayPointBehavior.setAstro(astro);

        // Spawns initial 10 enemies in random spots
        while (enemyCount < totalEnemies)
        {
            GameObject enemy = Instantiate(Resources.Load("Prefabs/Alien") as GameObject);
            enemyCount++;
            Vector3 randomPosition = 
                new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y,spawnBounds.y), 0);
            enemy.transform.localPosition = randomPosition;
        }

        wps = new WayPointBehavior[] {A,B,C,D,E,F};
    }

    // Update is called once per frame
    void Update()
    {
        // Q to quit game
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        // Enemy Spawns if less than 10
        if (enemyCount < totalEnemies)
        {
            spawnNew();
        }

        // Game mode updates
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentMode == GameMode.Sequential)
            {
                currentMode = GameMode.Random;
                
            }
            else if (currentMode == GameMode.Random)
            {
                currentMode = GameMode.Idle;
            }
            else
            {
                currentMode = GameMode.Sequential;
            }
        }

        // hide waypoints
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameManager.globalBehavior.waypointParent.SetActive(!GameManager.globalBehavior.waypointParent.activeSelf);
        }

        // UI updates
        projectileCount.text = astro.projStatus();
        playMode.text = astro.getPlayMode();
        currentEnemyCount.text = "Current Enemies: " + enemyCount;
        defeatedEnemyCount.text = astro.destroyedStatus();
        collisionCount.text = astro.collisonStatus();
        gameMode.text = "Game Mode: " + currentMode;
        uiCam.setEnemyCamText("Enemy Chase Cam: " + getChaseState());
        uiCam.setWaypointCamText("Waypoint Cam: " + A.getWaypointState());

    }

    // Spawn new Alien
    private void spawnNew()
    {
        GameObject enemy = Instantiate(Resources.Load("Prefabs/Alien") as GameObject);
        enemyCount++;
        updateSpawnBounds(mainCam);
        Vector3 randomPosition =
            new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y), 0);
        enemy.transform.localPosition = randomPosition;
    }

    // Updates spawnbounds if window changes
    private void updateSpawnBounds(Camera cam)
    {
        // new bounds within 90% of game window
        spawnBounds = new Vector3((cam.orthographicSize * cam.aspect)*0.9f, 
                                            (cam.orthographicSize)*0.9f, 0);
    }

    // Deincrements alien count
    public void defeatAlien() { enemyCount--; }

    public void updateEnemyCamBool(bool active)
    {
        enemyCamActive = active;
    }


    public string getChaseState()
    {
        if (enemyCamActive)
            return "Active";
        else
            return "Shut Off";
    }

}
