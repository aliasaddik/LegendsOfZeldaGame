using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GernadeBoss : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Boss"))
        {
            Boss2 boss = collision.gameObject.GetComponent<Boss2>();
            Boss2.gotHit = true;
            boss.onAttack(10);
        }
    }


    void Explode() 
    {
       GameObject Ex=  Instantiate(explosionEffect, transform.position, transform.rotation);
        Object.Destroy(Ex, 2.0f);
        Destroy(gameObject);
    }
 

}

