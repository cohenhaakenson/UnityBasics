using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public float lifeTime = .3f;
    private float timeAlive;
    // Start is called before the first frame update
    void Start()
    {
        timeAlive = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Deletes itself after lifeTime
        if (Time.time > timeAlive + lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
