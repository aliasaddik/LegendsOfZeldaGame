using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class RunesAbilities_shrine : MonoBehaviour
    {



        [Header("References")]
        public Transform Hand;
        public Transform Head;
        public GameObject objectToThrow;


        [Header("Material")]
        public Material stone;
        public Material Ice;



        [Header("Throwing")]
        public float throwForce;
        public float throwUpwardForce;

        GameObject platform;

        public static bool freezeAbility =false;
        public static bool bombAbility =true;

        bool freezeBool = false;

        GameObject projectile;
        Rigidbody projectileRb;

        public GameObject RuneState;

        public GameObject bombImg;
        public GameObject snowImg;

        private float _bombTimeoutDelta;
        private int _animIDBomb;
        public Animator _animator;
        private bool activeBombAnim;







    private void Start()
        {
        _animIDBomb = Animator.StringToHash("throw");
        _bombTimeoutDelta = 0.2f;

    }

    private void Update()
        {
        
        if (freezeAbility)
            {
                bombImg.SetActive(false);
                snowImg.SetActive(true);

            }
            else if (bombAbility)
            {
                bombImg.SetActive(true);
                snowImg.SetActive(false);

            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (freezeAbility)
                {
                    if (!freezeBool)
                    {
                        Freeze();
                    }
                }
                if (bombAbility)
                {
                    activeBombAnim = true;
                    Throw();
                    StopFreeze();
                }

            }
        RuneAbilities();


        //if (Input.GetKeyDown(KeyCode.Alpha1) && readyToThrow )
        //{
        //    Throw();
        //    stopFreeze();

        //}

        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    if (!freezeBool)
        //    {
        //        Freeze();
        //    }

        //}

    }
    public void SetupRuneBar(Canvas canvas ,Camera camera)
        {
            RuneState.transform.SetParent(canvas.transform);
            if(RuneState.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
            {
                faceCamera.Camera = camera;

            }
        }

    private void Throw()
        {

            projectile = Instantiate(objectToThrow, Hand.position, gameObject.transform.rotation);
            projectile.transform.SetParent(Hand, true);

            // get rigidbody component
            projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.useGravity = false;


            StartCoroutine(WaitShoot());
            
        }

        private void Freeze()
        {

            RaycastHit hit;

            Vector3 freezeDir = new Vector3(-1, -1, 0);

            if (Physics.Raycast(Head.transform.position, freezeDir, out hit, 500f))
            {
                //Debug.DrawLine(Head.transform.position, hit.point, Color.green, 0.5f);

                if (hit.collider.gameObject.CompareTag("Platform") && !freezeBool && !(hit.collider.gameObject == MovingPlatform_shrine.onPlatform))
                {
                    freezeBool = true;
                    //Debug.DrawLine(Head.transform.position, hit.point, Color.green, 0.5f);
                    platform = hit.collider.gameObject;

                    platform.GetComponent<Animator>().enabled = false;
                    platform.GetComponent<MeshRenderer>().material = Ice;
                    StartCoroutine(UnFreeze());

                }
            }

        }
        private void StopFreeze()
        {
            platform.GetComponent<Animator>().enabled = true;
            platform.GetComponent<MeshRenderer>().material = stone;
        }
        IEnumerator UnFreeze()
        {
            yield return new WaitForSeconds(10);

            platform.GetComponent<Animator>().enabled = true;
            platform.GetComponent<MeshRenderer>().material = stone;
            freezeBool = false;

        }

        IEnumerator WaitShoot()
        {
            yield return new WaitForSeconds(1.5f);

            // get rigidbody component
            projectileRb.useGravity = true;


            // calculate direction
            Vector3 forceDirection = Hand.transform.forward;

            RaycastHit hit;

            if (Physics.Raycast(Hand.position, Hand.forward, out hit, 500f))
            {
                forceDirection = (hit.point - Hand.transform.forward).normalized;
            }

            // add force
            Vector3 forceToAdd = transform.forward * throwForce + transform.up * throwUpwardForce;

            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);


        }
    private void RuneAbilities()
    {



        // bomb
        if (activeBombAnim && bombAbility && _bombTimeoutDelta > 0.0f)
        {

            // update animator if using character
            
                _animator.SetBool(_animIDBomb, true);
            
        }

        // bomb timeout
        if (_bombTimeoutDelta >= 0.0f)
        {
            _bombTimeoutDelta -= Time.deltaTime;
        }

        if (_bombTimeoutDelta <= 0.0f)
        {
            _bombTimeoutDelta = 0.2f;
            _animator.SetBool(_animIDBomb, false);
            activeBombAnim = false;
            // reset the jump timeout timer


        }
    }

}


