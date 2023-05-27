using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OverworldTransition : MonoBehaviour
{
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
        // When player collides with box collider at the door of the castle
        //Debug.Log(AIAgent.deaths);
        if(other.gameObject.CompareTag("Player") && AIAgent.deaths == 9)
        {
            ScenesOrganizer.sceneList.Add("ShrineScene");
            SceneManager.LoadSceneAsync("ShrineScene");
        }



    }
}
