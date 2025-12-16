using UnityEngine;

namespace ReadyPlayerMe.Samples.QuickStart
{
    [RequireComponent(typeof(CharacterController), typeof(GroundCheck))]
    public class ThirdPersonMovement : MonoBehaviour
    {
        private const float TURN_SMOOTH_TIME = 0.05f;

        [SerializeField][Tooltip("Used to determine movement direction based on input and camera forward axis")]
        [HideInInspector] public Transform playerCamera;
        [SerializeField][Tooltip("Move speed of the character in")]
        public float walkSpeed = 3f;
        [SerializeField][Tooltip("Run speed of the character")] 
        private float runSpeed = 8f;
        [SerializeField][Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
        private float gravity = -18f;
        [SerializeField][Tooltip("The height the player can jump ")] 
        private float jumpHeight = 3f;

        private CharacterController controller;
        private GameObject avatar;
        private float verticalVelocity;  
        private float turnSmoothVelocity;

        private bool jumpTrigger;
        public float CurrentMoveSpeed { get; private set; }
        private bool isRunning;

        private GroundCheck groundCheck;

        [Header("AR Follow Mode")]
        [SerializeField] private bool enableARFollowMode = true; // Check this in Inspector for your AR build
        [SerializeField] private Transform arFollowTarget;       // Drag the cube here (optional, if you want to set from Inspector)
        [SerializeField] private float runDistanceThreshold = 5f; // Distance at which to trigger running animation

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            groundCheck = GetComponent<GroundCheck>();
        }

        public void Setup(GameObject target)
        {
            avatar = target;
            if (playerCamera == null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        public void Move(float inputX, float inputY)
        {
            if (enableARFollowMode && arFollowTarget != null)
            {
                ARFollowModeMove();
                return;
            }

            // Simplified flat-ground movement (no gravity since AR doesn't need it)
            var moveDirection = playerCamera.right * inputX + playerCamera.forward * inputY;
            var moveSpeed = isRunning ? runSpeed : walkSpeed;

            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);

            var moveMagnitude = moveDirection.magnitude;
            CurrentMoveSpeed = isRunning ? runSpeed * moveMagnitude : walkSpeed * moveMagnitude;

            if (moveMagnitude > 0)
            {
                RotateAvatarTowardsMoveDirection(moveDirection);
            }
        }

        private void ARFollowModeMove()
        {
            Vector3 direction = arFollowTarget.position - transform.position;
            float distance = direction.magnitude;

            if (distance < 0.1f)
            {
                CurrentMoveSpeed = 0f;
                return;
            }

            // Full 3D movement (including Y)
            Vector3 moveDir = direction.normalized;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir * speed * Time.deltaTime);

            CurrentMoveSpeed = speed * Mathf.Clamp01(distance / runDistanceThreshold); // For animation blending

            // Rotate to face target direction (horizontal only — keep upright)
            Vector3 horizontalDir = new Vector3(moveDir.x, 0, moveDir.z);
            if (horizontalDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(horizontalDir);
                avatar.transform.rotation = Quaternion.Slerp(avatar.transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            // Keep avatar upright
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

        private void RotateAvatarTowardsMoveDirection(Vector3 moveDirection)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + transform.rotation.y;
            float angle = Mathf.SmoothDampAngle(avatar.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TURN_SMOOTH_TIME);
            avatar.transform.rotation = Quaternion.Euler(0, angle, 0);
        }


        public void SetIsRunning(bool running)
        {
            isRunning = running;
        }
        
        public bool TryJump()
        {
            jumpTrigger = false;
            if (controller.isGrounded)
            {
                jumpTrigger = true;
            }
            return jumpTrigger;
        }

        public bool IsGrounded()
        {
            return groundCheck.IsGrounded();
        }
    }
}
