using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_shrine : MonoBehaviour
{
    public static GameObject onPlatform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = transform;
            onPlatform = gameObject;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
            gameObject.GetComponent<Animator>().enabled = true;
        }
    }
    


}
