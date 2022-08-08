using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour
{
    private const float bombSpeed = 60f;

    static private AstroBehavior astro = null;
    public static void setAstro(AstroBehavior a) { astro = a; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Bomb";
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * (bombSpeed * Time.smoothDeltaTime);

        CamSupport s = Camera.main.GetComponent<CamSupport>();
        if (s != null)
        {
            Bounds bBound = GetComponent<Renderer>().bounds; // bomb bounds
            CamSupport.WorldBoundStatus status = s.CollideWorldBound(bBound);

            if (status != CamSupport.WorldBoundStatus.Inside) // if hits bounds
            {
                Destroy(transform.gameObject); // destroy bomb
            }
        }
    }
}
