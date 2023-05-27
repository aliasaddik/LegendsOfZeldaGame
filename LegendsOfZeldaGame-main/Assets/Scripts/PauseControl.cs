using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject gameCanvas;
    public GameObject HealthCanvas;
    bool pause;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 
        if (Time.timeScale == 0)
        {
            PausePanel.SetActive(true);
            gameCanvas.SetActive(false);
            HealthCanvas.SetActive(false);

        }
        else
        {
            PausePanel.SetActive(false);
            gameCanvas.SetActive(true);
            HealthCanvas.SetActive(true);


        }

    }
    public void resume()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
        gameCanvas.SetActive(true);
        HealthCanvas.SetActive(true);


    }
    public void MainMenu()
    {
        Time.timeScale = 1;

        gameCanvas.SetActive(true);
        HealthCanvas.SetActive(true);
        PausePanel.SetActive(false);
        SceneManager.LoadSceneAsync("MainMenu");


    }
     public void Restart()
    {
        Time.timeScale = 1;

        gameCanvas.SetActive(true);
        HealthCanvas.SetActive(true);
        PausePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
   
}
