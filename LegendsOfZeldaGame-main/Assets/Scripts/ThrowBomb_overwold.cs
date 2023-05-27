using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb_overwold : MonoBehaviour
{
    //throw bomb 
    GameObject projectile;
    Rigidbody projectileRb;

    public Transform Hand;
    public GameObject objectToThrow;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
        yield return new WaitForSeconds(0f);

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
