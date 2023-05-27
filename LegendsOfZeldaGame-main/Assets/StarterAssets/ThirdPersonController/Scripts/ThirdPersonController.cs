using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;


#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip HurtClip;
        public AudioClip SwordSliceClip;
        public AudioClip ArrowReleaseClip;
        public AudioClip FootStepAudioClip;
        public AudioClip RunningClip;
        public AudioClip DeathClip;
        public AudioClip dancingClip;

        private float initial_height;
        private float final_height;

        AudioSource m_MyAudioSource;

        //public AudioClip FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _bombTimeoutDelta;


        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDBomb;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private const float _threshold = 0.01f;

        public Transform player_camera;
        public Transform right_arm;
        public GameObject arrowObject;
        public GameObject player_camera_3rd;
        public GameObject player_camera_1st;


        public GameObject arrow_projectile;
        public GameObject explosion;
        private bool _hasAnimator;

        // climbing Variables
        [Header("Climbing")]
        public bool isClimbing = false;
        public Transform orientation; // where the player is looking (facing the wall --> climbe , else --> do nothing)
        public Rigidbody rb;
        public LayerMask whatIsWall; // defining the wall surfaces
        private bool walkingUpWhileClimbing = false;

        [Header("Wall Detection")]
        public float detectionLength;
        public float sphereCastRadius;
        public float maxWallLookAngle; // because the player can't climb if she isn't facing the player directly
        private float wallLookAngle; // where the player currently looking
        private RaycastHit frontWallHit; //to store the info for the front wall hit
        private bool isFrontWall; //to check if there is a wall in front of the player
        public float climbSpeed;
        public Transform bokoblin_dance;
        // moving while climbing

        private bool walkingRightWhileClimbing = false;
        private bool walkingLeftWhileClimbing = false;
        private bool walkingDownWhileClimbing = false;
        private float climbingDownSpeed = 3.0f;
        private bool free_fall_initial = true;
        //gliding

        private bool isGliding = false;
        private bool isFreeFall = false;
        public GameObject m_Paraglider;




        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            //AudioSource.PlayClipAtPoint(dancingClip, bokoblin_dance.position, 0.5f);


            m_MyAudioSource = GetComponent<AudioSource>();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif     

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _bombTimeoutDelta = 0.2f;


        }

        private void Update()
        {
            GameObject source_audio = GameObject.Find("One shot audio");
            if (source_audio && AIAgent.bokoblin_attack)
            { 
                Destroy(source_audio);
            }


            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Climb();
            Move();
            wallCheck();
            walkingRWhileClimbing();
            walkingLWhileClimbing();
            walkingDWhileClimbing();
            ///gliding();
            FreeFall();
            RuneAbilities();
            walkingUWhileClimbing();





        }

        private void FreeFall()
        {
            if (!Grounded && free_fall_initial)
            {
                initial_height = transform.position.y;
                free_fall_initial = false;

            }
            if (Grounded)
            {
                final_height = transform.position.y;
                if (initial_height - final_height >= 10)
                {
                    _animator.SetTrigger("death");
                    ScenesOrganizer.sceneList.Add("GameOver");
                    SceneManager.LoadSceneAsync("GameOver");
                }

                else
                    free_fall_initial = true;

            }
            if(isGliding)
                initial_height = transform.position.y;


        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDBomb = Animator.StringToHash("throw");

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void Climb()
        {
            //Input.GetKeyDown(KeyCode.L)
            if (Input.GetKeyDown(KeyCode.LeftShift) && isFrontWall && (wallLookAngle < maxWallLookAngle) && (Grounded))
            {
                _animator.SetBool("isClimbing", true);
                isClimbing = true;
                Gravity = 0;
                FallTimeout = float.MaxValue;
                _verticalVelocity = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _animator.SetBool("isClimbing", false);
                isClimbing = false;
                FallTimeout = 0.15f;
                Gravity = -15;
                _verticalVelocity = 0;

            }
            else if ((isClimbing) && !isFrontWall && !walkingLeftWhileClimbing && !walkingRightWhileClimbing)
            {
                _animator.SetBool("isClimbing", false);
                FallTimeout = 0.15f;
                Gravity = -15;
                //_verticalVelocity = 0;
                isClimbing = false;

            }
        }
        private void walkingUWhileClimbing()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && (isClimbing) && !(Grounded))
            {
                walkingUpWhileClimbing = true;
            }
            else
            {
                walkingUpWhileClimbing = false;
            }
        }
        private void walkingRWhileClimbing()
        {
            if ((Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D)) && isClimbing == true)
            {
                walkingRightWhileClimbing = true;
                _verticalVelocity = 0;
                _animator.SetBool("walkingRightWhileClimbing", true);

            }
            else if ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)) && isClimbing == true)
            {
                walkingRightWhileClimbing = false;
                _animator.SetBool("walkingRightWhileClimbing", false);
                _verticalVelocity = 1f;
            }
            else if (isClimbing == false)
            {
                walkingRightWhileClimbing = false;
                _animator.SetBool("walkingRightWhileClimbing", false);
            }
        }

        private void walkingLWhileClimbing()
        {
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && isClimbing == true)
            {
                walkingLeftWhileClimbing = true;
                _verticalVelocity = 0;
                _animator.SetBool("walkingLeftWhileClimbing", true);

            }
            else if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) && isClimbing == true)
            {
                walkingLeftWhileClimbing = false;
                _animator.SetBool("walkingLeftWhileClimbing", false);
                _verticalVelocity = 1f;
            }
            else if (isClimbing == false)
            {
                walkingLeftWhileClimbing = false;
                _animator.SetBool("walkingLeftWhileClimbing", false);
            }
        }
        private void RuneAbilities()
        {
            // bomb
            if (_input.rune && _bombTimeoutDelta > 0.0f)
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
                _input.rune = false;
                // reset the jump timeout timer
                _animator.SetBool(_animIDBomb, false);


            }
        }



        private void walkingDWhileClimbing()
        {
            bool G = _animator.GetBool("Grounded");
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && (isClimbing) && !(Grounded))
            {

                            
                    walkingDownWhileClimbing = true;
                    _animator.SetBool("walkingDownWhileClimbing", true);
               
            }
            else if ((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) && (isClimbing))
            {
                walkingDownWhileClimbing = false;
                _animator.SetBool("walkingDownWhileClimbing", false);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) && !(isClimbing))
            {
                walkingDownWhileClimbing = false;
                _animator.SetBool("walkingDownWhileClimbing", false);
            }
        }
        private void wallCheck()
        {
            isFrontWall = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
            wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }


            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero & !(isClimbing))
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            if (!isClimbing)
            {
                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                // move the player
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else if (isClimbing)
            {

                _verticalVelocity = 1.0f;
                if (walkingRightWhileClimbing)
                {
                    _verticalVelocity = 0;
                    Vector3 forward = transform.rotation * Vector3.forward;
                    Vector3 down = transform.rotation * Vector3.down;
                    Vector3 normal = Vector3.Cross(transform.forward, -1*transform.up).normalized;
                    _controller.Move(normal * (_speed * Time.deltaTime));
         
                }
                else if (walkingLeftWhileClimbing)
                {
                    _verticalVelocity = 0;
                    Vector3 forward = transform.rotation * Vector3.forward;
                    Vector3 up = transform.rotation * Vector3.up;
                    Vector3 normalL = Vector3.Cross(transform.forward, transform.up).normalized;
                    _controller.Move(normalL * (_speed * Time.deltaTime));
            
                }
                else if (walkingDownWhileClimbing)
                {
                    _verticalVelocity = 0;
                    Vector3 down = Vector3.down;
                    _controller.Move(down * (climbingDownSpeed * Time.deltaTime));
                }
                else
                {
                    Vector3 targetDirection = Vector3.forward;


                    _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                        new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                }
                

            }
            //if (isGliding)
            //{
            //    Vector3 down = Vector3.down;
            //    _controller.Move( down* ((_speed/4) * Time.deltaTime));
            //}
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

      

                if(Input.GetKeyDown(KeyCode.Space) && _input.jump){
                    _animator.SetBool("Jump", false);
                }
                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void gliding()
        {
            bool Is_Jumping = _animator.GetBool("Jump");
            bool Is_Grounded = _animator.GetBool("Grounded");
            if ((Input.GetKeyDown(KeyCode.Space)) && !(Is_Jumping))
            {
               // m_Paraglider.SetActive(true);
                isGliding = true;
                _animator.SetBool("isGliding", true);
                Gravity = -1f;
                _verticalVelocity += (Gravity * Time.deltaTime);
                _verticalVelocity /= 3;
                _animator.SetBool("FreeFall", true);
                m_Paraglider.SetActive(true);
            }
            else if ((Input.GetKeyUp(KeyCode.Space)) || (Is_Grounded))
            {
               // m_Paraglider.SetActive(false);
                _animator.SetBool("isGliding", false);
                isGliding = false;
                m_Paraglider.SetActive(false);
                Gravity = -15;
                _verticalVelocity = 0;
            }
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = FootStepAudioClip;
            m_MyAudioSource.Play();

        }
        private void OnRunning(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = RunningClip;
            m_MyAudioSource.Play();

        }

        private void OnLand(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = LandingAudioClip;
            m_MyAudioSource.Play();
        }
        public void OnSwordSlice(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = SwordSliceClip;
            m_MyAudioSource.Play();
        }

        public void OnArrowRelease(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = ArrowReleaseClip;
            m_MyAudioSource.Play();
            
        }
        private void OnHit(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = HurtClip;
            m_MyAudioSource.Play();

        }
        private void OnDeath(AnimationEvent animationEvent)
        {
            m_MyAudioSource.clip = DeathClip;
            m_MyAudioSource.Play();

        }
    }
}