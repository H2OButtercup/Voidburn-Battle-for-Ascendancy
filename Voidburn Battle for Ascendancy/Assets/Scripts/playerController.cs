using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform opponent;
    [SerializeField] Animator animator;
    [SerializeField] float doubleTapThreshold = 0.3f;
    [SerializeField] float sideStepDistance = 2f;
    [SerializeField] float sideStepSpeed = 10f;

    private float lastLeftTapTime = -1f;
    private float lastRightTapTime = -1f;
    private bool isSideStepping = false;
    private Vector3 sideStepTarget;
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isJumpPressed;
    private bool isAttackPressed;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => isJumpPressed = true;
        controls.Player.Jump.canceled += ctx => isJumpPressed = false;

        controls.Player.Attack.performed += ctx => isAttackPressed = true;
        controls.Player.Attack.canceled += ctx => isAttackPressed = false;
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Update()
    {
        FaceOpponent();
        HandleMovement();
        HandleJump();
        HandleAttack();

        void DetectDoubleTap()
        {
            Vector3 toOpponent = opponent.position - transform.position;
            Vector3 fightRight = Vector3.Cross(Vector3.up, toOpponent).normalized;

            if (moveInput.x < -0.5f)
            {
                if (Time.time - lastLeftTapTime < doubleTapThreshold)
                    TriggerSideStep(-fightRight); // Side step left
                lastLeftTapTime = Time.time;
            }
            else if (moveInput.x > 0.5f)
            {
                if (Time.time - lastRightTapTime < doubleTapThreshold)
                    TriggerSideStep(fightRight); // Side step right
                lastRightTapTime = Time.time;
            }
        }

        if (isSideStepping)
        {
            Vector3 move = (sideStepTarget - transform.position).normalized * sideStepSpeed * Time.deltaTime;
            controller.Move(move);

            if (Vector3.Distance(transform.position, sideStepTarget) < 0.1f)
            {
                isSideStepping = false;
            }
        }
        else
        {
            DetectDoubleTap(); // Only detect taps when not side stepping
            HandleMovement();  // Regular movement
        }
    }

    void FaceOpponent()
    {
        if (opponent == null) return;

        Vector3 direction = opponent.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void HandleMovement()
    {
        Vector3 toOpponent = opponent.position - transform.position;
        toOpponent.y = 0;
        Vector3 fightRight = Vector3.Cross(Vector3.up, toOpponent).normalized;

        Vector3 move = fightRight * moveInput.x * moveSpeed * Time.deltaTime;
        controller.Move(move);

        animator.SetFloat("MoveSpeed", Mathf.Abs(moveInput.x));
    }

    void HandleJump()
    {
        if (isJumpPressed)
        {
            animator.SetTrigger("Jump");
            isJumpPressed = false;
        }
    }

    void HandleAttack()
    {
        if (isAttackPressed)
        {
            animator.SetTrigger("Attack");
            isAttackPressed = false;
        }
    }

    void TriggerSideStep(Vector3 direction)
    {
        if (isSideStepping) return;

        isSideStepping = true;
        sideStepTarget = transform.position + direction * sideStepDistance;
        animator.SetTrigger("SideStep");
    }
}