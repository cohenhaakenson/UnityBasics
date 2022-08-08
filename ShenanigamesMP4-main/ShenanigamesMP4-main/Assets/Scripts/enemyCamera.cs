using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCamera : MonoBehaviour
{
    public GameObject hero;
    private Camera theCam;

    [HideInInspector]
    public List<GameObject> enemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //*******to see background image from canvas, need to make vackground
        theCam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float newX = 0;
        float newY = 0;
        float distance = 0;

        
        if (enemies.Count != 0) {
            theCam.clearFlags = CameraClearFlags.Depth;//without this, the camera just keeps showing the background image
            theCam.farClipPlane = 1000f;//without this, the camera just keeps showing the background image

            newX += enemies[0].transform.position.x;
            newY += enemies[0].transform.position.y;
            newX += hero.transform.position.x;
            newY += hero.transform.position.y;

            newX /= 2;
            newY /= 2;

            transform.position = new Vector3(newX, newY, -10);
            //distance = Mathf.Sqrt(Mathf.Pow(newX-hero.transform.position.x,2) + Mathf.Pow(newY - hero.transform.position.y, 2));
            distance = (hero.transform.position - enemies[0].transform.position).magnitude;
            theCam.orthographicSize = distance;
        }
        else
        {
            transform.position = new Vector3(0, 0, 50);
            theCam.farClipPlane = 50f;//without this, the camera just keeps showing the background image
            theCam.clearFlags = CameraClearFlags.Skybox;
        }
    }
}
