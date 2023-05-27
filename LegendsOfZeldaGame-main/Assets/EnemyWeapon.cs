using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public Animator animator;
    AnimatorClipInfo[] m_CurrentClipInfo;
    string previous_clip ="";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("From Weapon " + other.gameObject.tag);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("From Weapon " + other.gameObject.tag);
        Debug.Log("In on trig enemy");
        //Fetch the current Animation clip information for the base layer
        m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        //Access the Animation clip name
        string m_ClipName = m_CurrentClipInfo[0].clip.name;

        if (previous_clip == m_ClipName) return;
        //Check for enemy type
        if (other.gameObject.tag == "Player" )
        {
            Debug.Log("In Trigger: Attack Method: " + m_ClipName + " From: " + this.gameObject.tag + " on " + other.gameObject.tag);
            GameController player = other.gameObject.GetComponent<GameController>();
            if (m_ClipName == "Horizontal_Attack")
            {
                player.onAttack(1);
                Debug.Log("Horizontal Bokoblin");
            }
            if (m_ClipName == "Vertical_Attack")
            {
                player.onAttack(3);
                Debug.Log("Vertical Bokoblin");
            }
            previous_clip = m_ClipName;

            ////Enemy type 1
            //if (this.gameObject.tag == "Bokoblin")
            //{
            //    if (m_ClipName == "Horizontal_Attack")
            //    {
            //        player.onAttack(1);

            //        Debug.Log("Horizontal Bokoblin");
            //    }
            //    if (m_ClipName == "Vertical_Attack")
            //    {
            //        player.onAttack(3);
            //        Debug.Log("Vertical Bokoblin");
            //    }
            //}
            ////enemy type 2
            //if (this.gameObject.tag == "Moblin")
            //{
            //    if (m_ClipName == "Horizontal_Attack")
            //    {
            //        player.onAttack(2);
            //        Debug.Log("Horizontal Moblin");
            //    }
            //    if (m_ClipName == "Vertical_Attack")
            //    {
            //        player.onAttack(4);
            //        Debug.Log("Vertical Moblin");
            //    }

            //}
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("From Weap Col " + collision.gameObject.tag);
    }

}
