using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [Header("General Stats")]
    public int Hp;


    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float sideStepDistance = 2f;
    public float rotationSpeed = 5f;
    public float tapThreshold = 0.3f;

    [Header("References")]
    public Transform opponent;
    public Animator animator;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isGrounded = true;
    private bool isSideStepping = false;

    private float verticalVelocity = 0f;
    private float lastVerticalTapTime = -1f;
    private int verticalTapCount = 0;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Jump.performed += ctx => TryJump();
        controls.Player.Crouch.performed += ctx => TryCrouch();
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        controls.Player.Jump.performed -= ctx => TryJump();
        controls.Player.Crouch.performed -= ctx => TryCrouch();
        controls.Disable();
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleFacing();
        HandleVerticalInput(moveInput.y);
        ApplyGravity();
    }

    private void HandleMovement()
    {
        if (opponent == null || isSideStepping) return;

        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 moveDirection = toOpponent * moveInput.x; // Left/Right input = toward/away

        moveDirection.y = 0f;
        Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
        transform.position += move;

        bool walkingForward = moveInput.x > 0.1f;
        bool walkingBackward = moveInput.x < -0.1f;

        animator.SetBool("IsWalkingForward", walkingForward);
        animator.SetBool("IsWalkingBackward", walkingBackward);
    }

    private void HandleFacing()
    {
        if (isGrounded && !isSideStepping && opponent != null)
        {
            Vector3 direction = opponent.position - transform.position;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleVerticalInput(float vertical)
    {
        if (Mathf.Abs(vertical) < 0.8f) return;

        if (Time.time - lastVerticalTapTime < tapThreshold)
        {
            verticalTapCount++;
        }
        else
        {
            verticalTapCount = 1;
        }

        lastVerticalTapTime = Time.time;

        if (verticalTapCount == 2)
        {
            if (vertical > 0)
                TriggerSidestepOut(); // Double tap up
            else
                TriggerSidestepIn();  // Double tap down

            verticalTapCount = 0;
        }
        else
        {
            if (vertical > 0)
                TryJump();       // Single tap up
            else
                TryCrouch();     // Single tap down
        }
    }

    private void TryJump()
    {
        if (!isGrounded) return;

        verticalVelocity = jumpForce;
        animator.SetTrigger("Jump");
        isGrounded = false;
    }

    private void TryCrouch()
    {
        if (!isGrounded) return;

        animator.SetTrigger("Crouch");
    }

    private void TriggerSidestepIn()
    {
        if (!isGrounded || opponent == null) return;

        isSideStepping = true;

        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toOpponent).normalized;

        Vector3 step = -side * sideStepDistance;
        transform.position += step;

        animator.SetTrigger("SidestepLeft");
        Invoke(nameof(ResetSideStep), 0.3f);
    }

    private void TriggerSidestepOut()
    {
        if (!isGrounded || opponent == null) return;

        isSideStepping = true;

        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toOpponent).normalized;

        Vector3 step = side * sideStepDistance;
        transform.position += step;

        animator.SetTrigger("SidestepRight");
        Invoke(nameof(ResetSideStep), 0.3f);
    }

    private void ResetSideStep()
    {
        isSideStepping = false;
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            animator.SetTrigger("Land");
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        }
    }

    private void CheckGrounded()
    {
        float rayLength = 0.2f;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        isGrounded = Physics.Raycast(origin, Vector3.down, rayLength);
    }

    void takeDamage(int damage)
    {
        Hp -= damage;
    }
    public bool groundedCheck()
    {
        if (isGrounded)
            return true;
        else return false;

    }


}
