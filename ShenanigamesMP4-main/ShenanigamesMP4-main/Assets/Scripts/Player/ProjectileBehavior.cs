using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private const float projSpeed = 40f;

    static private AstroBehavior astro = null;
    public static void setAstro(AstroBehavior a) { astro = a; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Projectile";
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position += transform.up * (projSpeed * Time.smoothDeltaTime);

        CamSupport s = Camera.main.GetComponent<CamSupport>();
        if (s != null)
        {
            Bounds pBound = GetComponent<Renderer>().bounds; // projectile bounds
            CamSupport.WorldBoundStatus status = s.CollideWorldBound(pBound);

            if (status != CamSupport.WorldBoundStatus.Inside) // if hits bounds
            {
                Destroy(transform.gameObject); // destroy projectile
                //astro.destroyProjectile(); // deincrements projectile count
            }
        }
        
    }

}
