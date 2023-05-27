using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShrinePause : MonoBehaviour
{
    bool pause;
    public GameObject gameCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause)
            {
                Time.timeScale = 1.0f;
                pause = false;
                gameCanvas.SetActive(false);

            }
            else
            {
                Time.timeScale = 0f;
                pause = true;
                gameCanvas.SetActive(true);

            }
        }

    }
    public void resume()
    {
        Time.timeScale = 1;
        gameCanvas.SetActive(false);


    }
    public void MainMenu()
    {
        Time.timeScale = 1;

        gameCanvas.SetActive(false);
        SceneManager.LoadSceneAsync("MainMenu");


    }
    public void Restart()
    {
        Time.timeScale = 1;

        gameCanvas.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
