using UnityEngine;
using System.Collections.Generic;

public class CPUController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [Header("AI Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float reactionTime = 0.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxDistanceToPlayer = 10f;

    [Header("Learning Parameters")]
    [Tooltip("How many frames of player movement to analyze.")]
    [SerializeField] private int analysisWindowSize = 5;
    [SerializeField] private float jumpThreshold = 0.5f;
    [SerializeField] private float sidestepThreshold = 0.5f;
    // AI State Machine
    private enum AIState { Idle, Approaching, Attacking, Punishing, Blocking }
    private AIState currentState;

    // Internal State and Data
    private float timeSinceLastDecision;
    private Vector3 lastPlayerPosition;
    private Queue<PlayerAction> playerActionHistory;

    // Helper class to store player action data
    private class PlayerAction
    {
        public bool isJumping;
        public bool isSidestepping;
    }

    private void Start()
    {
        currentState = AIState.Idle;
        lastPlayerPosition = player.position;
        playerActionHistory = new Queue<PlayerAction>();

        // Ensure we have all necessary components
        if (!characterController)
        {
            Debug.LogError("CharacterController not assigned on CPUController.");
        }
    }

    private void Update()
    {
        timeSinceLastDecision += Time.deltaTime;

        // Only make a new decision after the reaction time has passed
        if (timeSinceLastDecision >= reactionTime)
        {
            AnalyzePlayerBehavior();
            MakeDecision();
            timeSinceLastDecision = 0f;
        }

        FacePlayer();
        ExecuteCurrentState();
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }
    }

    private void AnalyzePlayerBehavior()
    {
        Vector3 movementDelta = player.position - lastPlayerPosition;

        bool isJumping = movementDelta.y > jumpThreshold;
        bool isSidestepping = Mathf.Abs(movementDelta.x) > sidestepThreshold;

        // Add the current action to the history
        playerActionHistory.Enqueue(new PlayerAction { isJumping = isJumping, isSidestepping = isSidestepping });

        // Keep the history size within the specified window
        if (playerActionHistory.Count > analysisWindowSize)
        {
            playerActionHistory.Dequeue();
        }

        lastPlayerPosition = player.position;
    }

    private void MakeDecision()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Analyze player history for common actions
        int sidestepCount = 0;
        int jumpCount = 0;
        foreach (var action in playerActionHistory)
        {
            if (action.isSidestepping) sidestepCount++;
            if (action.isJumping) jumpCount++;
        }

        // --- Decision-Making Hierarchy ---

        // 1. Punish predictable behavior
        if (sidestepCount >= analysisWindowSize)
        {
            currentState = AIState.Punishing;
            animator.SetTrigger("Sweep"); // Punish sidestep
            playerActionHistory.Clear(); // Reset history to focus on new behavior
            return;
        }

        if (jumpCount >= analysisWindowSize - 1) // Give a little leeway for jumps
        {
            currentState = AIState.Punishing;
            animator.SetTrigger("AntiAir");
            playerActionHistory.Clear();
            return;
        }

        // 2. Handle distance to player
        if (distance <= attackRange)
        {
            currentState = AIState.Attacking;
        }
        else if (distance > attackRange)
        {
            currentState = AIState.Approaching;
        }
        else if (distance > maxDistanceToPlayer)
        {
            // If the player is too far, just wait
            currentState = AIState.Idle;
        }

        // You could add a random chance to block or sidestep here
        // to make the AI less predictable.
    }

    private void ExecuteCurrentState()
    {
        // Use a switch statement to handle each state's logic
        switch (currentState)
        {
            case AIState.Idle:
                animator.SetFloat("MoveSpeed", 0f);
                break;

            case AIState.Approaching:
                MoveTowardsPlayer();
                break;

            case AIState.Attacking:
                animator.SetTrigger("Attack");
                currentState = AIState.Idle;
                break;

            case AIState.Punishing:
                break;

            case AIState.Blocking:
                animator.SetBool("IsBlocking", true);
                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        // Use CharacterController.Move for proper collision handling
        if (characterController)
        {
            characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // Fallback for simple movement
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        animator.SetFloat("MoveSpeed", 1f);
    }
}