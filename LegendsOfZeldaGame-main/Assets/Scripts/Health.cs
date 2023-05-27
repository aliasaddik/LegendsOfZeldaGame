using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
  
    int number_of_hearts;
    public Image [] hearts;
    public GameObject SSmode;
    public GameObject BAmode;
    public Sprite full_heart;
    public Sprite half_heart;
    public Sprite empty_heart;

     

    // Update is called once per frame
    void Update()
    {
        int health = GameController.health;
        bool mode = GameController.arrow_mode; //true for one and false for the other
        bool isOdd = false;
        if( health % 2 == 0)
        {
            number_of_hearts = health / 2;
        }
        else
        {
            isOdd = true;
            number_of_hearts = (health-1) / 2;
        }
        for(int i = 0; i < hearts.Length; i++)
        {
            if (i < number_of_hearts)
            {
                hearts[i].sprite = full_heart;
            }
            else
            {
                hearts[i].sprite = empty_heart;
            }
        }
        if (isOdd)
        {
            hearts[number_of_hearts].sprite = half_heart;
        }
        

        if (!mode)
        {
            SSmode.SetActive(true);
            BAmode.SetActive(false);
        }
        else
        {
            SSmode.SetActive(false);
            BAmode.SetActive(true);
        }
    }
}
