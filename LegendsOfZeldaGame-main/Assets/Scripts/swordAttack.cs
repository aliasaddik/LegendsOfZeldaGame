using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordAttack : MonoBehaviour
{
    private GameController game_controller;
    public GameObject player;
    bool attacking;
    GameController game;
    public Animator animator;
    AnimatorClipInfo[] m_CurrentClipInfo;

    private void Update()
    {
        game = player.GetComponent<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        string m_ClipName = m_CurrentClipInfo[0].clip.name;
        //Debug.Log(m_ClipName);
        //Debug.Log(m_ClipName == "Sword And Shield Slash");
        if ((other.gameObject.CompareTag("Moblin") || other.gameObject.CompareTag("Bokoblin")) && m_ClipName == "Sword And Shield Slash")
        {
            ////Debug.Log("Did hit enemy here");
            AIAgent enemy = other.gameObject.GetComponent<AIAgent>();
            enemy.onAttack(10);
        }

        if (other.gameObject.CompareTag("Boss") && !Boss2.isBodySlamming)
        {
            Boss2 enemy = other.gameObject.GetComponent<Boss2>();
            Boss2.gotHit= true;
            enemy.onAttack(10);
        }
    }

}
