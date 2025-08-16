using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class playerController : MonoBehaviour
{
    [Header("General Stats")]
    public int Hp;


    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float jumpForce = 7f;
    public float sideStepDistance = 2f;
    public float sideWalkSpeed = 3f;
    public float rotationSpeed = 5f;
    public float tapThreshold = 0.3f;
    public float crouchHeight = 1f;
    public float crouchTransitionDuration = 0.15f;

    [Header("References")]
    public Transform opponent;
    public Animator animator;

    // Player State Machine
    private enum PlayerState
    {
        Idle, Walking, Dashing, Backdashing, Sidestepping, Sidewalking, Jumping, Crouching
    }
    private PlayerState currentState = PlayerState.Idle;

    // Input System & State
    private PlayerControls controls;
    private CharacterController characterController;
    private Vector2 moveInput;
    private float verticalVelocity = 0f;

    // Tap detection
    private float lastHorizontalTapTime = -1f;
    private float lastVerticalTapTime = -1f;

    // CharacterController dimensions
    private float originalHeight;
    private float originalCenterY;

    private Coroutine currentActionCoroutine;

    private void Awake()
    {
        controls = new PlayerControls();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Disable();
    }

    private void Update()
    {
        HandleFacing();
        ApplyGravity();
        HandleStateLogic();
    }

    private void HandleStateLogic()
    {
        // One-shot actions (like a dash or sidestep) complete via their Coroutine.
        // We do not want to interrupt them with continuous movement logic.
        if (currentState == PlayerState.Dashing || currentState == PlayerState.Backdashing || currentState == PlayerState.Sidestepping)
        {
            return;
        }

        // Continuous movement logic
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            currentState = PlayerState.Walking;
            HandleMovement();
        }
        else if (Mathf.Abs(moveInput.y) > 0.1f)
        {
            currentState = PlayerState.Sidewalking;
            HandleSidewalk();
        }
        else
        {
            // Only go to Idle if no movement input is detected
            if (currentState != PlayerState.Crouching && currentState != PlayerState.Jumping)
            {
                currentState = PlayerState.Idle;
                animator.SetFloat("MoveSpeed", 0f);
            }
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // We only care about horizontal taps for dashes
        float timeSinceLastHorizontalTap = Time.time - lastHorizontalTapTime;
        if (Mathf.Abs(moveInput.x) > 0.8f && timeSinceLastHorizontalTap < tapThreshold && characterController.isGrounded)
        {
            if (moveInput.x > 0)
                TriggerDash();
            else
                TriggerBackdash();

            lastHorizontalTapTime = -1f;
            return; // Exit to prevent double-tap also triggering walk
        }
        lastHorizontalTapTime = Time.time;

        // We only care about vertical taps for jumps, crouches, and sidesteps
        float timeSinceLastVerticalTap = Time.time - lastVerticalTapTime;
        if (Mathf.Abs(moveInput.y) > 0.8f)
        {
            // Double-tap vertical for Sidestep
            if (timeSinceLastVerticalTap < tapThreshold && characterController.isGrounded)
            {
                if (moveInput.y > 0)
                    TriggerSidestepOut(); // up, up
                else
                    TriggerSidestepIn(); // down, down

                lastVerticalTapTime = -1f;
                return; // Exit to prevent single-tap action as well
            }
            // Single-tap vertical for Jump/Crouch
            else
            {
                if (moveInput.y > 0 && characterController.isGrounded)
                    TryJump(); // up
                else if (moveInput.y < 0 && characterController.isGrounded)
                    TryCrouch(); // down
            }
        }
        lastVerticalTapTime = Time.time;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void HandleMovement()
    {
        if (opponent == null) return;

        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 moveDirection = toOpponent * moveInput.x;

        Vector3 move = moveDirection * moveSpeed;
        characterController.Move(move * Time.deltaTime);

        bool walkingForward = moveInput.x > 0.1f;
        bool walkingBackward = moveInput.x < -0.1f;

        animator.SetBool("IsWalkingForward", walkingForward);
        animator.SetBool("IsWalkingBackward", walkingBackward);
    }

    private void TriggerDash()
    {
        if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(DashRoutine(dashSpeed));
        currentState = PlayerState.Dashing;
        animator.SetTrigger("DashForward");
    }

    private void TriggerBackdash()
    {
        if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(DashRoutine(-dashSpeed));
        currentState = PlayerState.Backdashing;
        animator.SetTrigger("Backdash");
    }

    private IEnumerator DashRoutine(float speed)
    {
        float startTime = Time.time;
        Vector3 toOpponent = (opponent.position - transform.position).normalized;

        while (Time.time < startTime + dashDuration)
        {
            Vector3 dashDirection = toOpponent * (speed / moveSpeed);
            characterController.Move(dashDirection * moveSpeed * Time.deltaTime);
            yield return null;
        }
        currentState = PlayerState.Idle;
    }

    private void HandleSidewalk()
    {
        if (opponent == null) return;

        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toOpponent).normalized;

        Vector3 move = side * -moveInput.y * sideWalkSpeed;
        characterController.Move(move * Time.deltaTime);

        animator.SetFloat("SideWalkSpeed", Mathf.Abs(moveInput.y));
    }

    private void TriggerSidestepIn()
    {
        if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(SidestepRoutine(1));
        currentState = PlayerState.Sidestepping;
        animator.SetTrigger("SidestepLeft");
    }

    private void TriggerSidestepOut()
    {
        if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
        currentActionCoroutine = StartCoroutine(SidestepRoutine(-1));
        currentState = PlayerState.Sidestepping;
        animator.SetTrigger("SidestepRight");
    }

    private IEnumerator SidestepRoutine(float direction)
    {
        Vector3 toOpponent = (opponent.position - transform.position).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, toOpponent).normalized;
        Vector3 step = side * sideStepDistance * direction;

        characterController.Move(step);

        yield return new WaitForSeconds(0.2f);
        currentState = PlayerState.Idle;
    }

    private void HandleFacing()
    {
        if (opponent == null) return;
        Vector3 direction = opponent.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void TryJump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger("Jump");
            currentState = PlayerState.Jumping;
        }
    }

    private void TryCrouch()
    {
        if (characterController.isGrounded && currentState != PlayerState.Crouching)
        {
            if (currentActionCoroutine != null) StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = StartCoroutine(CrouchRoutine());
        }
    }

    private IEnumerator CrouchRoutine()
    {
        currentState = PlayerState.Crouching;
        animator.SetTrigger("Crouch");

        float time = 0;
        float heightChange = originalHeight - crouchHeight;
        Vector3 originalPosition = transform.position;

        // Crouch Down
        while (time < 1f)
        {
            float t = time / 1f;
            characterController.height = Mathf.Lerp(originalHeight, crouchHeight, t);
            characterController.center = new Vector3(characterController.center.x, Mathf.Lerp(originalCenterY, originalCenterY - heightChange / 2f, t), characterController.center.z);
            transform.position = Vector3.Lerp(originalPosition, new Vector3(originalPosition.x, originalPosition.y - heightChange, originalPosition.z), t);
            time += Time.deltaTime / crouchTransitionDuration;
            yield return null;
        }

        // Snap to final values
        characterController.height = crouchHeight;
        characterController.center = new Vector3(characterController.center.x, originalCenterY - heightChange / 2f, characterController.center.z);
        transform.position = new Vector3(originalPosition.x, originalPosition.y - heightChange, originalPosition.z);

        // Wait while the crouch is active
        while (moveInput.y < -0.8f && characterController.isGrounded)
        {
            yield return null;
        }

        // --- Un-Crouch ---

        // Check for a ceiling before standing up
        if (Physics.Raycast(transform.position, Vector3.up, originalHeight))
        {
            // If there's a ceiling, stay crouched
            currentState = PlayerState.Idle;
            yield break;
        }

        animator.SetTrigger("UnCrouch");

        time = 0;
        Vector3 crouchedPosition = transform.position;

        while (time < 1f)
        {
            float t = time / 1f;
            characterController.height = Mathf.Lerp(crouchHeight, originalHeight, t);
            characterController.center = new Vector3(characterController.center.x, Mathf.Lerp(originalCenterY - heightChange / 2f, originalCenterY, t), characterController.center.z);
            transform.position = Vector3.Lerp(crouchedPosition, originalPosition, t);
            time += Time.deltaTime / crouchTransitionDuration;
            yield return null;
        }

        characterController.height = originalHeight;
        characterController.center = new Vector3(characterController.center.x, originalCenterY, characterController.center.z);
        transform.position = originalPosition;
        currentState = PlayerState.Idle;
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
            if (currentState == PlayerState.Jumping)
            {
                animator.SetTrigger("Land");
                currentState = PlayerState.Idle;
            }
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        characterController.Move(moveVector * Time.deltaTime);
    }

    void takeDamage(int damage)
    {
        Hp -= damage;
    }
    //public bool groundedCheck()
    //{
    //    if (isGrounded)
    //        return true;
    //    else return false;

    //}


}
