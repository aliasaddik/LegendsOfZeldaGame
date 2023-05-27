using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public GameObject sword_hand;
    public GameObject sword_back;
    public GameObject shield_hand;
    public GameObject shield_back;

    public GameObject bow_hand;
    public GameObject bow_back;

    public static bool arrow_mode = true;
    public static int health = 24;

    private StarterAssets.StarterAssetsInputs _input;
    private StarterAssets.ThirdPersonController controller;

    public float max_shield_time = 10; // max_time to hold shield upward
    public float shield_recovery_time = 5;
    public static bool isShielded = false;
    public bool shieldAllowed = true;
    public bool sword_attack = false;
    public static bool attackable = true;
    bool invincible = false;
    bool slow = false;
    bool pause = false;



    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
        controller = GetComponent<StarterAssets.ThirdPersonController>();
        shieldAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            invincible = !invincible;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            health += 10;
            if (health > 24)
                health = 24;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (slow)
            {
                Time.timeScale = 1.0f;
                slow = false;
            }
            else
            {
                Time.timeScale = 0.5f;
                slow = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause)
            {
                Time.timeScale = 1.0f;
                pause = false;
            }
            else
            {
                Time.timeScale = 0f;
                pause = true;
            }
        }

        invinciblitytCheat();
        heal();
        if (health <= 0)
        {
            controller.enabled = false;
            ScenesOrganizer.sceneList.Add("GameOver");
            SceneManager.LoadSceneAsync("GameOver");

        }
        if (Input.GetKeyDown(KeyCode.Tab) && !controller.isClimbing)
        {
            switchWeapons();
        }
        if (arrow_mode && !controller.isClimbing)
        {
            rangedMode();
        }
        else if (!controller.isClimbing)
        {
            meeleMode();
        }
        if (isShielded) // as long as i'm shielded start counting 10 seconds
        {
            max_shield_time -= Time.deltaTime;
            if (max_shield_time <= 0)
            { // if 10 sec are over then use of shield is not allowed for 5 seconds
                shieldAllowed = false;
                max_shield_time = 10;
                controller._animator.SetBool("Shield", false);
            }
        }

        if (!shieldAllowed) // count 5 sec for shield recovery time
        {
            shield_recovery_time -= Time.deltaTime;
            if (shield_recovery_time <= 0)
            {
                shieldAllowed = true;
                shield_recovery_time = 5;
            }
        }
    }


    public void switchWeapons()
    {
        arrow_mode = !arrow_mode;
        if (!arrow_mode) // sword shield mode
        {
            // remove bow from hand
            bow_hand.SetActive(false);
            // add bow to back
            bow_back.SetActive(true);
            // remove shield & sword from back
            shield_back.SetActive(false);
            sword_back.SetActive(false);
            // add Shield & sword in Hand
            shield_hand.SetActive(true);
            sword_hand.SetActive(true);
            ;

        }
        else // arrow bow mode
        {
            // remove sword & shield from hand
            shield_hand.SetActive(false);
            sword_hand.SetActive(false);

            // add Shield & sword to back
            shield_back.SetActive(true);
            sword_back.SetActive(true);

            // remove bow from back
            bow_back.SetActive(false);

            //add bow to hand
            bow_hand.SetActive(true);

        }
    }

    public void meeleMode()
    {
        // Logic for holding shield: 
        if (!_input.aim) // if not shielded (right click mouse not pressed), then timer is reset
        {
            controller._animator.SetBool("Shield", false);
            max_shield_time = 10;
            isShielded = false;
        }
        if (_input.aim && controller.Grounded && shieldAllowed)
        {
            controller._animator.SetBool("Shield", true);
            isShielded = true;
        }

        controller._animator.SetBool("Sword", _input.shoot);
        sword_attack = _input.shoot;

    }

    private void rangedMode()
    {
        if (_input.aim && controller.Grounded)
        {
            controller.player_camera_3rd.SetActive(false);
            // Play Aim Animation
            controller._animator.SetBool("Aiming", _input.aim);
            controller._animator.SetBool("Shooting", _input.shoot);
            //if (_input.shoot) { ShootArrow(); }
        }
        else
        {
            // Switch camera to 3rd person
            controller.player_camera_3rd.SetActive(true);
            controller._animator.SetBool("Aiming", false);
            controller._animator.SetBool("Shooting", false);
        }
    }

    public void ShootArrow()
    {
        //GameObject arrow_shoot = Instantiate(arrowObject, right_arm.position, player_camera.rotation);
        Vector3 forceDirection = controller.player_camera.rotation * Vector3.forward;
        GameObject particle_arrow = Instantiate(controller.arrow_projectile, controller.right_arm.position, controller.player_camera.transform.rotation) as GameObject;
        RaycastHit hit;

        if (Physics.Raycast(controller.player_camera.position, forceDirection, out hit, Mathf.Infinity))
        {
            forceDirection = (hit.point - controller.right_arm.position).normalized;
            particle_arrow.GetComponent<Rigidbody>().AddForce(forceDirection * 25f, ForceMode.Impulse);

            if (hit.collider.gameObject.CompareTag("Moblin") || hit.collider.gameObject.CompareTag("Bokoblin"))

            {
                AIAgent enemy = hit.collider.gameObject.GetComponent<AIAgent>();
                enemy.onAttack(5);
            }


            if (hit.collider.gameObject.CompareTag("Boss") && !Boss2.isBodySlamming)
            {
                Boss2 enemy = hit.collider.gameObject.GetComponent<Boss2>();
                Boss2.gotHit = true;
                enemy.onAttack(5);
            }

        }
        //Debug.DrawRay(controller.player_camera.position, forceDirection * 200f, Color.red);
    }


    void heal()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (health + 10 > 24)
            {
                health = 24;
            }
            else
                health = health + 10;
        }
    }
    void hurt()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {

                health = health - 24;
            

        }
    }


    void invinciblitytCheat()
    {
        if (Input.GetKeyDown(KeyCode.I) && attackable)
        {
            attackable = false;
        }


        if (Input.GetKeyDown(KeyCode.I) && !attackable)
        {

            attackable = true;
        }


    }

    public void onAttack(int damage)
    {
        if (!invincible)
        {
            if (!isShielded)
            {

                if (health - damage <= 0)
                {
                    health = 0;
                    controller._animator.SetTrigger("death");
                }
                else
                {
                    health -= damage;
                    controller._animator.SetTrigger("attacked");
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
    }



}