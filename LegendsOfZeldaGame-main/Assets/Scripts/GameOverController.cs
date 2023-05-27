using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        //Debug.Log("Restart");
        SceneManager.LoadScene(ScenesOrganizer.sceneList[ScenesOrganizer.sceneList.Count - 2], LoadSceneMode.Single);

    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

