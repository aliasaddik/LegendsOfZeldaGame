using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class BombAbilityBoss : MonoBehaviour
    {
        [Header("References")]
        public Transform Hand;
        public GameObject objectToThrow;

        [Header("Throwing")]
        public float throwForce;
        public float throwUpwardForce;


        GameObject projectile;
        Rigidbody projectileRb;

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
               
                    Throw();
                   

            }

           

        }
        
    private void Throw()
        {

            projectile = Instantiate(objectToThrow, Hand.position, gameObject.transform.rotation);
            projectile.transform.SetParent(Hand, true);

            // get rigidbody component
            projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.useGravity = false;


            StartCoroutine(WaitShoot());
            
        }

       

        IEnumerator WaitShoot()
        {
            yield return new WaitForSeconds(1.5f);

            // get rigidbody component
            projectileRb.useGravity = true;


            // calculate direction
            Vector3 forceDirection = Hand.transform.forward;

            RaycastHit hit;

            if (Physics.Raycast(Hand.position, Hand.forward, out hit, 500f))
            {
                forceDirection = (hit.point - Hand.transform.forward).normalized;
            }

            // add force
            Vector3 forceToAdd = transform.forward * throwForce + transform.up * throwUpwardForce;

            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);


        }

        }


