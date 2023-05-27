using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public GameObject Play;
    public GameObject Options;
    public GameObject Menu;
    public GameObject SelectGame;
    public GameObject AssetsCredits;
    public GameObject TeamCredits;





    // Start is called before the first frame update
    void Start()
    {
        Options.gameObject.SetActive(false);
        Play.gameObject.SetActive(false);
        SelectGame.gameObject.SetActive(false);





    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void viewPlay()
    {
        Play.gameObject.SetActive(true);
        Menu.gameObject.SetActive(false);
    }
    public void OptionsPanel()
    {
        Options.gameObject.SetActive(true);
        Menu.gameObject.SetActive(false);

    }
    
    public void ChooseGame()
    {
        SelectGame.gameObject.SetActive(true);
        Play.gameObject.SetActive(false);

    }
    public void LoadAsset()
    {
        AssetsCredits.gameObject.SetActive(true);
        Options.gameObject.SetActive(false);

    }
    public void LoadTeam()
    {
        TeamCredits.gameObject.SetActive(true);
        Options.gameObject.SetActive(false);

    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadOverWorld()
    {
        ScenesOrganizer.sceneList.Add("OverWorldScene");
        SceneManager.LoadSceneAsync("OverWorldScene");


    }
    public void LoadShrine()
    {
        ScenesOrganizer.sceneList.Add("ShrineScene");
        SceneManager.LoadSceneAsync("ShrineScene");


    }
    public void LoadBoss()
    {
        ScenesOrganizer.sceneList.Add("BossScene");
        SceneManager.LoadSceneAsync("BossScene");

    }
    public void backFromoptions()
    {
        Options.gameObject.SetActive(false);
        Menu.gameObject.SetActive(true);

    }
    public void backFromSelectGame()
    {
        SelectGame.gameObject.SetActive(false);
        Play.gameObject.SetActive(true);

    }
    public void backFromPlay()
    {
        Play.gameObject.SetActive(false);
        Menu.gameObject.SetActive(true);

    }
    public void backFromAsset()
    {
        AssetsCredits.gameObject.SetActive(false);
        Options.gameObject.SetActive(true);

    }
    public void backFromTeam()
    {
        TeamCredits.gameObject.SetActive(false);
        Options.gameObject.SetActive(true);

    }

}
