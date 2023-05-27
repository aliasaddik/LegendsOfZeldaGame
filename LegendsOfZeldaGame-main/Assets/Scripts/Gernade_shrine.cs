using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gernade_shrine : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float expolosionForce = 700f;

    float countdown;

    bool hasExploded = false; 



    public GameObject explosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded )
        {

            Explode();
            hasExploded = true;

        }

        
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Moblin") ||  collision.gameObject.CompareTag("Bokoblin"))
        {
            AIAgent enemy = collision.gameObject.GetComponent<AIAgent>();
            enemy.onAttack(10);

        }
    }

    void Explode() {
       GameObject Ex=  Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider [] colliders =  Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(expolosionForce, transform.position, blastRadius);
            }
            Destructible dest = nearbyObject.GetComponent<Destructible>();
            if(dest != null)
            {
                dest.Destroy();
            }

        }
        Object.Destroy(Ex, 2.0f);
        Destroy(gameObject);

    }
 

}

