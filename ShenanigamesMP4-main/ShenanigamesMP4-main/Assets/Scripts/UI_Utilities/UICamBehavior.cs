using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // to add text

public class UICamBehavior : MonoBehaviour
{
    public Camera theCam;
    public Text enemyCamText;
    public Text waypointCamText;
    private string enemyText = "Shut Off";
    private string waypointText = "Shut Off";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(theCam != null);
        Debug.Assert(enemyCamText != null);
        Debug.Assert(waypointCamText != null);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCamText.text = enemyText;
        waypointCamText.text = waypointText;
    }

    public void setEnemyCamText(string s) { enemyText = s;}
    public void setWaypointCamText(string s) { waypointText = s;}
}
